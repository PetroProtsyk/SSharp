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

using System.Collections.Generic;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Script Array Constructor Expression
  /// </summary>
  internal class ScriptFunctionCall : ScriptExpr
  {
    private readonly ScriptExprList _funcArgs;

    public IEnumerable<ScriptExpr> Parameters
    {
      get { return _funcArgs == null ? new ScriptExpr[0] : _funcArgs.List; }
    }

    public ScriptFunctionCall(AstNodeArgs args)
      : base(args)
    {
      if (ChildNodes.Count != 0)
        _funcArgs = ChildNodes[0] as ScriptExprList;
    }

    public override void Evaluate(IScriptContext context)
    {
      if (_funcArgs != null)
      {
        _funcArgs.Evaluate(context);
        context.Result = context.Result;
      }
      else
      {
        context.Result = new object[0];
      }
    }
  }
}
