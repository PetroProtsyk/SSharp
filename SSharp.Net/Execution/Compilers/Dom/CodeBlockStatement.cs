namespace Scripting.SSharp.Execution.Compilers.Dom
{
  internal class CodeBlockStatement : CodeStatement
  {
    public CodeStatements Statements
    {
      get;
      private set;
    }

    public CodeBlockStatement()
    {
      Statements = new CodeStatements();
    }
  }

}
