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

using System.Diagnostics;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Script Array Constructor Expression
  /// </summary>
  internal class ScriptGenericsPostfix : ScriptExpr
  {
    private readonly ScriptTypeExprList _token;

    public ScriptGenericsPostfix(AstNodeArgs args)
        : base(args)
    {
      _token = ChildNodes[1] as ScriptTypeExprList;
    }

    public override void Evaluate(IScriptContext context)
    {
      _token.Evaluate(context);
      Debug.Assert(context.Result != null);
    }

    public string GetGenericTypeName(string name)
    {
      return string.Format("{0}`{1}", name, _token.ExprList.Length);
    }
  }
}
