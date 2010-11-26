using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Base class for Script.NET Ast's nodes
  /// </summary>
  internal class ScriptAst : AstNode
  {
    /// <summary>
    /// Base constructor
    /// </summary>
    /// <param name="args">AstNodeList</param>
    public ScriptAst(AstNodeArgs args)
      : base(args)
    {

    }

    /// <summary>
    /// Returns Source code for given AST
    /// </summary>
    /// <returns></returns>
    internal string Code(IScriptContext context)
    {
      return context.Owner.SourceCode.Substring(Span.Start.Position, Span.Length);
    }

    /// <summary>
    /// Returns string representing concrete syntax tree
    /// </summary>
    /// <returns></returns>
    internal string ConcreteSyntaxTree()
    {
      return ConcreteSyntaxTree("");
    }

    private string ConcreteSyntaxTree(string inted)
    {
      string tree = Term.Name + "\r\n";
      inted += " ";
      foreach (var node in ChildNodes)
      {
        var scriptNode = node as ScriptAst;
        if (scriptNode != null)
          tree += inted + scriptNode.ConcreteSyntaxTree(inted);
        else
        {
          if (!string.IsNullOrEmpty(node.Term.DisplayName))
            tree += inted + node +"\r\n";
        }
      }
      return tree;
    }
    
    //TODO: Move To ScriptProg   
    /// <summary>
    /// Evaluates all child nodes
    /// </summary>
    /// <param name="context">ScriptContext object</param>
    /// <returns>result of the last node evaluation</returns>
    public object Execute(IScriptContext context)
    {
      Evaluate(context);
      return context.Result;
    }

    /// <summary>
    /// Evaluates script
    /// </summary>
    /// <param name="context">ScriptContext</param>
    public virtual void Evaluate(IScriptContext context)
    {
      if (ChildNodes.Count <= 0) return;

      int index = 0;
      while (index < ChildNodes.Count)
      {
        var node = ChildNodes[index] as ScriptAst;
        if (node != null)
          node.Evaluate(context);
        index++;
      }
    }
  }
}