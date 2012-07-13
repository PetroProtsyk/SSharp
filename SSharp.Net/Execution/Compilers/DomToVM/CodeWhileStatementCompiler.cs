namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using VM;
  using VM.Operations;

  [CompilerType(typeof(CodeWhileStatement))]
  public class CodeWhileStatementCompiler : IVMCompiler
  {
    #region IVMCompiler Members

    public ExecutableMachine Compile(CodeObject code, ExecutableMachine machine)
    {
      // CCCC(indexA)SSSSSS(indexC)
      CodeWhileStatement whileStatement = (CodeWhileStatement)code;

      int cCount = machine.CommandCount;

      CodeDomCompiler.Compile(whileStatement.Condition, machine);
      //Put AX (result of condition evaluation) to BBX
      RegisterOperation exch = machine.CreateOperation<RegisterOperation>();
      exch.Source = MachineRegisters.AX;
      exch.Destination = MachineRegisters.BBX;
      //Jmp To Else
      JmpIfOperation jmpToNext = machine.CreateOperation<JmpIfOperation>();
      int eCount = machine.CommandCount;    
      
      //Compile Statement
      CodeDomCompiler.Compile(whileStatement.Statement, machine);

      JmpIfOperation jmpToCondition = machine.CreateOperation<JmpIfOperation>();
      jmpToCondition.Offset = cCount - machine.CommandCount + 1;

      jmpToNext.Offset = machine.CommandCount - eCount + 1;
      
      return machine;
    }

    #endregion
  }
}
