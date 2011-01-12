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

namespace Scripting.SSharp.Execution.Compilers.Dom
{
  using VM;

  internal class CodeDomCompiler : BaseCompiler<CodeObject, ExecutableMachine, ExecutableMachine, IVMCompiler>
  {
    public static ExecutableMachine Compile(CodeProgram program)
    {
      ExecutableMachine machine = ExecutableMachine.Create();
      Compile(program, machine);
      return machine; 
    }

    static CodeDomCompiler()
    {
      //Root
      Register<CodeProgramCompiler>();
  
      //Statements
      Register<CodeExpressionStatementCompiler>();
      Register<CodeIfStatementCompiler>();
      Register<CodeWhileStatementCompiler>();
      Register<CodeForStatementCompiler>();
      Register<CodeBlockStatementCompiler>();
      Register<CodeSwitchStatementCompiler>();
      Register<CodeForEachStatementCompiler>();

      Register<CodeReturnStatementCompiler>();
            
      //Expressions
      Register<CodeAssignExpressionCompiler>();
      Register<CodeBinaryOperatorCompiler>();

      //Qualified Name
      Register<CodeObjectReferenceCompiler>();
      Register<CodeObjectFunctionCallCompiler>();

      //Primitive expressions
      Register<CodeValueReferenceCompiler>();
      Register<CodeVariableReferenceCompiler>();
    }
  }
}
