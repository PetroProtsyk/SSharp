using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Script Array Constructor Expression
  /// </summary>
  internal class ScriptCondition : ScriptAst
  {
    private ScriptExpr conditionExpression;

    /// <summary>
    /// Returns condition
    /// </summary>
    public ScriptExpr Expression
    {
      get
      {
        return conditionExpression;
      }
      set
      {
        conditionExpression = value;
      }
    }

    public ScriptCondition(AstNodeArgs args)
        : base(args)
    {
      conditionExpression = (ScriptExpr)ChildNodes[0];
    }

    public override void Evaluate(IScriptContext context)
    {     
      conditionExpression.Evaluate(context);
      
#if DEBUG
      if (!(context.Result is bool))
        throw new ScriptException("Condition expression evaluates non boolean value");      
#endif
    }
  }
}