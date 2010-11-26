using System.Collections.Generic;
using System.Linq;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Script Array Expression List Expression
  /// </summary>
  internal class ScriptExprList : ScriptExpr
  {
    private readonly AstNodeList _exprList;

    public IEnumerable<ScriptExpr> List
    {
      get
      {
        return _exprList.OfType<ScriptExpr>();
      }
    }

    public ScriptExprList(AstNodeArgs args)
        : base(args)
    {
      _exprList = (ChildNodes[0] is ScriptExpr) ? ChildNodes : ChildNodes[0].ChildNodes;
    }

    public override void Evaluate(IScriptContext context)
    {
      var listObjects = new List<object>();
      foreach (ScriptExpr expr in _exprList)
      {
        expr.Evaluate(context);
        listObjects.Add(context.Result);
      }
      context.Result = listObjects.ToArray();
    }
  }
}
