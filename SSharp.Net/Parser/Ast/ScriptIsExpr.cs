using System.Diagnostics;

namespace Scripting.SSharp.Parser.Ast
{
  using Runtime.Operators;
  using Runtime;

  /// <summary>
  /// Script Array Constructor Expression
  /// </summary>
  internal class ScriptIsExpr : ScriptBinExpr
  {
    private readonly IOperator _operator;

    public ScriptIsExpr(AstNodeArgs args)
      : base(args)
    {
      Debug.Assert(Oper == "is");

      _operator = RuntimeHost.GetBinaryOperator(Oper);
      if (_operator == null)
        throw new ScriptRuntimeException(string.Format(Strings.MissingOperatorError, "is"));
    }

    public override void Evaluate(IScriptContext context)
    {
      left.Evaluate(context);
      var leftVal = context.Result;

      context.Result = RuntimeHost.NullValue;

      var name = (ScriptTypeExpr)right;
      name.Evaluate(context);
      var rightVal = context.Result;

      context.Result = _operator.Evaluate(leftVal, rightVal);
    }
  }
}
