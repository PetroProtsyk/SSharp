using System.Collections.Generic;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Script Array Constructor Expression
  /// </summary>
  internal class ScriptFunctionCall : ScriptExpr
  {
    private readonly ScriptExprList _funcArgs;

    public IEnumerable<ScriptExpr> Parameters
    {
      get { return _funcArgs == null ? new ScriptExpr[0] : _funcArgs.List; }
    }

    public ScriptFunctionCall(AstNodeArgs args)
      : base(args)
    {
      if (ChildNodes.Count != 0)
        _funcArgs = ChildNodes[0] as ScriptExprList;
    }

    public override void Evaluate(IScriptContext context)
    {
      if (_funcArgs != null)
      {
        _funcArgs.Evaluate(context);
        context.Result = context.Result;
      }
      else
      {
        context.Result = new object[0];
      }
    }
  }
}
