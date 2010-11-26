using Scripting.SSharp.Runtime;
using System;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptFuncContract : ScriptExpr
  {
    private readonly ScriptFuncContractInv _inv;
    private readonly ScriptFuncContractPre _pre;
    private readonly ScriptFuncContractPost _post;
    internal ScriptFunctionDefinition _function;

    public ScriptFuncContract(AstNodeArgs args)
        : base(args)
    {
      _pre = ChildNodes[0] as ScriptFuncContractPre;
      _post = ChildNodes[1] as ScriptFuncContractPost;
      _inv = ChildNodes[2] as ScriptFuncContractInv;
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
      if (_function == null) throw new NullReferenceException("Function");

      if (!CheckCondition(_pre, context))
      {
        throw new ScriptVerificationException(string.Format(Strings.VerificationPreCondition, _function.Name, Code(context)));
      }
    }

    public void CheckPost(IScriptContext context)
    {
      if (_function == null) throw new NullReferenceException("Function");

      if (!CheckCondition(_post, context))
      {
        throw new ScriptVerificationException(string.Format(Strings.VerificationPostCondition, _function.Name, Code(context)));
      }
    }

    public void CheckInv(IScriptContext context)
    {
      if (_function == null) throw new NullReferenceException("Function");

      if (!CheckCondition(_inv, context))
      {
        throw new ScriptVerificationException(string.Format(Strings.VerificationInvariantCondition, _function.Name, Code(context)));
      }
    }
  }
}
