using System.Text.RegularExpressions;
using Scripting.SSharp.Parser.Ast;

namespace Scripting.SSharp.Parser.FastGrammar
{
  internal abstract class RegexBasedTerminal : Terminal
  {
    protected RegexBasedTerminal(string name, string pattern)
      : base(name)
    {
      Expression = new Regex(pattern);
    }

    public Regex Expression
    {
      get;
      private set;
    }

    public override TokenAst TryMatch(CompilerContext context, ISourceStream source)
    {
      Match result = Expression.Match(source.Text, source.Position);
      if (!result.Success)
        return null;
      source.Position += result.Length;
      
      return CreateToken(context, source);
    }

    protected virtual TokenAst CreateToken(CompilerContext context, ISourceStream source)
    {
      string lexeme = source.GetLexeme();
      TokenAst token = TokenAst.Create(this, context, source.TokenStart, lexeme, lexeme);
      return token;
    }
  }
}
