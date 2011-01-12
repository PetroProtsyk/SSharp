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

namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using VM;
  using VM.Operations;

  [CompilerType(typeof(CodeBinaryOperator))]
  internal class CodeBinaryOperatorCompiler : IVMCompiler
  {
    private static readonly Dictionary<OperatorType, string> Mapping = new Dictionary<OperatorType, string>
    {
      {OperatorType.Plus, "+" },
      {OperatorType.Minus,"-" },
      {OperatorType.Mul,"*" },
      {OperatorType.Div,"/" },
      {OperatorType.Mod,"%" },
      {OperatorType.Pow, "^"},

      {OperatorType.Greater,">" },
      {OperatorType.Less,"<" },
      {OperatorType.GreaterEq,">=" },
      {OperatorType.LessEq,"<=" },
      {OperatorType.Eq,"==" },
      {OperatorType.Neq,"!=" }
    };

    #region IVMCompiler Members

    public ExecutableMachine Compile(CodeObject code, ExecutableMachine machine)
    {
      var codeExpression = (CodeBinaryOperator)code;

      CodeDomCompiler.Compile(codeExpression.Right, machine);
      var op = machine.CreateOperation<PushOperation>();
      op.SourceObject = codeExpression.Right;

      CodeDomCompiler.Compile(codeExpression.Left, machine);
      op = machine.CreateOperation<PushOperation>();
      op.SourceObject = codeExpression.Left;

      Operation sop;
      switch (codeExpression.Type)
      {
        case OperatorType.Plus:
          sop = machine.CreateOperation<AddOperation>();
          break;
        case OperatorType.Minus:
          sop = machine.CreateOperation<SubOperation>();
          break;
        case OperatorType.Mul:
          sop = machine.CreateOperation<MulOperation>();
          break;
        case OperatorType.Mod:
          sop = machine.CreateOperation<ModOperation>();
          break;
        case OperatorType.Div:
          sop = machine.CreateOperation<DivOperation>();
          break;
        default:
          var gop = machine.CreateOperation<GenericOperation>();
          gop.Symbol = Mapping[codeExpression.Type];
          sop = gop;
          break;
      }
      sop.SourceObject = codeExpression;

      var pop = machine.CreateOperation<PopOperation>();
      pop.SourceObject = codeExpression;

      return machine;
    }

    #endregion
  }
}
