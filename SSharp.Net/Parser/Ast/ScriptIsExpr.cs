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

namespace Scripting.SSharp.Parser.Ast
{
  using Runtime.Operators;
  using Runtime;
  using Debug = System.Diagnostics.Debug;

  /// <summary>
  /// Script Array Constructor Expression
  /// </summary>
  internal class ScriptIsExpr : ScriptBinExpr
  {
    private readonly IOperator _operator;

    public ScriptIsExpr(AstNodeArgs args)
      : base(args)
    {
      Debug.Assert(Oper == "is");

      _operator = RuntimeHost.GetBinaryOperator(Oper);
      if (_operator == null)
        throw new ScriptRuntimeException(string.Format(Strings.MissingOperatorError, "is"));
    }

    public override void Evaluate(IScriptContext context)
    {
      left.Evaluate(context);
      var leftVal = context.Result;

      context.Result = RuntimeHost.NullValue;

      var name = (ScriptTypeExpr)right;
      name.Evaluate(context);
      var rightVal = context.Result;

      context.Result = _operator.Evaluate(leftVal, rightVal);
    }
  }
}
