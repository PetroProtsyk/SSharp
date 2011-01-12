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
