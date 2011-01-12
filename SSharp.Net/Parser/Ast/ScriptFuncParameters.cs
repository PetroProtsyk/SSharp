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
  /// 
  /// </summary>
  internal class ScriptFuncParameters : ScriptExpr
  {
    internal readonly List<string> Identifiers = new List<string>();

    public ScriptFuncParameters(AstNodeArgs args)
      : base(args)
    {
      if (ChildNodes.Count != 1) return;

      foreach (var astNode in ChildNodes[0].ChildNodes)
      {
        Identifiers.Add(((TokenAst) astNode).Text);
      }
    }

    public override void Evaluate(IScriptContext context)
    {
      if (context.Result == null) return;

      var paramVals = (object[])context.Result;

      for (var index=0; index < paramVals.Length; index++)
        if (index < Identifiers.Count)
        {
          context.SetItem(Identifiers[index], paramVals[index]);
        }

      context.Result = RuntimeHost.NullValue;
    }
  }
}