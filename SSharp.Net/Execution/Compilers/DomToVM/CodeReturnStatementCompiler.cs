namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using VM;
  using VM.Operations;

  [CompilerType(typeof(CodeReturnStatement))]
  internal class CodeReturnStatementCompiler : IVMCompiler
  {
    #region IVMCompiler Members

    public ExecutableMachine Compile(CodeObject code, ExecutableMachine machine)
    {
      var ret = (CodeReturnStatement)code;

      CodeDomCompiler.Compile(ret.Expression, machine);
      machine.CreateOperation<RetOperation>();

      return machine;
    }

    #endregion
  }
}
