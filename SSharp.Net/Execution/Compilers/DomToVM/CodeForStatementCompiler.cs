namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using VM;

  [CompilerType(typeof(CodeForStatement))]
  internal class CodeForStatementCompiler : IVMCompiler
  {
    #region IVMCompiler Members

    public ExecutableMachine Compile(CodeObject code, ExecutableMachine machine)
    {
      var forStatement = (CodeForStatement)code;

      //for (init; cond; next) statement ~
      //
      //init;
      //while(cond) { statement; next;}

      CodeDomCompiler.Compile(forStatement.Init, machine);

      var body = new CodeBlockStatement();
      body.Statements.Add(forStatement.Statement);
      body.Statements.Add(new CodeExpressionStatement(forStatement.Next));
      var newWhile = new CodeWhileStatement(forStatement.Condition, body);

      CodeDomCompiler.Compile(newWhile, machine);
      return machine;
    }

    #endregion
  }
}
