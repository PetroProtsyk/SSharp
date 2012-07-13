using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptFuncContract : ScriptExpr
  {
    private ScriptFuncContractInv inv;
    private ScriptFuncContractPre pre;
    private ScriptFuncContractPost post;

    public ScriptFuncContract(AstNodeArgs args)
        : base(args)
    {
      pre = ChildNodes[0] as ScriptFuncContractPre;
      post = ChildNodes[1] as ScriptFuncContractPost;
      inv = ChildNodes[2] as ScriptFuncContractInv;
    }

    public override void Evaluate(IScriptContext context)
    {
    }

    protected static bool CheckCondition(ScriptAst cond, IScriptContext context)
    {
      cond.Evaluate(context);
      return (bool)context.Result;
    }

    public void CheckPre(IScriptContext context)
    {
      if (!CheckCondition(pre, context))
      {
        throw new ScriptVerificationException("Pre condition for function call failed");
      }
    }

    public void CheckPost(IScriptContext context)
    {
      if (!CheckCondition(post, context))
      {
        throw new ScriptVerificationException("Post condition for function call failed");
      }
    }

    public void CheckInv(IScriptContext context)
    {
      if (!CheckCondition(inv, context))
      {
        throw new ScriptVerificationException("Invariant for function call failed");
      }
    }

  }
}
