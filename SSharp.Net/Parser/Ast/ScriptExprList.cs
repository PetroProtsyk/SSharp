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
using System.Linq;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Script Array Expression List Expression
  /// </summary>
  internal class ScriptExprList : ScriptExpr
  {
    private readonly ReadOnlyAstNodeList _exprList;

    public IEnumerable<ScriptExpr> List
    {
      get
      {
        return _exprList.OfType<ScriptExpr>();
      }
    }

    public ScriptExprList(AstNodeArgs args)
        : base(args)
    {
        _exprList = (ChildNodes[0] is ScriptExpr) ? ChildNodes : ChildNodes[0].ChildNodes;
    }

    public override void Evaluate(IScriptContext context)
    {
      var listObjects = new List<object>();
      foreach (ScriptExpr expr in _exprList)
      {
        expr.Evaluate(context);
        listObjects.Add(context.Result);
      }
      context.Result = listObjects.ToArray();
    }
  }
}
