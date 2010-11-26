using System;
using System.Diagnostics;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Script Array Constructor Expression
  /// </summary>
  internal class ScriptFlowControlStatement : ScriptStatement
  {
    private readonly string _operation;
    private readonly ScriptAst _expression;

    public string Symbol
    {
      get
      {
        return _operation;
      }
    }

    public ScriptAst Expression
    {
      get
      {
        return _expression;
      }
    }

    public ScriptFlowControlStatement(AstNodeArgs args)
        : base(args)
    {
      var oper = ChildNodes[0] as TokenAst;
      _operation = oper.Text;
      Debug.Assert(oper.Text == "return" || oper.Text == "break" || oper.Text == "continue" || oper.Text == "throw");

      if (_operation == "return" || _operation == "throw")
        _expression = (ScriptExpr)ChildNodes[1];
    }

    //TODO: reorganize switch
    public override void Evaluate(IScriptContext context)
    {
      switch (_operation)
      {
        case "break":
          if (context.Result == null)
            context.Result = RuntimeHost.NullValue;
          context.SetBreak(true);
          break;
        case "continue":
          if (context.Result == null)
            context.Result = RuntimeHost.NullValue;
          context.SetContinue(true);
          break;
        case "return":
          _expression.Evaluate(context);
          context.SetReturn(true);
          break;
        case "throw":
          _expression.Evaluate(context);
          throw (Exception)context.Result;
        default:
          throw new ScriptSyntaxErrorException(_operation);
      }
    }
  }
}