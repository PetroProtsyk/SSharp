namespace Scripting.SSharp.Execution.Compilers.Dom
{
  internal class CodeWhileStatement : CodeStatement
  {
    public CodeExpression Condition
    {
      get;
      private set;
    }

    public CodeStatement Statement
    {
      get;
      private set;
    }

    public CodeWhileStatement(CodeExpression condition, CodeStatement statement)
    {
      Condition = condition;
      Statement = statement;
    }
  }

}
