using System.Collections.Generic;
using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Script Array Constructor Expression
  /// </summary>
  internal class ScriptFunctionCall : ScriptExpr
  {
    private ScriptExprList funcArgs;

    public IEnumerable<ScriptExpr> Parameters
    {
      get
      {
        if (funcArgs == null) return new ScriptExpr[0];
        return funcArgs.List;
      }
    }

    public ScriptFunctionCall(AstNodeArgs args)
      : base(args)
    {
      if (ChildNodes.Count != 0)
        funcArgs = ChildNodes[0] as ScriptExprList;
    }

    public override void Evaluate(IScriptContext context)
    {
      if (funcArgs != null)
      {
        funcArgs.Evaluate(context);
        context.Result = (object[])context.Result;
      }
      else
      {
        context.Result = new object[0];
      }
    }
  }
}
