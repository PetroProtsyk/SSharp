/*
 * Copyright © 2011, Petro Protsyk, Denys Vuika
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *  http://www.apache.org/licenses/LICENSE-2.0
 *  
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using Scripting.SSharp.Parser.Ast;
using System.Reflection;

namespace Scripting.SSharp.Parser.FastGrammar
{
  internal partial class LRParser
  {
    #region Fields
    public const string LineTerminators = "\n\r\v";
    public const string WhitespaceChars = " \t\r\n\v";

    private readonly ParserStack _stack = new ParserStack();

    private CompilerContext _context;
    private IEnumerator<TokenAst> _input;
    private TokenAst _currentToken;
    private ParserState _currentState;
    
    private int _currentLine;
    private int _tokenCount;

    private ParserState InitialState;
    private ParserState FinalState;

    private Scanner Scanner = new Scanner();

    internal CompilerContext Context { get { return _context; } }
    #endregion

    #region Parsing methods
    private void Reset()
    {
      _stack.Reset();
      _currentState = InitialState;
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
        _currentToken = TokenAst.Create(LRParser.Eof, _context, new SourceLocation(0, _currentLine - 1, 0), string.Empty);
    }

    private AstNode Parse(CompilerContext context, IEnumerable<TokenAst> tokenStream)
    {
      _context = context;
      Reset();
      _input = tokenStream.GetEnumerator();
      NextToken();
      
      while (true)
      {
        if (_currentState == FinalState)
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

    public AstNode Parse(string source)
    {
      CompilerContext Context = new CompilerContext();
      Scanner.Prepare(Context, new SourceFile(source, "Source"));
      IEnumerable<TokenAst> tokenStream = Scanner.BeginScan();
      AstNode rootNode = Parse(Context, tokenStream);
      return rootNode;
    }
    #endregion

    #region Error reporting and recovery
    private void ReportError(SourceLocation location, string message, params object[] args)
    {
      if (args != null && args.Length > 0)
        message = string.Format(message, args);
      _context.AddError(location, message, null);
    }

    private void ReportScannerError()
    {
      _context.AddError(_currentToken.Location, _currentToken.Text, null);
    }

    private void ReportParserError()
    {
      if (_currentToken.Terminal == LRParser.Eof)
      {
        ReportError(_currentToken.Location, "Unexpected end of file.");
        return;
      }

      string message = "Syntax error";
      ReportError(_currentToken.Location, message);
    }

    internal static TokenAst CreateSyntaxErrorToken(CompilerContext context, SourceLocation location, string message, params object[] args)
    {
      if (args != null && args.Length > 0)
        message = string.Format(message, args);
      return TokenAst.Create(SyntaxError, context, location, message);
    }

    internal readonly static Terminal Empty = new Terminal("EMPTY");
    internal readonly static Terminal Eof = new Terminal("EOF", TokenCategory.Outline);
    internal readonly static Terminal SyntaxError = new Terminal("SYNTAX_ERROR", TokenCategory.Error);

    internal bool IsPseudoTerminal(Terminal term)
    {
      return term == Empty || term == Eof || term == SyntaxError;
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

        ITerminal prevTerm = term as ITerminal;
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
      IGrammarTerm nonTeminal = reduceAction.NonTerminal;
      AstNode result;

      AstNodeArgs args = new AstNodeArgs(nonTeminal, sourceSpan, childNodes);
 
      Type ntNodeType = nonTeminal.NodeType ?? typeof(AstNode);

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
        Type childNodeType = childNodes[0].Term.NodeType ?? typeof(AstNode);
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
