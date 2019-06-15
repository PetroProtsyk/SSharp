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
  //NOTE: ref, out
  /// <summary>
  /// Script Array Constructor Expression
  /// </summary>
  internal class ScriptRefExpr : ScriptExpr
  {
    private readonly string _identifier;
    private readonly string _symbol;

    public string Indentifier { get { return _identifier; } }
    public string Symbol { get { return _symbol; } }

    public ScriptRefExpr(AstNodeArgs args)
      : base(args)
    {
      _symbol = ((TokenAst)ChildNodes[0]).Text;
      _identifier = ((TokenAst)ChildNodes[1]).Text;
    }

    public override void Evaluate(IScriptContext context)
    {
      context.Result = new ScopeValueReference(context.Scope, _identifier);
    }
  }
}