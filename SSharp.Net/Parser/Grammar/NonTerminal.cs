using System;
using System.Collections.Generic;

namespace Scripting.SSharp.Parser.FastGrammar
{
  internal class NonTerminal : IGrammarTerm
  {
    #region Constructors
    public NonTerminal(string name, Type nodeType, string key, TermOptions options, int id)
    {
      Name = name;
      NodeType = nodeType;
      Key = key;
      Options = options;
      this.id = id;
    }
    #endregion

    internal int id;
    public Type NodeType { get; private set; }
    public string Key { get; private set; }
    private TermOptions Options { get; set; }

    public bool IsSet(TermOptions option)
    {
      return (Options & option) != 0;
    }

    public string Name
    {
      get;
      private set;
    }

    public string DisplayName
    {
      get;
      private set;
    }
  }
}
