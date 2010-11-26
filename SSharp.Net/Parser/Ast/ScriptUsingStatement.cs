using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptUsingStatement : ScriptExpr
  {
    private readonly ScriptQualifiedName _name;
    private readonly ScriptAst _statement;

    public ScriptUsingStatement(AstNodeArgs args)
        : base(args)
    {
      _name = args.ChildNodes[1] as ScriptQualifiedName;
      _statement = args.ChildNodes[2] as ScriptAst;
    }

    public override void Evaluate(IScriptContext context)
    {
      _name.Evaluate(context);
  
      context.CreateScope(RuntimeHost.ScopeFactory.Create(ScopeTypes.Using, context.Scope, context.Result));
        _statement.Evaluate(context);
      context.RemoveLocalScope();
    }
  }
}
