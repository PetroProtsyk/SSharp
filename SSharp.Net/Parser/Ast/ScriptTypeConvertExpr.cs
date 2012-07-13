using System;
using Scripting.SSharp.Parser;

namespace Scripting.SSharp.Parser.Ast
{
  using Scripting.SSharp.Runtime.Operators;
  using Scripting.SSharp.Runtime;
  
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptTypeConvertExpr : ScriptExpr
  {
    private ScriptExpr expr;
    private ScriptExpr typeExpr;
    private IOperator @operator;

    public ScriptExpr Expression
    {
      get
      {
        return expr;
      }
    }

    public ScriptExpr TypeExpression
    {
      get
      {
        return typeExpr;
      }
    }

    public ScriptTypeConvertExpr(AstNodeArgs args)
        : base(args)
    {
      if (ChildNodes.Count == 2)
      {
        if (args.ChildNodes[0] is ScriptExpr &&
            !(args.ChildNodes[1] is ScriptExpr))
        {
          // ( Expr )
          expr = args.ChildNodes[0] as ScriptExpr;
          typeExpr = null;
        }
        else
        {
          //(Type) Expr
          typeExpr = args.ChildNodes[0] as ScriptExpr;
          expr = args.ChildNodes[1] as ScriptExpr;         
        }
      }
      else
      {
        throw new ScriptException("Grammar error!");
      }

      @operator = RuntimeHost.GetBinaryOperator("+");
      if (@operator == null)
        throw new ScriptException("RuntimeHost did not initialize property. Can't find binary operators.");
    }

    public override void Evaluate(IScriptContext context)
    {
      // ( Expr )
      if (typeExpr == null)
      {
        expr.Evaluate(context);
      }
      // (Type) Expr
      else
      {
        typeExpr.Evaluate(context);

        Type type = context.Result as Type;
        if (type == null)
        {
          //NOTE: Handling special case of unary minus operator:
          //      (3+2)-2;
          ScriptUnaryExpr unary = expr as ScriptUnaryExpr;

          if (unary == null || unary.OperationSymbol != "-")
            throw new ScriptException("Wrong type expression!");

          //NOTE: expr + (unary expr)
          object left = context.Result;
          unary.Evaluate(context);
          context.Result = @operator.Evaluate(left, context.Result);
          return;
        }

        expr.Evaluate(context);
        context.Result = RuntimeHost.Binder.ConvertTo(context.Result, type);
      }

    }
  }
}