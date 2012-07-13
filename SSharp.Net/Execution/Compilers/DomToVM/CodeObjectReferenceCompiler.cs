namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using VM;
  using VM.Operations;

  [CompilerType(typeof(CodeObjectReference))]
  public class CodeObjectReferenceCompiler : IVMCompiler
  {
    #region IVMCompiler Members

    public ExecutableMachine Compile(CodeObject code, ExecutableMachine machine)
    {
      CodeObjectReference codeExpression = (CodeObjectReference)code;

      if (codeExpression.Modifiers.Count == 0)
      {
        GetValueOperation op = machine.CreateOperation<GetValueOperation>();
        op.Id = codeExpression.Id;
        machine.CreateOperation<PushOperation>();
      }
      else
      {
        foreach (CodeObject modifier in codeExpression.Modifiers)
        {
          CodeObjectFunctionCall functionCall = modifier as CodeObjectFunctionCall;
          if (functionCall != null)
          {
            CodeDomCompiler.Compile(modifier, machine);

            ObjectMemberOperation callOp = machine.CreateOperation<ObjectMemberOperation>();
            callOp.MemberName = codeExpression.Id;
          }
        }
      }

      if (codeExpression.Next != null)
      {
        CodeDomCompiler.Compile(codeExpression.Next, machine);
      }

      return machine;
   }

    #endregion
  }
}
