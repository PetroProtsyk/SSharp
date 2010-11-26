using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Script Array Constructor Expression
  /// </summary>
  internal class ScriptWhileStatement : ScriptExpr
  {
    public ScriptCondition Condition { get; private set; }
    public ScriptStatement Statement { get; private set; }

    public ScriptWhileStatement(AstNodeArgs args)
        : base(args)
    {
      Condition = args.ChildNodes[1] as ScriptCondition;
      Statement = args.ChildNodes[2] as ScriptStatement;
    }

    public override void Evaluate(IScriptContext context)
    {
      Condition.Evaluate(context);
      object lastResult = RuntimeHost.NullValue;

      while ((bool)context.Result)
      {
        Statement.Evaluate(context);
        lastResult = context.Result;

        if (context.IsBreak() || context.IsReturn())
        {
          context.SetBreak(false);
          break;
        }

        if (context.IsContinue())
        {
          context.SetContinue(false);
        }

        Condition.Evaluate(context);
      }

      context.Result = lastResult;
    }
  }
}