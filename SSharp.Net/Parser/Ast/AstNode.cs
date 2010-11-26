namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Base node for representing abstract syntax tree
  /// </summary>
  internal class AstNode
  {
    #region Constructor
    public AstNode(AstNodeArgs args)
    {
      Term = args.Term;
      Span = args.Span;
      if (args.ChildNodes == null || args.ChildNodes.Count == 0) return;

      foreach (AstNode child in args.ChildNodes)
      {
        if (child != null && !child.Term.IsSet(TermOptions.IsPunctuation))
          AddChild(child);
      }
    }
    #endregion

    #region Fields
    private readonly AstNodeList _childNodes = new AstNodeList();
    #endregion

    #region Properties
    public SourceLocation Location
    {
      get { return Span.Start; }
    }

    internal IGrammarTerm Term
    {
      get;
      private set;
    }

    public SourceSpan Span
    {
      get;
      private set;
    }

    public AstNode Parent
    {
      get;
      set;
    }

    public AstNodeList ChildNodes
    {
      get { return _childNodes; }
    }

    #endregion

    #region Methods
    public void AddChild(AstNode child)
    {
      if (child == null) return;
      child.Parent = this;
      ChildNodes.Add(child);
    }
    #endregion

    #region Visitors, Iterators
    public virtual void AcceptVisitor(IAstVisitor visitor)
    {
      visitor.BeginVisit(this);
      if (ChildNodes.Count > 0)
        foreach (AstNode node in ChildNodes)
          node.AcceptVisitor(visitor);
      visitor.EndVisit(this);
    }
    #endregion
  }
}
