namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using VM;
  using VM.Operations;

  [CompilerType(typeof(CodeReturnStatement))]
  public class CodeReturnStatementCompiler : IVMCompiler
  {
    #region IVMCompiler Members

    public ExecutableMachine Compile(CodeObject code, ExecutableMachine machine)
    {
      CodeReturnStatement ret = (CodeReturnStatement)code;

      CodeDomCompiler.Compile(ret.Expression, machine);
      machine.CreateOperation<RetOperation>();

      return machine;
    }

    #endregion
  }
}
