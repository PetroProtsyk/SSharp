using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Scripting.SSharp.Parser.Ast;

namespace Scripting.SSharp.Parser
{
  internal class Parser
  {
    #region Constructors
    public Parser(ParserData data)
    {
      Data = data;
    }
    #endregion

    #region Fields
    private readonly ParserData Data;
    private readonly ParserStack _stack = new ParserStack();

    private CompilerContext _context;
    private IEnumerator<TokenAst> _input;
    private TokenAst _currentToken;
    private ParserState _currentState;
    private int _currentLine;
    private int _tokenCount;
    #endregion

    #region Parsing methods
    private void Reset()
    {
      _stack.Reset();
      _currentState = Data.InitialState;
      _currentLine = 0;
      _tokenCount = 0;
      _context.Errors.Clear();
    }

    private TokenAst ReadToken()
    {
      while (_input.MoveNext())
      {
        TokenAst result = _input.Current;
        _tokenCount++;
        _currentLine = result.Span.Start.Line + 1;
        
        if (result.Terminal.IsSet(TermOptions.IsNonGrammar))
          continue;

        return result;
      }

      return null;
    }

    private void NextToken()
    {
      _currentToken = ReadToken();

      if (_currentToken == null)
        _currentToken = TokenAst.Create(Grammar.Eof, _context, new SourceLocation(0, _currentLine - 1, 0), string.Empty);
    }

    public AstNode Parse(CompilerContext context, IEnumerable<TokenAst> tokenStream)
    {
      _context = context;
      Reset();
      _input = tokenStream.GetEnumerator();
      NextToken();
      
      while (true)
      {
        if (_currentState == Data.FinalState)
        {
          AstNode result = _stack[0].Node;
          _stack.Reset();
          return result;
        }
        
        if (_currentToken.Terminal.Category == TokenCategory.Error)
        {
          ReportScannerError();
          return null;
        }

        ActionRecord action = GetCurrentAction();
        if (action == null)
        {
          ReportParserError();          
          return null;
        }

        //TODO: perform conflict resolving
        //if (action.HasConflict())

        switch (action.ActionType)
        {
          case ParserActionType.Operator:
            if (GetActionTypeForOperation(_currentToken) == ParserActionType.Shift)
              goto case ParserActionType.Shift;
            else
              goto case ParserActionType.Reduce;

          case ParserActionType.Shift:
            ExecuteShiftAction(action);
            break;

          case ParserActionType.Reduce:
            ExecuteReduceAction(action);
            break;
        }
      }
    }
    #endregion

    #region Error reporting and recovery
    private void ReportError(SourceLocation location, string message, params object[] args)
    {
      if (args != null && args.Length > 0)
        message = string.Format(message, args);
      _context.AddError(location, message, _currentState);
    }

    private void ReportScannerError()
    {
      _context.AddError(_currentToken.Location, _currentToken.Text, _currentState);
    }

    private void ReportParserError()
    {
      if (_currentToken.Terminal == Grammar.Eof)
      {
        ReportError(_currentToken.Location, "Unexpected end of file.");
        return;
      }
      StringList expectedList = GetCurrentExpectedSymbols();
      string message = null;
      if (message == null)
        message = "Syntax error" + (expectedList.Count == 0 ? "." : ", expected: " + expectedList.ToString(" ").Replace("\b"," "));
      ReportError(_currentToken.Location, message);
    }

    private StringList GetCurrentExpectedSymbols()
    {
      BnfTermList inputElements = new BnfTermList();
      StringSet inputKeys = new StringSet();
      inputKeys.AddRange(_currentState.Actions.Keys);
      //First check all NonTerminals
      foreach (NonTerminal nt in Data.NonTerminals)
      {
        if (!inputKeys.Contains(nt.Key)) continue;
        //nt is one of our available inputs; check if it has an alias. If not, don't add it to element list;
        // and we have already all its "Firsts" keys in the list. 
        // If yes, add nt to element list and remove
        // all its "fists" symbols from the list. These removed symbols will be represented by single nt alias. 
        if (string.IsNullOrEmpty(nt.DisplayName))
          inputKeys.Remove(nt.Key);
        else
        {
          inputElements.Add(nt);
          foreach (string first in nt.Firsts)
            inputKeys.Remove(first);
        }
      }
      //Now terminals
      foreach (Terminal term in Data.Terminals)
      {
        if (inputKeys.Contains(term.Key))
          inputElements.Add(term);
      }
      StringList result = new StringList();
      foreach (GrammarTerm term in inputElements)
        result.Add(string.IsNullOrEmpty(term.DisplayName) ? term.Name : term.DisplayName);
      result.Sort();
      return result;
    }
    #endregion

