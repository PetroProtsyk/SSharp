using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Script Array Constructor Expression
  /// </summary>
  internal class ScriptSwitchCaseStatement : ScriptStatement
  {
    private readonly ScriptExpr _cond;
    private readonly ScriptStatement _statement;

    public ScriptExpr Condition { get { return _cond; } }
    public ScriptStatement Statement { get { return _statement; } }

    public ScriptSwitchCaseStatement(AstNodeArgs args)
        : base(args)
    {
      _cond = ChildNodes[1] as ScriptExpr;
      _statement = ChildNodes[3] as ScriptStatement;
    }

    public override void Evaluate(IScriptContext context)
    {
      var switchValue = context.Result;     
      _cond.Evaluate(context);
      if (switchValue.Equals(context.Result))
      {
        _statement.Evaluate(context);
        context.SetBreak(true);
      }
      else
        context.Result = switchValue;
    }
  }
}
