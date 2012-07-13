using Scripting.SSharp.Parser;

namespace Scripting.SSharp.Parser.Ast
{
  using Scripting.SSharp.Runtime.Operators;
  using Scripting.SSharp.Runtime;

  /// <summary>
  /// Binary Expression
  /// </summary>
  internal class ScriptBinExpr : ScriptExpr
  {
    protected ScriptExpr left;
    protected ScriptExpr right;
    protected string oper;
    protected IOperator operatorFunction;

    public ScriptExpr Left { get { return left; } }
    public ScriptExpr Right { get { return right; } }
    public string Symbol { get { return oper; } }

    public ScriptBinExpr(AstNodeArgs args)
        : base(args)
    {
      left = (ScriptExpr)ChildNodes[0];
      oper = ((TokenAst)ChildNodes[1]).Text;
      right = (ScriptExpr)ChildNodes[2];

      operatorFunction = RuntimeHost.GetBinaryOperator(oper);
      if (operatorFunction == null)
        throw new ScriptException("RuntimeHost did not initialize property. Can't find binary operators.");
    }

    public override void Evaluate(IScriptContext context)
    {
      object leftVal, rightVal;
      HandleOperatorArgs handling;

      left.Evaluate(context);
      leftVal = context.Result;

      context.Result = RuntimeHost.NullValue;

      if ((oper == "&&" && false.Equals(leftVal)) || (oper == "||" && true.Equals(leftVal)))
      {
        handling = OnHandleOperator(this, context, oper, leftVal);

        if (handling.Cancel)
          context.Result = handling.Result;
        else
          context.Result = leftVal;
        
        return;
      }

      right.Evaluate(context);
      rightVal = context.Result;

      handling = OnHandleOperator(this, context, oper, leftVal, rightVal);
      if (handling.Cancel)
      {
        context.Result = handling.Result;
      }
      else
      {
        context.Result = operatorFunction.Evaluate(leftVal, rightVal);
          //RuntimeHost.GetBinaryOperator(oper).Evaluate(leftVal, rightVal);
      }
      //Context.EvaluateOperator(oper, leftVal, rightVal);
    }
  }
}