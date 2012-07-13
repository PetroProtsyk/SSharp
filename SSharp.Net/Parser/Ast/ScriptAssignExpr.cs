using System.Diagnostics;
using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime.Operators;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Assignment Expression
  /// </summary>
  internal class ScriptAssignExpr : ScriptExpr
  {
    private ScriptQualifiedName nameExpr;
    private ScriptExpr rightExpr;
    private string oper;
    private AssignmentFunction assignOperation;

    public ScriptQualifiedName LeftExpression
    {
      get
      {
        return nameExpr;
      }
    }

    public ScriptExpr RightExpression
    {
      get
      {
        return rightExpr;
      }
    }

    public string Symbol
    {
      get
      {
        return oper;
      }
    }

    private delegate object AssignmentFunction(IScriptContext context);

    public ScriptAssignExpr(AstNodeArgs args)
        : base(args)
    {
      ScriptVarExpr varExpr = args.ChildNodes[0] as ScriptVarExpr;
      if (varExpr != null)
      {
        TokenAst id = varExpr.ChildNodes[1] as TokenAst;
        nameExpr = new ScriptQualifiedName(new AstNodeArgs(args.Term, args.Span,
           new AstNodeList() { id, new AstNode(new AstNodeArgs(args.Term, new SourceSpan(new SourceLocation(0, 0, 0), 0), new AstNodeList())) })) { IsVar = true };
      }
      else
      {
        nameExpr = (ScriptQualifiedName)args.ChildNodes[0];
      }

      oper = ((TokenAst)args.ChildNodes[1]).Text;
      if (args.ChildNodes.Count == 3)
        rightExpr = (ScriptExpr)args.ChildNodes[2];

      Debug.Assert(oper == "=" || oper == ":=" || oper == "+=" || oper == "-=" || oper == "++" || oper == "--" || oper == ":=");

      switch (oper)
      {
        case "=":
          assignOperation = Assign;
          break;
        case ":=":
          assignOperation = AssignEx;
          break;
        case "++":
          assignOperation = PlusPlus;
          break;
        case "--":
          assignOperation = MinusMinus;
          break;
        case "+=":
          assignOperation = PlusEqual;
          break;
        case "-=":
          assignOperation = MinusEqual;
          break;
        default:
          throw new ScriptException("Assignment operator:" + oper + " is not supported");
      }

      minus = RuntimeHost.GetBinaryOperator("-");
      plus = RuntimeHost.GetBinaryOperator("+");

      if (plus == null || minus == null)
        throw new ScriptException("RuntimeHost did not initialize property. Can't find binary operators.");
    }

    public override void Evaluate(IScriptContext context)
    {
      if (rightExpr != null)
      {
        rightExpr.Evaluate(context);
      }
 
      context.Result = assignOperation(context);
    }

    #region Operators
    private IOperator minus;
    private IOperator plus;
    #endregion

    #region Assignments
    private object MinusEqual(IScriptContext context)
    {
      object rez = context.Result;
      nameExpr.Evaluate(context);

      object rezName = context.Result;

      HandleOperatorArgs handling = OnHandleOperator(this, context, "-=", rezName, rez);
      if (handling.Cancel)
        rez = handling.Result;
      else
        rez = minus.Evaluate(rezName, rez);

      //if (!(rezName is EventInfo))
      //{
      //  rez = RuntimeHost.GetBinaryOperator("-").Evaluate(rezName, rez);
      //}
      //else
      //{
      //  rez = new RemoveDelegate((IInvokable)rez);
      //}
      nameExpr.Assign(rez, context);
      return rez;
    }

    private object PlusEqual(IScriptContext context)
    {
      object rez = context.Result;

      nameExpr.Evaluate(context);
      object rezName = context.Result;

      HandleOperatorArgs handling = OnHandleOperator(this, context, "+=", rezName, rez);
      if (handling.Cancel)
        rez = handling.Result;
      else
        rez = plus.Evaluate(rezName, rez);

      //TODO: Events!
      //if (!(rezName is EventInfo))
      //{
      //  rez = RuntimeHost.GetBinaryOperator("+").Evaluate(rezName, rez);
      //}

      nameExpr.Assign(rez, context);
      return rez;
    }

    private object MinusMinus(IScriptContext context)
    {
      object rez = context.Result;

      nameExpr.Evaluate(context);
      rez = minus.Evaluate(context.Result, 1);

      nameExpr.Assign(rez, context);
      return rez;
    }

    private object PlusPlus(IScriptContext context)
    {
      object rez = context.Result;

      nameExpr.Evaluate(context);
      rez = plus.Evaluate(context.Result, 1);

      nameExpr.Assign(rez, context);
      return rez;
    }

    private object AssignEx(IScriptContext context)
    {
      object rez = context.Result;

      nameExpr.Evaluate(context);

      HandleOperatorArgs handling = OnHandleOperator(this, context, ":=", context.Result, rez);
      if (handling.Cancel)
        rez = handling.Result;
      else
      {
        ((ISupportAssign)rez).AssignTo(context.Result);
        rez = context.Result;
      }

      return rez;
    }

    private object Assign(IScriptContext context)
    {
      object rez = context.Result;
      nameExpr.Assign(rez, context);
      return rez;
    }
    #endregion
  }
}
