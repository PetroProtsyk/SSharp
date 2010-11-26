using System;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Script Array Constructor Expression
  /// </summary>
  internal class ScriptTypeConstructor : ScriptExpr
  {
    private readonly ScriptTypeExpr _typeExpr;
    private readonly ScriptFunctionCall _callExpr;

    public ScriptTypeConstructor(AstNodeArgs args)
        : base(args)
    {
      _typeExpr = ChildNodes[0] as ScriptTypeExpr;
      _callExpr = ChildNodes[1] as ScriptFunctionCall;
    }

    public override void Evaluate(IScriptContext context)
    {
      _typeExpr.Evaluate(context);
      var type = (Type)context.Result;
      _callExpr.Evaluate(context);
      var arguments = (object[])context.Result;

      context.Result = RuntimeHost.Binder.BindToConstructor(type, arguments);
    }
  }
}


