namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using VM;

  [CompilerType(typeof(CodeForStatement))]
  public class CodeForStatementCompiler : IVMCompiler
  {
    #region IVMCompiler Members

    public ExecutableMachine Compile(CodeObject code, ExecutableMachine machine)
    {
      CodeForStatement forStatement = (CodeForStatement)code;

      //for (init; cond; next) statement ~
      //
      //init;
      //while(cond) { statement; next;}

      CodeDomCompiler.Compile(forStatement.Init, machine);

        CodeBlockStatement body = new CodeBlockStatement ();
        body.Statements.Add(forStatement.Statement);
        body.Statements.Add(new CodeExpressionStatement(forStatement.Next));
        CodeWhileStatement newWhile = new CodeWhileStatement(forStatement.Condition, body);

      CodeDomCompiler.Compile(newWhile, machine);
      return machine;
    }

    #endregion
  }
}
