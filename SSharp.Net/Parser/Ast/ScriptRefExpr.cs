using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  //NOTE: ref, out
  /// <summary>
  /// Script Array Constructor Expression
  /// </summary>
  internal class ScriptRefExpr : ScriptExpr
  {
    private readonly string _identifier;
    private readonly string _symbol;

    public string Indentifier { get { return _identifier; } }
    public string Symbol { get { return _symbol; } }

    public ScriptRefExpr(AstNodeArgs args)
      : base(args)
    {
      _symbol = ((TokenAst)ChildNodes[0]).Text;
      _identifier = ((TokenAst)ChildNodes[1]).Text;
    }

    public override void Evaluate(IScriptContext context)
    {
      context.Result = new ScopeValueReference(context.Scope, _identifier);
    }
  }
}