using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Unary Expression
  /// </summary>
  internal class ScriptUnaryExpr : ScriptExpr
  {
    private readonly ScriptExpr _expr;

    internal string OperationSymbol { get; private set; }

    public ScriptUnaryExpr(AstNodeArgs args)
        : base(args)
    {
      if (ChildNodes[0] is ScriptExpr)
      { 
        _expr = (ScriptExpr)ChildNodes[0];
        OperationSymbol = ((TokenAst)ChildNodes[1]).Text;
      }
      else
      {
        _expr = (ScriptExpr)ChildNodes[1];
        OperationSymbol = ((TokenAst)ChildNodes[0]).Text;
      }
    }

    public override void Evaluate(IScriptContext context)
    {
      _expr.Evaluate(context);
      var handler = OnHandleOperator(this, context, OperationSymbol, context.Result);
      context.Result = handler.Cancel ? handler.Result : RuntimeHost.GetUnaryOperator(OperationSymbol).Evaluate(context.Result);
    }
  }
}
