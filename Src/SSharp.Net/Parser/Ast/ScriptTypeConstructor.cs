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

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Script Array Constructor Expression
  /// </summary>
  internal class ScriptTypeConstructor : ScriptExpr
  {
    private readonly ScriptTypeExpr _typeExpr;
    private readonly ScriptFunctionCall _callExpr;

    public ScriptTypeConstructor(AstNodeArgs args)
        : base(args)
    {
      _typeExpr = ChildNodes[0] as ScriptTypeExpr;
      _callExpr = ChildNodes[1] as ScriptFunctionCall;
    }

    public override void Evaluate(IScriptContext context)
    {
      _typeExpr.Evaluate(context);
      var type = (Type)context.Result;
      _callExpr.Evaluate(context);
      var arguments = (object[])context.Result;

      context.Result = RuntimeHost.Binder.BindToConstructor(type, arguments);
    }
  }
}


