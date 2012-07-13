using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Base class for Script.NET Ast's nodes
  /// </summary>
  public class ScriptAst : AstNode
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
    public string Code()
    {
      return "Unimplemented yet"; 
    }

    /// <summary>
    /// Returns string representing concrete syntax tree
    /// </summary>
    /// <returns></returns>
    public string ConcreteSyntaxTree()
    {
      return ConcreteSyntaxTree("");
    }

    private string ConcreteSyntaxTree(string inted)
    {
      string tree = Term.Name + "\r\n";
      inted += " ";
      foreach (AstNode node in ChildNodes)
      {
        ScriptAst scriptNode = node as ScriptAst;
        if (scriptNode != null)
          tree += inted + scriptNode.ConcreteSyntaxTree(inted);
        else
        {
          if (!string.IsNullOrEmpty(node.Term.DisplayName))
            tree += inted + node.ToString() +"\r\n";
        }
      }
      return tree;
    }
    
    //TODO: Move To ScriptProg   
    /// <summary>
    /// Evaluates all child nodes
    /// </summary>
    /// <param name="Context">ScriptContext object</param>
    /// <returns>result of the last node evaluation</returns>
    public object Execute(IScriptContext context)
    {
      Evaluate(context);
      return context.Result;
    }

    /// <summary>
    /// Evaluates script
    /// </summary>
    /// <param name="Context">ScriptContext</param>
    public virtual void Evaluate(IScriptContext context)
    {
      if (ChildNodes.Count > 0)
      {
        int index = 0;
        while (index < ChildNodes.Count)
        {
            ScriptAst node = ChildNodes[index] as ScriptAst;
            if (node != null)
                node.Evaluate(context);
            index++;
        }
      }
    }
  }
}