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

namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using VM;
  using VM.Operations;

  [CompilerType(typeof(CodeProgram))]
  internal class CodeProgramCompiler : IVMCompiler
  {
    #region IVMCompiler Members

    public ExecutableMachine Compile(CodeObject code, ExecutableMachine machine)
    {
      var program = (CodeProgram)code;

      foreach (var statement in program.Statements)
        CodeDomCompiler.Compile(statement, machine);
      
      machine.CreateOperation<RetOperation>();

      //TODO: Functions
      //foreach (CodeObject function in program.Functions)
      //  Compile(function, machine);

      return machine;
    }

    #endregion
  }
}
