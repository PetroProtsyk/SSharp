namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using VM;
  using VM.Operations;

  [CompilerType(typeof(CodeValueReference))]
  internal class CodeValueReferenceCompiler : IVMCompiler
  {
    #region IVMCompiler Members

    public ExecutableMachine Compile(CodeObject code, ExecutableMachine machine)
    {
      var op = machine.CreateOperation<ValueOperation>();
      op.Value = ((CodeValueReference)code).Value;
      op.SourceObject = code;

      return machine;
   }

    #endregion
  }
}
