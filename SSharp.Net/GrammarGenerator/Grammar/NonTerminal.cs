using System;
using System.Collections.Generic;

namespace Scripting.SSharp.Parser
{
  internal class NonTerminal : GrammarTerm
  {
    #region Constructors
    public NonTerminal(string name)
      : base(name)
    {
      id = counter++;
    }

    internal int id;
    private static int counter;

    public NonTerminal(string name, Type nodeType)
      : this(name)
    {
      base.NodeType = nodeType;
    }

    public NonTerminal(string name, Type nodeType, string key, TermOptions Options, int id)
      : this(name)
    {
      base.NodeType = nodeType;
      base.Key = key;
      base.Options = Options;
      this.id = id;
    }
    #endregion

    #region Properties
    public GrammarExpression Rule
    {
      get;
      set;
    }

    public readonly ProductionList Productions = new ProductionList();
    public readonly StringSet Firsts = new StringSet();
    public readonly NonTerminalList PropagateFirstsTo = new NonTerminalList();
    #endregion
  }
}
