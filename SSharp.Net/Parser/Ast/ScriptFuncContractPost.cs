using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptFuncContractPost : ScriptExpr
  {
    private readonly ScriptExprList _list;

    public ScriptFuncContractPost(AstNodeArgs args)
      : base(args)
    {
      _list = ChildNodes[1] as ScriptExprList;
    }

    public override void Evaluate(IScriptContext context)
    {
      var result = true;
      if (_list == null)
      {
        context.Result = true;
        return;
      }

      _list.Evaluate(context);

      var rez = (object[])context.Result;
      foreach (var o in rez)
      {
        try
        {
          result = result & (bool)o;
        }
        catch
        {
          throw new ScriptVerificationException(Strings.VerificationNonBoolean);
        }
      }

      context.Result = result;
    }
  }
}
