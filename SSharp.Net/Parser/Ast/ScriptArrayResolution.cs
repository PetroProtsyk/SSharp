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

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptArrayResolution : ScriptAst
  {
    private readonly ScriptExprList _args;
    private static readonly object Empty = new object[0];

    public ScriptArrayResolution(AstNodeArgs args)
        : base(args)
    {
      if (args.ChildNodes.Count != 0)
        _args = args.ChildNodes[0] as ScriptExprList;
    }

    public override void Evaluate(IScriptContext context)
    {
      if (_args == null)
      {
        context.Result = Empty;
        return;
      }
      _args.Evaluate(context);
    }
  }
}