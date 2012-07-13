using System;
using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  //NOTE: ref, out
  /// <summary>
  /// Script Array Constructor Expression
  /// </summary>
  internal class ScriptRefExpr : ScriptExpr
  {
    private string identifier;
    private string symbol;

    public string Indentifier { get { return identifier; } }
    public string Symbol { get { return symbol; } }

    public ScriptRefExpr(AstNodeArgs args)
      : base(args)
    {
      symbol = ((TokenAst)ChildNodes[0]).Text;
      identifier = ((TokenAst)ChildNodes[1]).Text;
    }

    public override void Evaluate(IScriptContext context)
    {
      context.Result = new ScopeValueReference(context.Scope, identifier);
    }
  }
}