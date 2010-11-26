namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using VM;
  using VM.Operations;

  [CompilerType(typeof(CodeVariableReference))]
  internal class CodeVariableReferenceCompiler : IVMCompiler
  {
    #region IVMCompiler Members

    public ExecutableMachine Compile(CodeObject code, ExecutableMachine machine)
    {
      var codeExpression = (CodeVariableReference)code;

      var op = machine.CreateOperation<GetValueOperation>();
      op.Id = codeExpression.Id;
      op.SourceObject = codeExpression;

      return machine;
    }

    #endregion
  }
}
