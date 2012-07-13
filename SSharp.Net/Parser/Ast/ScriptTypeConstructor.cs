using System;
using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Script Array Constructor Expression
  /// </summary>
  internal class ScriptTypeConstructor : ScriptExpr
  {
    private ScriptTypeExpr typeExpr;
    private ScriptFunctionCall callExpr;

    public ScriptTypeConstructor(AstNodeArgs args)
        : base(args)
    {
      typeExpr = ChildNodes[0] as ScriptTypeExpr;
      callExpr = ChildNodes[1] as ScriptFunctionCall;
    }

    public override void Evaluate(IScriptContext context)
    {
      typeExpr.Evaluate(context);
      Type type = (Type)context.Result;
      callExpr.Evaluate(context);
      object[] arguments = (object[])context.Result;

      context.Result = RuntimeHost.Binder.BindToConstructor(type, arguments);
    }
  }
}


