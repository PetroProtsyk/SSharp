using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Unary Expression
  /// </summary>
  internal class ScriptUnaryExpr : ScriptExpr
  {
    private ScriptExpr expr;
    private string oper;

    internal string OperationSymbol { get { return oper; } }

    public ScriptUnaryExpr(AstNodeArgs args)
        : base(args)
    {
      if (ChildNodes[0] is ScriptExpr)
      { 
        expr = (ScriptExpr)ChildNodes[0];
        oper = ((TokenAst)ChildNodes[1]).Text;
      }
      else
      {
        expr = (ScriptExpr)ChildNodes[1];
        oper = ((TokenAst)ChildNodes[0]).Text;
      }
    }

    public override void Evaluate(IScriptContext context)
    {
      expr.Evaluate(context);

      HandleOperatorArgs handler = OnHandleOperator(this, context, oper, context.Result);

      if (handler.Cancel)
        context.Result = handler.Result;
      else
        context.Result = RuntimeHost.GetUnaryOperator(oper).Evaluate(context.Result);
        //Context.EvaluateUnaryOperator(oper, Context.Result);
    }
  }
}
