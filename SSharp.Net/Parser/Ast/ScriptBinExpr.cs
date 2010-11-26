namespace Scripting.SSharp.Parser.Ast
{
  using Runtime.Operators;
  using Runtime;
using System;

  /// <summary>
  /// Binary Expression
  /// </summary>
  internal class ScriptBinExpr : ScriptExpr
  {
    protected ScriptExpr left;
    protected ScriptExpr right;
    protected string Oper;
    protected IOperator OperatorFunction;

    public ScriptExpr Left { get { return left; } }
    public ScriptExpr Right { get { return right; } }
    public string Symbol { get { return Oper; } }

    protected internal override bool IsConst {
        get {
            if (Left == null || Right == null) return false;
            return Left.IsConst && Right.IsConst;
        }
    }
    protected object Value { get; set; }
    protected Action<IScriptContext> Evaluation;

    public ScriptBinExpr(AstNodeArgs args)
        : base(args)
    {
      left = (ScriptExpr)ChildNodes[0];
      Oper = ((TokenAst)ChildNodes[1]).Text;
      right = (ScriptExpr)ChildNodes[2];
      
      Value = RuntimeHost.NullValue;
      if (IsConst)
          Evaluation = ConstFirstEvaluate;
      else
          Evaluation = CompleteEvaluate;

      OperatorFunction = RuntimeHost.GetBinaryOperator(Oper);
      if (OperatorFunction == null)
        throw new ScriptRuntimeException(string.Format(Strings.MissingOperatorError, Oper));
    }

    public override void Evaluate(IScriptContext context)
    {
        Evaluation(context);        
    }

    private void ConstFirstEvaluate(IScriptContext context) {
        CompleteEvaluate(context);
        Value = context.Result;
        Evaluation = ConstEvaluate;
    }

    private void ConstEvaluate(IScriptContext context) {
        context.Result = Value;
    }

    private void CompleteEvaluate(IScriptContext context) {
        HandleOperatorArgs handling;

        left.Evaluate(context);
        var leftVal = context.Result;

        context.Result = RuntimeHost.NullValue;

        if ((Oper == "&&" && false.Equals(leftVal)) || (Oper == "||" && true.Equals(leftVal))) {
            handling = OnHandleOperator(this, context, Oper, leftVal);
            context.Result = handling.Cancel ? handling.Result : leftVal;
            return;
        }

        right.Evaluate(context);
        var rightVal = context.Result;

        handling = OnHandleOperator(this, context, Oper, leftVal, rightVal);
        context.Result = handling.Cancel ? handling.Result : OperatorFunction.Evaluate(leftVal, rightVal);
    }
  }
}