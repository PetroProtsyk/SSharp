using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Script Array Constructor Expression
  /// </summary>
  internal class ScriptCondition : ScriptAst
  {
    private ScriptExpr _conditionExpression;

    /// <summary>
    /// Returns condition
    /// </summary>
    public ScriptExpr Expression
    {
      get
      {
        return _conditionExpression;
      }
      set
      {
        _conditionExpression = value;
      }
    }

    public ScriptCondition(AstNodeArgs args)
        : base(args)
    {
      _conditionExpression = (ScriptExpr)ChildNodes[0];
    }

    public override void Evaluate(IScriptContext context)
    {     
      _conditionExpression.Evaluate(context);
      
#if DEBUG
      if (!(context.Result is bool))
        throw new ScriptVerificationException(Strings.VerificationNonBoolean);      
#endif
    }
  }
}