using System;
using System.Collections.Generic;
using System.Globalization;
using Scripting.SSharp.Parser.Ast;

namespace Scripting.SSharp.Parser
{
  internal interface IGrammarTerm
  {
    string Name { get; }
    string DisplayName { get; }
    Type NodeType { get; }

    bool IsSet(TermOptions termOptions);
  }

  internal interface ITerminal : IGrammarTerm
  {
    string Key { get; }
    TokenCategory Category { get; }
    TokenMatchMode MatchMode { get; }
    int Precedence { get; }
    Associativity Associativity { get; }
    int Priority { get; }
    TokenAst TryMatch(CompilerContext context, ISourceStream source);    
  }

  internal class AstNodeList : List<AstNode> { }

  internal class StringDictionary : Dictionary<string, string> { }
  internal class CharList : List<char> { }
  internal class TokenList : List<TokenAst> { }
  internal class TerminalList : List<ITerminal> { }
  internal class SyntaxErrorList : List<SyntaxError> { }
  internal class EscapeTable : Dictionary<char, char> { }
  internal class TerminalLookupTable : Dictionary<char, TerminalList> { }
  internal class UnicodeCategoryList : List<UnicodeCategory> { }

  internal class StringList : List<string>
  {
    public StringList() { }
    
    public StringList(params string[] args)
    {
      AddRange(args);
    }

    public new void AddRange(IEnumerable<string> keys)
    {
      foreach (string key in keys)
        this.Add(key);
    }
   
    public override string ToString()
    {
      return ToString(" ");
    }
    
    public string ToString(string separator)
    {
      return String.Join(separator, ToArray());
    }

    public static int LongerFirst(string x, string y)
    {
      try
      {
        if (x.Length > y.Length) return -1;
      }
      catch { }
      return 0;
    }

  }

}
