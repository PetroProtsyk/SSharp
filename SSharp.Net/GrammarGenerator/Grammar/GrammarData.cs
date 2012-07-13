using System.Collections.Generic;
using System.Text;
using System;
using Scripting.SSharp.Parser.Ast;

namespace Scripting.SSharp.Parser
{
  internal class ParserData
  {   
    public const string Delimiters = ",;[](){}";
    public const string WhitespaceChars = " \t\r\n\v";
    public const string LineTerminators = "\n\r\v";
    public const string DecimalDigits = "1234567890";
    public const string OctalDigits = "12345670";
    public const string HexDigits = "1234567890aAbBcCdDeEfF";
    public const string BinaryDigits = "01";
    public static EscapeTable Escapes = new EscapeTable(){
         {'a', '\u0007'},
         {'b', '\b'},
         {'t', '\t'},
         {'n', '\n'},
         {'v', '\v'},
         {'f', '\f'},
         {'r', '\r'},
         {'"', '"'},
         {'\'', '\''},
         {'\\', '\\'},
         {' ', ' '},
         {'\n', '\n'},
      };  

    public Type DefaultNodeType = typeof(AstNode);

    public NonTerminal AugmentedRoot;

    //NOTE: Parser
    public ParserState InitialState;
    //NOTE: Parser
    public ParserState FinalState;

    //NOTE: Parser (not necessary)
    public readonly NonTerminalList NonTerminals = new NonTerminalList();
    //NOTE: Parser (not necessary)
    public readonly TerminalList Terminals = new TerminalList();
    //NOTE: Parser/Scanner
    public readonly TerminalLookupTable TerminalsLookup = new TerminalLookupTable();
    
    public readonly ProductionList Productions = new ProductionList();
    public readonly ParserStateList States = new ParserStateList();
    public readonly StringSet Errors = new StringSet();
  }
  
  //NOTE: Parser
  internal partial class ParserState
  {
    private static long counter = 0;
    public readonly long ID = counter++;

    public readonly string Name;
    //NOTE: Parser
    public readonly ActionRecordTable Actions = new ActionRecordTable();
    public readonly LRItemList Items = new LRItemList();

    public ParserState(string name, LR0ItemList coreItems)
    {
      Name = name;
      foreach (LR0Item coreItem in coreItems)
        Items.Add(new LRItem(this, coreItem));
    }

    public override string ToString()
    {
      return Name;
    }
  }

  //NOTE: Parser
  internal class ActionRecord
  {
    private static long counter = 0;
    public readonly long ID = counter++;
    public string Key;
    
    //NOTE: Parser
    public ParserActionType ActionType = ParserActionType.Shift;
    //NOTE: Parser
    public ParserState NewState;
    public ProductionList ReduceProductions = new ProductionList(); //may be more than one, in case of conflict

    internal ActionRecord(string key, ParserActionType type, ParserState newState, Production reduceProduction)
    {
      this.Key = key;
      this.ActionType = type;
      this.NewState = newState;
      if (reduceProduction != null)
        ReduceProductions.Add(reduceProduction);
    }
    
    //NOTE: Parser
    public Production Production
    {
      get { return ReduceProductions.Count > 0 ? ReduceProductions[0] : null; }
    }
    
    //NOTE: Parser
    public NonTerminal NonTerminal
    {
      get { return Production == null ? null : Production.LValue; }
    }
    //NOTE: Parser
    public int PopCount
    {
      get { return Production == null ? 0 : Production.RValues.Count; }
    }
  }

  internal class Production
  {
    private static long counter = 0;
    public readonly long ID = counter++;

    public readonly bool IsInitial;
    public readonly bool HasTerminals;
    public readonly bool IsError;        
    public readonly NonTerminal LValue;  
    public readonly BnfTermList RValues = new BnfTermList(); 
    public readonly LR0ItemList LR0Items = new LR0ItemList(); 
    
    public Production(bool isInitial, NonTerminal lvalue, BnfTermList rvalues)
    {
      LValue = lvalue;
      
      foreach (GrammarTerm rv in rvalues)
        if (rv != Grammar.Empty)
          RValues.Add(rv);

      foreach (GrammarTerm term in RValues)
      {
        Terminal terminal = term as Terminal;
        if (terminal == null) continue;
        HasTerminals = true;
        if (terminal.Category == TokenCategory.Error) IsError = true;
      }

      for (int p = 0; p <= RValues.Count; p++)
        LR0Items.Add(new LR0Item(this, p));
    }

    public bool IsEmpty()
    {
      return RValues.Count == 0;
    }
  }

  internal class LRItem
  {
    private static long counter = 0;
    public readonly long ID = counter++;

    public readonly ParserState State;
    public readonly LR0Item Core;
    public readonly LRItemList PropagateTargets = new LRItemList();
    public readonly StringSet Lookaheads = new StringSet();
    public readonly StringSet NewLookaheads = new StringSet();

    public LRItem(ParserState state, LR0Item core)
    {
      State = state;
      Core = core;
    }
  }

  internal class LR0Item
  {
    public readonly Production Production;
    public readonly StringSet TailFirsts = new StringSet();
    public readonly int Position;
    public bool TailIsNullable = false;

    internal int ID;
    internal static int instance_counter;

    public LR0Item(Production production, int position)
    {
      Production = production;
      Position = position;
      ID = instance_counter++;
    }

    public GrammarTerm NextElement
    {
      get
      {
        if (Position < Production.RValues.Count)
          return Production.RValues[Position];
        else
          return null;
      }
    }

    public bool IsKernel
    {
      get { return Position > 0 || (Production.IsInitial && Position == 0); }
    }
  }
}
