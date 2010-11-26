namespace Scripting.SSharp.Execution.Compilers.Dom
{
  internal class CodeBinaryOperator : CodeExpression
  {
    public CodeExpression Left { get; set; }
    public CodeExpression Right { get; set; }
    public OperatorType Type { get; set; }
  } 
}