namespace Scripting.SSharp.Execution.Compilers.Dom
{
  internal class CodeReturnStatement : CodeStatement
  {
    public CodeExpression Expression
    {
      get;
      private set;
    }

    public CodeReturnStatement(CodeExpression expression)
    {
      Expression = expression;
    }
  }

}
