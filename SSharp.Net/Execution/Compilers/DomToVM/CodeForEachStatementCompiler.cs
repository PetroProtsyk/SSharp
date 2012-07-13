namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using VM;

  [CompilerType(typeof(CodeForEachStatement))]
  public class CodeForEachStatementCompiler : IVMCompiler
  {
    #region IVMCompiler Members

    private static long sid = 0;

    public ExecutableMachine Compile(CodeObject code, ExecutableMachine machine)
    {
      CodeForEachStatement forStatement = (CodeForEachStatement)code;

      //foreach (i in c) statement ~
      //
      //f_ = c.GetEnumerator();
      //while(f_.Next()) {i = f_.Current; statement; }

      sid++;
      string fName = "#ForEach_" + sid;

      CodeBlockStatement fblock = new CodeBlockStatement();

      CodeAssignExpression f = new CodeAssignExpression(fName, forStatement.Container);
      CodeAssignExpression f1 = new CodeAssignExpression(fName, 
        new CodeObjectReference(fName, 
           new CodeObjectReference("GetEnumerator", null,
            new CodeObject[] { new CodeObjectFunctionCall(new CodeExpression[0]) }), new CodeObject[0]));

      CodeBlockStatement fwhileBlock = new CodeBlockStatement();

      CodeAssignExpression i = new CodeAssignExpression(forStatement.Id.Id,
        new CodeObjectReference(fName,
           new CodeObjectReference("get_Current", null,
            new CodeObject[] { new CodeObjectFunctionCall(new CodeExpression[0]) }), new CodeObject[0]));

      fwhileBlock.Statements.Add(new CodeExpressionStatement(i));
      fwhileBlock.Statements.Add(forStatement.Statement);

      CodeWhileStatement fwhile = new CodeWhileStatement(
        new CodeObjectReference(fName,
           new CodeObjectReference("MoveNext", null,
            new CodeObject[] { new CodeObjectFunctionCall(new CodeExpression[0]) }), new CodeObject[0]),
        fwhileBlock);

      fblock.Statements.Add(new CodeExpressionStatement(f));
      fblock.Statements.Add(new CodeExpressionStatement(f1));
      fblock.Statements.Add(fwhile);

      CodeDomCompiler.Compile(fblock, machine);

      return machine;
    }

    #endregion
  }
}
