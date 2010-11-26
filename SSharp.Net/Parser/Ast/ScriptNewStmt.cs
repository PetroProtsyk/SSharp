using Scripting.SSharp.Runtime.Promotion;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Script Array Constructor Expression
  /// </summary>
  internal class ScriptNewStmt : ScriptExpr
  {
    private readonly ScriptTypeConstructor _constrExpr;

    public ScriptNewStmt(AstNodeArgs args)
        : base(args)
    {
      _constrExpr = ChildNodes[1] as ScriptTypeConstructor;      
    }

    public override void Evaluate(IScriptContext context)
    {
      _constrExpr.Evaluate(context);

      var call = (IBinding)context.Result;
      context.Result = RuntimeHost.Activator.CreateInstance(context, call);
    }
  }
}