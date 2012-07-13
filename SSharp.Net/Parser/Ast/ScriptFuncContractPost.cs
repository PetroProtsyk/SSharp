using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptFuncContractPost : ScriptExpr
  {
    private ScriptExprList list;

    public ScriptFuncContractPost(AstNodeArgs args)
      : base(args)
    {
      list = ChildNodes[1] as ScriptExprList;
    }

    public override void Evaluate(IScriptContext context)
    {
      bool result = true;
      if (list == null)
      {
        context.Result = true;
        return;
      }

      list.Evaluate(context);

      object[] rez = (object[])context.Result;
      foreach (object o in rez)
      {
        try
        {
          result = result & (bool)o;
        }
        catch
        {
          throw new ScriptException("Non boolean expression in post condition");
        }
      }

      context.Result = result;
    }
  }
}
