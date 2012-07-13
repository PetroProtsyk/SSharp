namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using VM;
  using VM.Operations;

  [CompilerType(typeof(CodeAssignExpression))]
  public class CodeAssignExpressionCompiler : IVMCompiler
  {
    #region IVMCompiler Members

    public ExecutableMachine Compile(CodeObject code, ExecutableMachine machine)
    {
      CodeAssignExpression codeExpression = (CodeAssignExpression)code;
      
      CodeDomCompiler.Compile(codeExpression.RightExpression, machine);

      SetValueOperation op = machine.CreateOperation<SetValueOperation>();
      op.Id = codeExpression.Id;
      op.SourceObject = code;

      return machine;
    }

    #endregion
  }
}
