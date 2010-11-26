namespace Scripting.SSharp.Execution.Compilers.Dom
{
  internal class CodeValueReference : CodeExpression
  {
    public object Value { get; private set; }

    public CodeValueReference(object value)
    {
      Value = value;
    }
  }
}
