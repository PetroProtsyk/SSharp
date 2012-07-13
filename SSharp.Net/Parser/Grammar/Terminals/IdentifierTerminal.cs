using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

using Scripting.SSharp.Parser.Ast;

namespace Scripting.SSharp.Parser.FastGrammar
{
  internal sealed class IdentifierTerminal : RegexBasedTerminal
  {
    #region Fields
    private const string AllLatinLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    private const string pattern = "(?<prefix>global:)?(?<body>[_A-Za-z][_A-Za-z0-9]*)";
    private static List<string> firsts = new List<string>();
    private readonly StringSet Keywords = new StringSet();
    #endregion

    #region Construction
    public IdentifierTerminal()
      : base("idn", pattern)
    {
      firsts.AddRange(AllLatinLetters.ToCharArray().Select(c => c.ToString()));
      firsts.Add("global:");

      AddKeywords("true", "false", "null", "if", "else",
             "while", "for", "foreach", "in",
             "switch", "case", "default", "break",
             "continue", "return", "function", "is",
             "pre", "post", "invariant", "new", "using",
             "global", "ref", "out", "var");
    }

    private void AddKeywords(params string[] keywords)
    {
      Keywords.AddRange(keywords);
    }
    #endregion

    #region overrides
    protected override TokenAst CreateToken(CompilerContext context, ISourceStream source)
    {
      TokenAst token = base.CreateToken(context, source);

      if (Keywords.Contains(token.Text))
        token.IsKeyword = true;

      return token;
    }
    #endregion
  }
}
