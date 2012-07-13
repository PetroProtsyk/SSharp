namespace Scripting.SSharp.Execution.Compilers.Dom
{
  using VM;

  public class CodeDomCompiler : BaseCompiler<CodeObject, ExecutableMachine, ExecutableMachine, IVMCompiler>
  {
    public static ExecutableMachine Compile(CodeProgram program)
    {
      ExecutableMachine machine = ExecutableMachine.Create();
      Compile(program, machine);
      return machine; 
    }

    static CodeDomCompiler()
    {
      //Root
      Register<CodeProgramCompiler>();
  
      //Statements
      Register<CodeExpressionStatementCompiler>();
      Register<CodeIfStatementCompiler>();
      Register<CodeWhileStatementCompiler>();
      Register<CodeForStatementCompiler>();
      Register<CodeBlockStatementCompiler>();
      Register<CodeSwitchStatementCompiler>();
      Register<CodeForEachStatementCompiler>();

      Register<CodeReturnStatementCompiler>();
            
      //Expressions
      Register<CodeAssignExpressionCompiler>();
      Register<CodeBinaryOperatorCompiler>();

      //Qualified Name
      Register<CodeObjectReferenceCompiler>();
      Register<CodeObjectFunctionCallCompiler>();

      //Primitive expressions
      Register<CodeValueReferenceCompiler>();
      Register<CodeVariableReferenceCompiler>();
    }
  }
}
