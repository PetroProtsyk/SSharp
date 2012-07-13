namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using VM;
  using VM.Operations;

  [CompilerType(typeof(CodeObjectFunctionCall))]
  public class CodeObjectFunctionCallCompiler : IVMCompiler
  {
    #region IVMCompiler Members

    public ExecutableMachine Compile(CodeObject code, ExecutableMachine machine)
    {
      CodeObjectFunctionCall call = (CodeObjectFunctionCall)code;

      // AX = function name
      // BX = parameter count

      // stack is full of parameters

      foreach (CodeExpression codeParameter in call.Parameters)
      {
        CodeDomCompiler.Compile(codeParameter, machine);
        PushOperation op = machine.CreateOperation<PushOperation>();
      }
      
      RegisterOperation countOp = machine.CreateOperation<RegisterOperation>();
      countOp.Destination = MachineRegisters.BX;
      countOp.Value = call.Parameters.Count;

      return machine;
   }

    #endregion
  }
}
