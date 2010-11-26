using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Variable declaration expression
  /// </summary>
  internal class ScriptVarExpr : ScriptExpr
  {
    private readonly string _identifier;
    private readonly string _symbol;

    public string Indentifier { get { return _identifier; } }
    public string Symbol { get { return _symbol; } }

    public ScriptVarExpr(AstNodeArgs args)
      : base(args)
    {
      _symbol = ((TokenAst)ChildNodes[0]).Text;
      _identifier = ((TokenAst)ChildNodes[1]).Text;
    }

    public override void Evaluate(IScriptContext context)
    {
      var local = context.Scope as LocalScope;
      if (local != null)
        local.CreateVariable(_identifier, null);
      else
        context.SetItem(_identifier, null);
    }
  }
}