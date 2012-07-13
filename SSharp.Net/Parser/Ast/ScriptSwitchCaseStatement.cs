using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Script Array Constructor Expression
  /// </summary>
  internal class ScriptSwitchCaseStatement : ScriptStatement
  {
    private ScriptExpr cond;
    private ScriptStatement statement;

    public ScriptExpr Condition { get { return cond; } }
    public ScriptStatement Statement { get { return statement; } }

    public ScriptSwitchCaseStatement(AstNodeArgs args)
        : base(args)
    {
      cond = ChildNodes[1] as ScriptExpr;
      statement = ChildNodes[3] as ScriptStatement;
    }

    public override void Evaluate(IScriptContext context)
    {
      object switchValue = context.Result;     
      cond.Evaluate(context);
      if (switchValue.Equals(context.Result))
      {
        statement.Evaluate(context);
        context.SetBreak(true);
      }
      else
        context.Result = switchValue;
    }
  }
}
