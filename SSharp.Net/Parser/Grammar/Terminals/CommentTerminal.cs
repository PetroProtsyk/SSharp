using System;
using Scripting.SSharp.Parser.Ast;

namespace Scripting.SSharp.Parser.FastGrammar
{
  internal sealed class CommentTerminal : Terminal
  {
    public CommentTerminal(string name, string startSymbol, string endSymbol)
      : base(name)
    {
      Category = TokenCategory.Comment;
      _startSymbol = startSymbol;
      _endSymbol = endSymbol;
      _isLineComment |= _endSymbol.Contains("\n");
      Options |= TermOptions.IsNonGrammar;
    }

    private readonly string _startSymbol;
    private readonly string _endSymbol;
    private readonly bool _isLineComment; 

    #region overrides
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

          return _isLineComment
                   ? TokenAst.Create(this, context, source.TokenStart, source.GetLexeme())
                   : LRParser.CreateSyntaxErrorToken(context, source.TokenStart, "Unclosed comment block");
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
    #endregion
  }
}
