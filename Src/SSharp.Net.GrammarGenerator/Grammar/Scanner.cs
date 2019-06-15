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

using System.Collections.Generic;
using Scripting.SSharp.Parser.Ast;

namespace Scripting.SSharp.Parser
{
  internal class Scanner
  {
    #region Constructor
    public Scanner(ParserData data)
    {
      _data = data;
      _lineTerminators = ParserData.LineTerminators.ToCharArray();
    }
    #endregion

    #region Fields
    private ParserData _data;
    private ISourceStream _source;
    private CompilerContext _context;
    private char[] _lineTerminators;
    private TokenAst _currentToken;
    private int _nextNewLinePosition = -1;
    private TokenList _bufferedTokens = new TokenList();

    private static char[] _tab_arr = { '\t' };
    #endregion

    #region Methods
    public void Prepare(CompilerContext context, ISourceStream source)
    {
      _context = context;
      _source = source;
      _currentToken = null;
      _bufferedTokens.Clear();
      ResetSource();
    }

    public IEnumerable<TokenAst> BeginScan()
    {
      while (true)
      {
        _currentToken = ReadToken();
        yield return _currentToken;
       
        if (_currentToken.Terminal == Grammar.Eof)
          yield break;
      }
    }

    private TokenAst ReadToken()
    {
      if (_bufferedTokens.Count > 0)
      {
        TokenAst tkn = _bufferedTokens[0];
        _bufferedTokens.RemoveAt(0);
        return tkn;
      }

      SkipWhiteSpaces();
      SetTokenStartLocation();
      
      if (_source.EOF())
        return TokenAst.Create(Grammar.Eof, _context, _source.TokenStart, string.Empty, Grammar.Eof.Name);

      TerminalList terms = SelectTerminals(_source.CurrentChar);
      TokenAst result = MatchTerminals(terms);
      
      if (result != null && !result.IsError())
      {
        _source.Position = _source.TokenStart.Position + result.Length;
        return result;
      }

      if (result == null)
        result = Grammar.CreateSyntaxErrorToken(_context, _source.TokenStart, "Invalid character: '{0}'", _source.CurrentChar);

      return result;
    }

    private void SkipWhiteSpaces()
    {
      while (ParserData.WhitespaceChars.IndexOf(_source.CurrentChar) >= 0)
        _source.Position++;
    }

    private TokenAst MatchTerminals(TerminalList terminals)
    {
      TokenAst result = null;
      foreach (Terminal term in terminals)
      {
        if (result != null && result.Terminal.Priority > term.Priority)
          break;

        _source.Position = _source.TokenStart.Position;
        TokenAst token = term.TryMatch(_context, _source);

        if (token != null && (token.IsError() || result == null || token.Length > result.Length))
          result = token;

        if (result != null && result.IsError()) break;
      }

      return result;
    }

    private TerminalList SelectTerminals(char current)
    {
      TerminalList result;

      if (_data.TerminalsLookup.TryGetValue(current, out result))
        return result;

      throw new CompilerException("Can't select terminal for start char " + current);
    }

    public void ResetSource()
    {
      _source.Position = 0;
      _source.TokenStart = new SourceLocation();
      _nextNewLinePosition = _source.Text.IndexOf('\n');
    }

    private void SetTokenStartLocation()
    {
      SourceLocation tokenStart = _source.TokenStart;
      int newPosition = _source.Position;
      string text = _source.Text;

      if (newPosition <= _nextNewLinePosition || _nextNewLinePosition < 0)
      {
        tokenStart.Column += newPosition - tokenStart.Position;
        tokenStart.Position = newPosition;
        _source.TokenStart = tokenStart;
        return;
      }

      int lineStart = _nextNewLinePosition;
      int nlCount = 1;
      CountCharsInText(text, _lineTerminators, lineStart + 1, newPosition - 1, ref nlCount, ref lineStart);
      tokenStart.Line += nlCount;

      int tabCount = 0;
      int dummy = 0;
      if (_source.TabWidth > 1)
        CountCharsInText(text, _tab_arr, lineStart, newPosition - 1, ref tabCount, ref dummy);

      tokenStart.Position = newPosition;
      tokenStart.Column = newPosition - lineStart - 1;
      if (tabCount > 0)
        tokenStart.Column += (_source.TabWidth - 1) * tabCount;

      _nextNewLinePosition = text.IndexOfAny(_lineTerminators, newPosition);
      _source.TokenStart = tokenStart;
    }

    private void CountCharsInText(string text, char[] chars, int from, int until, ref int count, ref int lastPosition)
    {
      if (from > until) return;
      while (true)
      {
        int next = text.IndexOfAny(chars, from, until - from + 1);
        if (next < 0) return;

        bool isCRLF = (text[next] == '\n' && next > 0 && text[next - 1] == '\r');
        if (!isCRLF)
          count++; 
        lastPosition = next;
        from = next + 1;
      }

    }
    #endregion
  }
}
