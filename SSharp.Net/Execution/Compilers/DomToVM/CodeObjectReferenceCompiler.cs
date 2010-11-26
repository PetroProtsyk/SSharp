namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using VM;
  using VM.Operations;

  [CompilerType(typeof(CodeObjectReference))]
  internal class CodeObjectReferenceCompiler : IVMCompiler
  {
    #region IVMCompiler Members

    public ExecutableMachine Compile(CodeObject code, ExecutableMachine machine)
    {
      var codeExpression = (CodeObjectReference)code;

      if (codeExpression.Modifiers.Count == 0)
      {
        var op = machine.CreateOperation<GetValueOperation>();
        op.Id = codeExpression.Id;        
      }
      else
      {
        foreach (var modifier in codeExpression.Modifiers)
        {
          var functionCall = modifier as CodeObjectFunctionCall;
          if (functionCall == null) continue;

          CodeDomCompiler.Compile(modifier, machine);

          var callOp = machine.CreateOperation<ObjectMemberOperation>();
          callOp.MemberName = codeExpression.Id;
        }
      }

      if (codeExpression.Next != null)
      {
        machine.CreateOperation<PushOperation>();
        CodeDomCompiler.Compile(codeExpression.Next, machine);
      }

      return machine;
   }

    #endregion
  }
}
