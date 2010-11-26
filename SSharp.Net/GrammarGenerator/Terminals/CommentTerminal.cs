using System.Collections.Generic;
using System;
using Scripting.SSharp.Parser.Ast;

namespace Scripting.SSharp.Parser
{
  internal class CommentTerminal : Terminal
  {
    public CommentTerminal(string name, string startSymbol, string endSymbol)
      : base(name, TokenCategory.Comment)
    {
      _startSymbol = startSymbol;
      _endSymbol = endSymbol;
      SetOption(TermOptions.IsNonGrammar);
    }

    private string _startSymbol;
    private string _endSymbol;
    private bool _isLineComment; 

    #region overrides
    public override void Init(Grammar grammar)
    {
      base.Init(grammar);
      _isLineComment |= _endSymbol.Contains("\n");
    }

    public override TokenAst TryMatch(CompilerContext context, ISourceStream source)
    {
      if (!source.MatchSymbol(_startSymbol, false)) return null;
      source.Position += _startSymbol.Length;

      while (!source.EOF())
      {
        int firstCharPos = source.Text.IndexOf(_endSymbol, source.Position);

        if (firstCharPos < 0)
        {
          source.Position = source.Text.Length;

          if (_isLineComment)
            return TokenAst.Create(this, context, source.TokenStart, source.GetLexeme());
          else
            return Grammar.CreateSyntaxErrorToken(context, source.TokenStart, "Unclosed comment block");
        }
      
        source.Position = firstCharPos;
        if (source.MatchSymbol(_endSymbol, false))
        {
          source.Position += _endSymbol.Length;
          return TokenAst.Create(this, context, source.TokenStart, source.GetLexeme());
        }
        
        source.Position++; 
      }

      throw new NotSupportedException();
    }

    public override IList<string> GetFirsts()
    {
      return new string[] { _startSymbol };
    }
    #endregion
  }
}
