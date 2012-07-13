using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Scripting.SSharp.Parser
{
  internal class GrammarDataBuilder
  {
    #region Fields
    private Grammar _grammar;
    private ParserStateTable _stateHash;
    private readonly ParserData _data = new ParserData();
    private int _unnamedCount;
    #endregion

    #region Construction
    public GrammarDataBuilder(Grammar grammar)
    {
      _grammar = grammar;
    }

    public ParserData Build()
    {
      if (_grammar.Root == null)
        Cancel("Root property of the grammar is not set.");

      _data.AugmentedRoot = new NonTerminal(_grammar.Root.Name + "'") { Rule = new GrammarExpression(_grammar.Root) };

      CollectAllElements();

      CreateProductions();

      CalculateNullability();
      
      CalculateFirsts();
      
      CalculateTailFirsts();

      CreateParserStates();

      PropagateLookaheads();

      CreateReduceActions();

      CheckActionConflicts();

      InitAll();

      BuildTerminalsLookupTable();

      ValidateAll();

      Clear();

      return _data;
    }

    private void Cancel(string msg)
    {
      if (msg == null) msg = "Grammar analysis canceled.";
      throw new GrammarErrorException(msg);
    }
    #endregion

    #region Methods
    private void CollectAllElements()
    {
      _data.NonTerminals.Clear();
      _data.Terminals.Clear();

      foreach (Terminal terminal in _grammar.NonGrammarTerminals)
      {
        terminal.SetOption(TermOptions.IsNonGrammar);
        _data.Terminals.Add(terminal);
      }

      _unnamedCount = 0;

      CollectAllElementsRecursive(_data.AugmentedRoot);
      _data.Terminals.Sort(Terminal.ByName);
    }

    private void CollectAllElementsRecursive(GrammarTerm element)
    {
      Terminal terminal = element as Terminal;
      if (terminal != null && !_data.Terminals.Contains(terminal) && !_grammar.IsPseudoTerminal(terminal))
      {
        _data.Terminals.Add(terminal);
        return;
      }

      NonTerminal nonTerminal = element as NonTerminal;
      if (nonTerminal == null || _data.NonTerminals.Contains(nonTerminal))
        return;

      if (nonTerminal.Name == null)
      {
        if (nonTerminal.Rule != null && !string.IsNullOrEmpty(nonTerminal.Rule.Name))
          nonTerminal.Name = nonTerminal.Rule.Name;
        else
          nonTerminal.Name = "NT" + (_unnamedCount++);
      }

      _data.NonTerminals.Add(nonTerminal);
      
      if (nonTerminal.Rule == null)
      {
        Cancel(string.Format("Non-terminal {0} has uninitialized Rule property.", nonTerminal.Name));
      }

      foreach (BnfTermList elemList in nonTerminal.Rule.Data)
        for (int i = 0; i < elemList.Count; i++)
        {
          GrammarTerm child = elemList[i];
          if (child == null)
            Cancel(string.Format("Rule for NonTerminal {0} contains null as an operand in position {1} in one of productions.", nonTerminal, i));
      
          GrammarExpression expr = child as GrammarExpression;
          if (expr != null)
          {
            child = new NonTerminal(null) { Rule = expr };
            elemList[i] = child;
          }

          CollectAllElementsRecursive(child);
        }
    }

    private void BuildTerminalsLookupTable()
    {
      _data.TerminalsLookup.Clear();
      foreach (Terminal term in _data.Terminals)
      {
        IList<string> prefixes = term.GetFirsts();

        foreach (string prefix in prefixes)
        {
          if (string.IsNullOrEmpty(prefix)) continue;

          char hashKey = prefix[0];
          TerminalList currentList;

          if (!_data.TerminalsLookup.TryGetValue(hashKey, out currentList))
          {
            currentList = new TerminalList();
            _data.TerminalsLookup[hashKey] = currentList;
          }
          
          currentList.Add(term);
        }
      }
      
      foreach (TerminalList list in _data.TerminalsLookup.Values)
        if (list.Count > 1)
          list.Sort(Terminal.ByPriorityReverse);
    }

    private void CreateProductions()
    {
      _data.Productions.Clear();

      LR0Item.instance_counter = 0;
      foreach (NonTerminal nt in _data.NonTerminals)
      {
        nt.Productions.Clear();
        BnfExpressionData allData = new BnfExpressionData();
        allData.AddRange(nt.Rule.Data);

        foreach (BnfTermList prodOperands in allData)
        {
          bool isInitial = (nt == _data.AugmentedRoot);
          Production prod = new Production(isInitial, nt, prodOperands);
          nt.Productions.Add(prod);
          _data.Productions.Add(prod);
        }
      }
    }

    private void CalculateNullability()
    {
      NonTerminalList undecided = _data.NonTerminals;
      while (undecided.Count > 0)
      {
        NonTerminalList newUndecided = new NonTerminalList();
        foreach (NonTerminal nt in undecided)
          if (!CalculateNullability(nt, undecided))
            newUndecided.Add(nt);
        if (undecided.Count == newUndecided.Count) return; 
        undecided = newUndecided;
      }
    }

    private bool CalculateNullability(NonTerminal nonTerminal, NonTerminalList undecided)
    {
      foreach (Production prod in nonTerminal.Productions)
      {
        if (prod.HasTerminals) continue;
        if (prod.IsEmpty())
        {
          nonTerminal.Nullable = true;
          return true;
        }

        bool allNullable = true;
        foreach (GrammarTerm term in prod.RValues)
        {
          NonTerminal nt = term as NonTerminal;
          if (nt != null)
            allNullable &= nt.Nullable;
        }

        if (allNullable)
        {
          nonTerminal.Nullable = true;
          return true;
        }
      }
      return false;
    }

    private void CalculateFirsts()
    {
 
      foreach (Production prod in _data.Productions)
      {
        foreach (GrammarTerm term in prod.RValues)
        {
          if (term is Terminal)
          { 
            prod.LValue.Firsts.Add(term.Key); 
            break;
          }
          NonTerminal nt = term as NonTerminal;
          if (!nt.PropagateFirstsTo.Contains(prod.LValue))
            nt.PropagateFirstsTo.Add(prod.LValue); 
          if (!nt.Nullable) break;
        }
      }

      NonTerminalList workList = _data.NonTerminals;
      while (workList.Count > 0)
      {
        NonTerminalList newList = new NonTerminalList();
        foreach (NonTerminal nt in workList)
        {
          foreach (NonTerminal toNt in nt.PropagateFirstsTo)
            foreach (string symbolKey in nt.Firsts)
            {
              if (!toNt.Firsts.Contains(symbolKey))
              {
                toNt.Firsts.Add(symbolKey);
                if (!newList.Contains(toNt))
                  newList.Add(toNt);
              }
            }
        }
        workList = newList;
      }
    }

    private void CalculateTailFirsts()
    {
      foreach (Production prod in _data.Productions)
      {
        StringSet accumulatedFirsts = new StringSet();
        bool allNullable = true;

        for (int i = prod.LR0Items.Count - 1; i >= 0; i--)
        {
          LR0Item item = prod.LR0Items[i];
          if (i >= prod.LR0Items.Count - 2)
          {
            item.TailIsNullable = true;
            item.TailFirsts.Clear();
            continue;
          }
          GrammarTerm term = prod.RValues[item.Position + 1]; 
          NonTerminal ntElem = term as NonTerminal;
          if (ntElem == null || !ntElem.Nullable)
          { 
            accumulatedFirsts.Clear();
            allNullable = false;
            item.TailIsNullable = false;
            if (ntElem == null)
            {
              item.TailFirsts.Add(term.Key);
              accumulatedFirsts.Add(term.Key);
            }
            else
            {
              item.TailFirsts.AddRange(ntElem.Firsts); 
              accumulatedFirsts.AddRange(ntElem.Firsts);
            }
            continue;
          }
          accumulatedFirsts.AddRange(ntElem.Firsts);
          item.TailFirsts.AddRange(accumulatedFirsts);
          item.TailIsNullable = allNullable;
        }
      }
    }

    private void CreateInitialAndFinalStates()
    {
      LR0ItemList itemList = new LR0ItemList();
      itemList.Add(_data.AugmentedRoot.Productions[0].LR0Items[0]);
      _data.InitialState = FindOrCreateState(itemList);
      _data.InitialState.Items[0].NewLookaheads.Add(Grammar.Eof.Key);

      itemList.Clear();
      itemList.Add(_data.AugmentedRoot.Productions[0].LR0Items[1]);

      _data.FinalState = FindOrCreateState(itemList);
      _data.InitialState.Actions[_data.AugmentedRoot.Key] =
        new ActionRecord(_data.AugmentedRoot.Key, ParserActionType.Shift, _data.FinalState, null);
    }

    private void CreateParserStates()
    {
      _data.States.Clear();
      _stateHash = new ParserStateTable();
      CreateInitialAndFinalStates();

      string augmRootKey = _data.AugmentedRoot.Key;

      for (int index = 0; index < _data.States.Count; index++)
      {
        ParserState state = _data.States[index];
        AddClosureItems(state);
    
        Dictionary<string, LR0ItemList> shiftTable = GetStateShifts(state);

        foreach (string input in shiftTable.Keys)
        {
          LR0ItemList shiftedCoreItems = shiftTable[input];
          ParserState newState = FindOrCreateState(shiftedCoreItems);

          state.Actions[input] = new ActionRecord(input, ParserActionType.Shift, newState, null);

          foreach (LR0Item coreItem in shiftedCoreItems)
          {
            LRItem fromItem = FindItem(state, coreItem.Production, coreItem.Position - 1);
            LRItem toItem = FindItem(newState, coreItem.Production, coreItem.Position);
            if (!fromItem.PropagateTargets.Contains(toItem))
              fromItem.PropagateTargets.Add(toItem);
          }
        }
      }

      _data.FinalState = _data.InitialState.Actions[_data.AugmentedRoot.Key].NewState;
    }

    private LRItem TryFindItem(ParserState state, LR0Item core)
    {
      foreach (LRItem item in state.Items)
        if (item.Core == core)
          return item;
      return null;
    }

    private LRItem FindItem(ParserState state, Production production, int position)
    {
      foreach (LRItem item in state.Items)
        if (item.Core.Production == production && item.Core.Position == position)
          return item;
      string msg = string.Format("Failed to find an LRItem in state {0} by production [{1}] and position {2}. ",
        state, production.ToString(), position.ToString());
      throw new CompilerException(msg);
    }

    private Dictionary<string, LR0ItemList> GetStateShifts(ParserState state)
    {
      Dictionary<string, LR0ItemList> shifts = new Dictionary<string, LR0ItemList>();
      LR0ItemList list;
      foreach (LRItem item in state.Items)
      {
        GrammarTerm term = item.Core.NextElement;
        if (term == null) continue;
        LR0Item shiftedItem = item.Core.Production.LR0Items[item.Core.Position + 1];
        if (!shifts.TryGetValue(term.Key, out list))
          shifts[term.Key] = list = new LR0ItemList();
        list.Add(shiftedItem);
      }
      return shifts;
    }

    private ParserState FindOrCreateState(LR0ItemList lr0Items)
    {
      string key = CalcItemListKey(lr0Items);
      ParserState result;
      if (_stateHash.TryGetValue(key, out result))
        return result;
      result = new ParserState("S" + _data.States.Count, lr0Items);
      _data.States.Add(result);
      _stateHash[key] = result;
      return result;
    }

    private bool AddClosureItems(ParserState state)
    {
      bool result = false;
      for (int i = 0; i < state.Items.Count; i++)
      {
        LRItem item = state.Items[i];
        NonTerminal nextNT = item.Core.NextElement as NonTerminal;
        if (nextNT == null) continue;

        foreach (Production prod in nextNT.Productions)
        {
          LR0Item core = prod.LR0Items[0]; 
          LRItem newItem = TryFindItem(state, core);
          if (newItem == null)
          {
            newItem = new LRItem(state, core);
            state.Items.Add(newItem);
            result = true;
          }

          newItem.NewLookaheads.AddRange(item.Core.TailFirsts);
          if (item.Core.TailIsNullable && !item.PropagateTargets.Contains(newItem))
            item.PropagateTargets.Add(newItem);
        }
      }

      return result;
    }

    private string CalcItemListKey(LR0ItemList items)
    {
      items.Sort((x, y) => x.ID - y.ID); //Sort by ID
      if (items.Count == 0) return "";

      if (items.Count == 1 && items[0].IsKernel)
        return items[0].ID.ToString();

      StringBuilder sb = new StringBuilder(1024);
      foreach (LR0Item item in items)
      {
        if (item.IsKernel)
        {
          sb.Append(item.ID);
          sb.Append(",");
        }
      }
      return sb.ToString();
    }

    private void PropagateLookaheads()
    {
      LRItemList currentList = new LRItemList();
      _data.States.ForEach(state => currentList.AddRange(state.Items));

      while (currentList.Count > 0)
      {
        LRItemList newList = new LRItemList();
        
        foreach (LRItem item in currentList)
        {
          if (item.NewLookaheads.Count == 0) continue;
        
          int oldCount = item.Lookaheads.Count;
          item.Lookaheads.AddRange(item.NewLookaheads);
          
          if (item.Lookaheads.Count != oldCount)
          {
            foreach (LRItem targetItem in item.PropagateTargets)
            {
              targetItem.NewLookaheads.AddRange(item.NewLookaheads);
              newList.Add(targetItem);
            }
          }

          item.NewLookaheads.Clear();
        }

        currentList = newList;
      }
    }

    private void CreateReduceActions()
    {
      foreach (ParserState state in _data.States)
      {
        foreach (LRItem item in state.Items)
        {
          if (item.Core.NextElement != null) continue;
          foreach (string lookahead in item.Lookaheads)
          {
            ActionRecord action;
            if (state.Actions.TryGetValue(lookahead, out action))
              action.ReduceProductions.Add(item.Core.Production);
            else
              state.Actions[lookahead] = new ActionRecord(lookahead, ParserActionType.Reduce, null, item.Core.Production);
          }
        }
      }
    }

    private void CheckActionConflicts()
    {
      StringDictionary errorTable = new StringDictionary();
      foreach (ParserState state in _data.States)
      {
        foreach (ActionRecord action in state.Actions.Values)
        {
          if (action.NewState != null && action.ReduceProductions.Count == 0)
            continue;

          if (action.NewState == null && action.ReduceProductions.Count == 1)
          {
            action.ActionType = ParserActionType.Reduce;
            continue;
          }

          if (action.NewState != null && action.ReduceProductions.Count > 0)
          {
            SymbolTerminal opTerm = SymbolTerminal.GetSymbol(action.Key);
            if (opTerm != null && opTerm.IsSet(TermOptions.IsOperator))
            {
              action.ActionType = ParserActionType.Operator;
            }
            else
            {
              AddErrorForInput(errorTable, action.Key, "Shift-reduce conflict in state {0}, reduce production: {1}",
                  state, action.ReduceProductions[0]);
            }
          }

          if (action.ReduceProductions.Count > 1)
          {
            AddErrorForInput(errorTable, action.Key, "Reduce-reduce conflict in state {0} in productions: {1} ; {2}",
                state, action.ReduceProductions[0], action.ReduceProductions[1]);
          }

        }
      }

      foreach (string msg in errorTable.Keys)
      {
        _data.Errors.Add(msg + " on inputs: " + errorTable[msg]);
      }
    }

    private void InitAll()
    {
      foreach (Terminal term in _data.Terminals)
        term.Init(_grammar);
      foreach (NonTerminal nt in _data.NonTerminals)
        nt.Init(_grammar);
    }

    private void ValidateAll()
    {
      StringSet ntList = new StringSet();
      foreach (NonTerminal nt in _data.NonTerminals)
      {
        if (nt == _data.AugmentedRoot) continue;
        BnfExpressionData data = nt.Rule.Data;
        if (data.Count == 1 && data[0].Count == 1 && data[0][0] is NonTerminal)
          ntList.Add(nt.Name);
      }
      if (ntList.Count > 0)
      {
        AddError("Warning: Possible non-terminal duplication. The following non-terminals have rules containing a single non-terminal: \r\n {0}. \r\n" +
         "Consider merging two non-terminals; you may need to use 'nt1 = nt2;' instead of 'nt1.Rule=nt2'.");
      }
    }

    private void AddError(string message, params object[] args)
    {
      if (args != null && args.Length > 0)
        message = string.Format(message, args);
      _data.Errors.Add(message);
    }

    private void AddErrorForInput(StringDictionary errors, string input, string template, params object[] args)
    {
      string msg = string.Format(template, args);
      string tmpInputs;
      errors.TryGetValue(msg, out tmpInputs);
      errors[msg] = tmpInputs + input + " ";
    }

    private void Clear()
    {
      SymbolTerminal.ClearSymbols();
    }
    #endregion
  }
}
