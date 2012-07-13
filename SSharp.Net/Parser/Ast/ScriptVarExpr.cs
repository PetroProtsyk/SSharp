using System;
using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Variable declaration expression
  /// </summary>
  internal class ScriptVarExpr : ScriptExpr
  {
    private string identifier;
    private string symbol;

    public string Indentifier { get { return identifier; } }
    public string Symbol { get { return symbol; } }

    public ScriptVarExpr(AstNodeArgs args)
      : base(args)
    {
      symbol = ((TokenAst)ChildNodes[0]).Text;
      identifier = ((TokenAst)ChildNodes[1]).Text;
    }

    public override void Evaluate(IScriptContext context)
    {
      LocalScope local = context.Scope as LocalScope;
      if (local != null)
        local.CreateVariable(identifier, null);
      else
        context.SetItem(identifier, null);
    }
  }
}