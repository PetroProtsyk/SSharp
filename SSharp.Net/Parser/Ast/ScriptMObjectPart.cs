using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{  
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptMObjectPart : ScriptExpr
  {
    private readonly string _name;
    private readonly ScriptExpr _expr;

    public ScriptMObjectPart(AstNodeArgs args)
        : base(args)
    {
      _name = ((TokenAst) ChildNodes[0]).Text;
      _expr = ChildNodes[2] as ScriptExpr;
    }

    public override void Evaluate(IScriptContext context)
    {
      _expr.Evaluate(context);
      context.Result = new[] { _name, context.Result };
    }
  }
}