    #region Misc private methods
    private ActionRecord GetCurrentAction()
    {
      ActionRecord action = null;
      if (_currentToken.MatchByValue)
      {
        if (_currentState.Actions.TryGetValue(_currentToken.Text, out action))
          return action;
      }

      if (_currentToken.MatchByType && _currentState.Actions.TryGetValue(_currentToken.Terminal.Key, out action))
          return action;

      return null;
    }

    private ParserActionType GetActionTypeForOperation(TokenAst current)
    {
      ITerminal thisTerm = current.Terminal;
      for (int i = _stack.Count - 2; i >= 0; i--)
      {
        if (_stack[i].Node == null) continue;

        IGrammarTerm term = _stack[i].Node.Term;
        if (!term.IsSet(TermOptions.IsOperator)) continue;

        Terminal prevTerm = term as Terminal;
        if (prevTerm.Precedence == thisTerm.Precedence)
          return thisTerm.Associativity == Associativity.Left ? ParserActionType.Reduce : ParserActionType.Shift;

        ParserActionType result = prevTerm.Precedence > thisTerm.Precedence ? ParserActionType.Reduce : ParserActionType.Shift;
        return result;
      }

      return ParserActionType.Shift;
    }
    
    private void ExecuteShiftAction(ActionRecord action)
    {
      _stack.Push(_currentToken, _currentState);
      _currentState = action.NewState;
      NextToken();
    }

    private void ExecuteReduceAction(ActionRecord action)
    {
      ParserState oldState = _currentState;
      int popCnt = action.PopCount;

      AstNodeList childNodes = new AstNodeList();
      for (int i = 0; i < action.PopCount; i++)
      {
        AstNode child = _stack[_stack.Count - popCnt + i].Node;
        if (!child.Term.IsSet(TermOptions.IsPunctuation))
          childNodes.Add(child);
      }

      SourceSpan newNodeSpan;
      if (popCnt == 0)
      {
        newNodeSpan = new SourceSpan(_currentToken.Location, 0);
      }
      else
      {
        SourceLocation firstPopLoc = _stack[_stack.Count - popCnt].Node.Location;
        int lastPopEndPos = _stack[_stack.Count - 1].Node.Span.EndPos;
        newNodeSpan = new SourceSpan(firstPopLoc, lastPopEndPos - firstPopLoc.Position);
        _currentState = _stack[_stack.Count - popCnt].State;
        _stack.Pop(popCnt);
      }
      
      AstNode node = CreateNode(action, newNodeSpan, childNodes);
      _stack.Push(node, _currentState);

      ActionRecord gotoAction;
      if (_currentState.Actions.TryGetValue(action.NonTerminal.Key, out gotoAction))
      {
        _currentState = gotoAction.NewState;
      }
      else
        throw new CompilerException(string.Format("Cannot find transition for input {0}; state: {1}, popped state: {2}",
              action.NonTerminal, oldState, _currentState));
    }


    private AstNode CreateNode(ActionRecord reduceAction, SourceSpan sourceSpan, AstNodeList childNodes)
    {
      NonTerminal nonTeminal = reduceAction.NonTerminal;
      AstNode result;
      AstNodeArgs args = new AstNodeArgs(nonTeminal, sourceSpan, childNodes);

      Type defaultNodeType = _context.Compiler.Data.DefaultNodeType;
      Type ntNodeType = nonTeminal.NodeType ?? defaultNodeType ?? typeof(AstNode);

      bool isList = nonTeminal.IsSet(TermOptions.IsList);
      if (isList && childNodes.Count > 1 && childNodes[0].Term == nonTeminal)
      {
        result = childNodes[0];
        AstNode newChild = childNodes[childNodes.Count - 1];
        newChild.Parent = result;
        result.ChildNodes.Add(newChild);
        return result;
      }

      if (nonTeminal.IsSet(TermOptions.IsStarList) && childNodes.Count == 1)
      {
        childNodes = childNodes[0].ChildNodes;
      }

      if (!isList && !nonTeminal.IsSet(TermOptions.IsPunctuation) && childNodes.Count == 1)
      {
        Type childNodeType = childNodes[0].Term.NodeType ?? defaultNodeType ?? typeof(AstNode);
        if (childNodeType == ntNodeType || childNodeType.IsSubclassOf(ntNodeType))
          return childNodes[0];
      }

      result = null;
      if (ntNodeType == typeof(AstNode))
      {
        result = new AstNode(args);
      }
      else
      {
        ConstructorInfo ctor = ntNodeType.GetConstructor(new Type[] { typeof(AstNodeArgs) });
        if (ctor == null)
          throw new Exception("Failed to located constructor: " + ntNodeType.ToString() + "(AstNodeArgs args)");

        result = (AstNode)ctor.Invoke(new object[] { args });
      }

      return result;
    }
    #endregion
  }
}
