using System;
using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Parser.Ast;
using Scripting.SSharp.Parser.FastGrammar;

namespace Scripting.SSharp.CustomFunctions
{
  internal class EvalFunc : IInvokable
  {
    public static EvalFunc FunctionDefinition = new EvalFunc();
    public static string FunctionName = "eval";

    private EvalFunc()
    {
    }

    #region IInvokable Members

    public bool CanInvoke()
    {
      return true;
    }

    public object Invoke(IScriptContext context, object[] args)
    {      
      string code = (String)args[0];
      ScriptAst result = null;

      LRParser compiler = (LRParser)context.GetItem("Compiler", true);
      RuntimeHost.Lock();
      
      try
      {
        result = Script.Parse(code + ";", false) as ScriptAst;
        //TODO: Create LocalOnlyScope that can't change Parent's variables
        //No, need for doing these. It is already done
        context.CreateScope();
      }
      finally
      {
        RuntimeHost.UnLock();
      }

      result.Evaluate(context);
      context.RemoveLocalScope();
      
      return context.Result;
    }

    #endregion
  }
}