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
  /// Unary Expression
  /// </summary>
  internal class ScriptUnaryExpr : ScriptExpr
  {
    private readonly ScriptExpr _expr;

    internal string OperationSymbol { get; private set; }

    public ScriptUnaryExpr(AstNodeArgs args)
        : base(args)
    {
      if (ChildNodes[0] is ScriptExpr)
      { 
        _expr = (ScriptExpr)ChildNodes[0];
        OperationSymbol = ((TokenAst)ChildNodes[1]).Text;
      }
      else
      {
        _expr = (ScriptExpr)ChildNodes[1];
        OperationSymbol = ((TokenAst)ChildNodes[0]).Text;
      }
    }

    public override void Evaluate(IScriptContext context)
    {
      _expr.Evaluate(context);
      var handler = OnHandleOperator(this, context, OperationSymbol, context.Result);
      context.Result = handler.Cancel ? handler.Result : RuntimeHost.GetUnaryOperator(OperationSymbol).Evaluate(context.Result);
    }
  }
}
