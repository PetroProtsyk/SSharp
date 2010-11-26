using System.Diagnostics;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Script Array Constructor Expression
  /// </summary>
  internal class ScriptGenericsPostfix : ScriptExpr
  {
    private readonly ScriptTypeExprList _token;

    public ScriptGenericsPostfix(AstNodeArgs args)
        : base(args)
    {
      _token = ChildNodes[1] as ScriptTypeExprList;
    }

    public override void Evaluate(IScriptContext context)
    {
      _token.Evaluate(context);
      Debug.Assert(context.Result != null);
    }

    public string GetGenericTypeName(string name)
    {
      return string.Format("{0}`{1}", name, _token.ExprList.Length);
    }
  }
}
