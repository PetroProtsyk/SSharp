namespace Scripting.SSharp.Execution.Compilers.Dom
{
  internal class CodeExpressionStatement : CodeStatement
  {
    public CodeExpression Expression { get; private set; }

    public CodeExpressionStatement(CodeExpression expression)
    {
      Expression = expression;
    }
  }
}
