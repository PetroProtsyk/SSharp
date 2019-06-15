/*
 * Copyright © 2011, Petro Protsyk, Denys Vuika
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *  http://www.apache.org/licenses/LICENSE-2.0
 *  
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Parser.Ast;

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
      var code = (String)args[0];
      ScriptAst result;

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

      if (result != null) result.Evaluate(context);
      context.RemoveLocalScope();
      
      return context.Result;
    }

    #endregion
  }
}