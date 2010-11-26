namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using VM;
  using VM.Operations;

  [CompilerType(typeof(CodeAssignExpression))]
  internal class CodeAssignExpressionCompiler : IVMCompiler
  {
    #region IVMCompiler Members

    public ExecutableMachine Compile(CodeObject code, ExecutableMachine machine)
    {
      var codeExpression = (CodeAssignExpression)code;
      
      CodeDomCompiler.Compile(codeExpression.RightExpression, machine);

      var op = machine.CreateOperation<SetValueOperation>();
      op.Id = codeExpression.Id;
      op.SourceObject = code;

      return machine;
    }

    #endregion
  }
}
