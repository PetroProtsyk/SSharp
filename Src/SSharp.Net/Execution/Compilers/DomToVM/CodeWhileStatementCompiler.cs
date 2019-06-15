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

  [CompilerType(typeof(CodeWhileStatement))]
  internal class CodeWhileStatementCompiler : IVMCompiler
  {
    #region IVMCompiler Members

    public ExecutableMachine Compile(CodeObject code, ExecutableMachine machine)
    {
      // CCCC(indexA)SSSSSS(indexC)
      var whileStatement = (CodeWhileStatement)code;

      int cCount = machine.CommandCount;

      CodeDomCompiler.Compile(whileStatement.Condition, machine);
      //Put AX (result of condition evaluation) to BBX
      var exch = machine.CreateOperation<RegisterOperation>();
      exch.Source = MachineRegisters.AX;
      exch.Destination = MachineRegisters.BBX;
      //Jmp To Else
      var jmpToNext = machine.CreateOperation<JmpIfOperation>();
      int eCount = machine.CommandCount;    
      
      //Compile Statement
      CodeDomCompiler.Compile(whileStatement.Statement, machine);

      var jmpToCondition = machine.CreateOperation<JmpIfOperation>();
      jmpToCondition.Offset = cCount - machine.CommandCount + 1;

      jmpToNext.Offset = machine.CommandCount - eCount + 1;
      
      return machine;
    }

    #endregion
  }
}
