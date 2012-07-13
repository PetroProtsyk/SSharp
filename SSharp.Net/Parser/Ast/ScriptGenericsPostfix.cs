using System;
using System.Diagnostics;
using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Script Array Constructor Expression
  /// </summary>
  internal class ScriptGenericsPostfix : ScriptExpr
  {
    private ScriptTypeExprList token;

    public ScriptGenericsPostfix(AstNodeArgs args)
        : base(args)
    {
      token = ChildNodes[1] as ScriptTypeExprList;
    }

    public override void Evaluate(IScriptContext context)
    {
      token.Evaluate(context);
      Debug.Assert(context.Result != null);
    }

    public string GetGenericTypeName(string name)
    {
      return string.Format("{0}`{1}", name, token.exprList.Length);
    }
  }
}
