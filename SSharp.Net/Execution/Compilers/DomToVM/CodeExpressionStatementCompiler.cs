namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using VM;

  [CompilerType(typeof(CodeExpressionStatement))]
  public class CodeExpressionStatementCompiler : IVMCompiler
  {
    #region IVMCompiler Members

    public ExecutableMachine Compile(CodeObject code, ExecutableMachine machine)
    {
      return CodeDomCompiler.Compile(((CodeExpressionStatement)code).Expression, machine);
    }

    #endregion
  }
}
