namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using VM;
  using VM.Operations;

  [CompilerType(typeof(CodeObjectFunctionCall))]
  internal class CodeObjectFunctionCallCompiler : IVMCompiler
  {
    #region IVMCompiler Members

    public ExecutableMachine Compile(CodeObject code, ExecutableMachine machine)
    {
      var call = (CodeObjectFunctionCall)code;

      // AX = function name
      // BX = parameter count

      // stack is full of parameters

      foreach (var codeParameter in call.Parameters)
      {
        CodeDomCompiler.Compile(codeParameter, machine);
        var op = machine.CreateOperation<PushOperation>();
      }
      
      var countOp = machine.CreateOperation<RegisterOperation>();
      countOp.Destination = MachineRegisters.BX;
      countOp.Value = call.Parameters.Count;

      return machine;
   }

    #endregion
  }
}
