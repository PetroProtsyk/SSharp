using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptUsingStatement : ScriptExpr
  {
    private ScriptQualifiedName name;
    private ScriptAst statement;

    public ScriptUsingStatement(AstNodeArgs args)
        : base(args)
    {
      name = args.ChildNodes[1] as ScriptQualifiedName;
      statement = args.ChildNodes[2] as ScriptAst;
    }

    public override void Evaluate(IScriptContext context)
    {
      name.Evaluate(context);
  
      context.CreateScope(RuntimeHost.ScopeFactory.Create(ScopeTypes.Using, context.Scope, context.Result));
        statement.Evaluate(context);
      context.RemoveLocalScope();
    }
  }
}
