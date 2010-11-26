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
