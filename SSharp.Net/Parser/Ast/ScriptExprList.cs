using System.Collections.Generic;
using System.Linq;
using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Script Array Expression List Expression
  /// </summary>
  internal class ScriptExprList : ScriptExpr
  {
    private AstNodeList exprList;

    public IEnumerable<ScriptExpr> List
    {
      get
      {
        return exprList.OfType<ScriptExpr>();
      }
    }

    public ScriptExprList(AstNodeArgs args)
        : base(args)
    {
      exprList = (ChildNodes[0] is ScriptExpr) ? ChildNodes : ChildNodes[0].ChildNodes;
    }

    public override void Evaluate(IScriptContext context)
    {
      List<object> listObjects = new List<object>();
      foreach (ScriptExpr expr in exprList)
      {
        expr.Evaluate(context);
        listObjects.Add(context.Result);
      }
      context.Result = listObjects.ToArray();
    }
  }
}
