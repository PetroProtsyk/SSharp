namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using VM;
  using VM.Operations;

  [CompilerType(typeof(CodeVariableReference))]
  public class CodeVariableReferenceCompiler : IVMCompiler
  {
    #region IVMCompiler Members

    public ExecutableMachine Compile(CodeObject code, ExecutableMachine machine)
    {
      CodeVariableReference codeExpression = (CodeVariableReference)code;

      GetValueOperation op = machine.CreateOperation<GetValueOperation>();
      op.Id = codeExpression.Id;
      op.SourceObject = codeExpression;

      return machine;
    }

    #endregion
  }
}
