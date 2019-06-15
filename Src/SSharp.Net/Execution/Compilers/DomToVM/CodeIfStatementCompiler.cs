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

  [CompilerType(typeof(CodeIfStatement))]
  internal class CodeIfStatementCompiler : IVMCompiler
  {
    #region IVMCompiler Members

    public ExecutableMachine Compile(CodeObject code, ExecutableMachine machine)
    {
      // CCCC(indexA)SSSSSS(indexB)EEEEEE(indexC)
      
      var ifStatement = (CodeIfStatement)code;

      CodeDomCompiler.Compile(ifStatement.Condition, machine);
      //Put AX (result of condition evaluation) to BBX
      var exch = machine.CreateOperation<RegisterOperation>();
      exch.Source = MachineRegisters.AX;
      exch.Destination = MachineRegisters.BBX;
      //Jmp To Else
      var jmpToElse = machine.CreateOperation<JmpIfOperation>();
      int eCount = machine.CommandCount;    
      
      //Compile Statement
      CodeDomCompiler.Compile(ifStatement.Statement, machine);
      jmpToElse.Offset = machine.CommandCount - eCount + 1;     
      
      //Compile Else if Any
      if (ifStatement.ElseStatement != null)
      {
        //Jmp To Next
        var jmpToNext = machine.CreateOperation<JmpOperation>();
        //Modify jmpToElse
        jmpToElse.Offset++;

        int sCount = machine.CommandCount;
        CodeDomCompiler.Compile(ifStatement.ElseStatement, machine);
        jmpToNext.Offset = machine.CommandCount - sCount + 1;
      }

      return machine;
    }

    #endregion
  }
}
