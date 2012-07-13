using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Parser.Ast;

namespace Scripting.SSharp.CustomFunctions
{
  internal class ReplaceFunc : IInvokable
  {
    public static ReplaceFunc FunctionDefinition = new ReplaceFunc();
    public static string FunctionName = "ReplaceAst";

    private ReplaceFunc()
    {
    }

    #region IInvokable Members

    public bool CanInvoke()
    {
      return true;
    }

    public object Invoke(IScriptContext context, object[] args)
    {
      AstNode node = (AstNode)args[0];
      if (args[0] is ScriptMetaExpr)
        node = ((ScriptMetaExpr)node).ChildNodes[1];

      //Get Prog
      AstNode prog = node.Parent;
      while (prog != null && !(prog is ScriptQualifiedName) && ! (prog.ChildNodes[0] is TokenAst && ((TokenAst)prog.ChildNodes[0]).Text == ReplaceFunc.FunctionName))
        prog = prog.Parent;
      
      prog.Parent.ChildNodes.Add(node);
      return node;
    }

    #endregion
  }
}