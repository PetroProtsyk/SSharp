using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime.Promotion;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Script Array Constructor Expression
  /// </summary>
  internal class ScriptNewStmt : ScriptExpr
  {
    ScriptTypeConstructor constrExpr;

    public ScriptNewStmt(AstNodeArgs args)
        : base(args)
    {
      constrExpr = ChildNodes[1] as ScriptTypeConstructor;      
    }

    public override void Evaluate(IScriptContext context)
    {
      constrExpr.Evaluate(context);

      IBinding call = (IBinding)context.Result;
      context.Result = RuntimeHost.Activator.CreateInstance(context, call);
    }
  }
}