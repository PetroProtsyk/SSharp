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
  using Debug = System.Diagnostics.Debug;

  /// <summary>
  /// Script Array Constructor Expression
  /// </summary>
  internal class ScriptFlowControlStatement : ScriptStatement
  {
    private readonly string _operation;
    private readonly ScriptAst _expression;

    public string Symbol
    {
      get
      {
        return _operation;
      }
    }

    public ScriptAst Expression
    {
      get
      {
        return _expression;
      }
    }

    public ScriptFlowControlStatement(AstNodeArgs args)
        : base(args)
    {
      var oper = ChildNodes[0] as TokenAst;
      _operation = oper.Text;
      Debug.Assert(oper.Text == "return" || oper.Text == "break" || oper.Text == "continue" || oper.Text == "throw");

      if (_operation == "return" || _operation == "throw")
        _expression = (ScriptExpr)ChildNodes[1];
    }

    //TODO: reorganize switch
    public override void Evaluate(IScriptContext context)
    {
      switch (_operation)
      {
        case "break":
          if (context.Result == null)
            context.Result = RuntimeHost.NullValue;
          context.SetBreak(true);
          break;
        case "continue":
          if (context.Result == null)
            context.Result = RuntimeHost.NullValue;
          context.SetContinue(true);
          break;
        case "return":
          _expression.Evaluate(context);
          context.SetReturn(true);
          break;
        case "throw":
          _expression.Evaluate(context);
          throw (Exception)context.Result;
        default:
          throw new ScriptSyntaxErrorException(_operation);
      }
    }
  }
}