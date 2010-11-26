using System.Collections.Generic;
using System.Linq;
using Scripting.SSharp.Parser.Ast;

namespace Scripting.SSharp.Parser.FastGrammar
{
  internal sealed class IdentifierTerminal : RegexBasedTerminal
  {
    #region Fields
    private const string AllLatinLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    private const string Pattern = "(?<prefix>global:)?(?<body>[_A-Za-z][_A-Za-z0-9]*)";
    private static readonly List<string> Firsts = new List<string>();
    private readonly HashSet<string> _keywords = new HashSet<string>();
    #endregion

    #region Construction
    public IdentifierTerminal()
      : base("idn", Pattern)
    {
      Firsts.AddRange(AllLatinLetters.ToCharArray().Select(c => c.ToString()));
      Firsts.Add("global:");

      AddKeywords("true", "false", "null", "if", "else",
             "while", "for", "foreach", "in",
             "switch", "case", "default", "break",
             "continue", "return", "function", "is",
             "pre", "post", "invariant", "new", "using",
             "global", "ref", "out", "var");
    }

    private void AddKeywords(params string[] keywords)
    {
      _keywords.AddRange(keywords);
    }
    #endregion

    #region overrides
    protected override TokenAst CreateToken(CompilerContext context, ISourceStream source)
    {
      var token = base.CreateToken(context, source);

      if (_keywords.Contains(token.Text))
        token.IsKeyword = true;

      return token;
    }
    #endregion
  }
}
