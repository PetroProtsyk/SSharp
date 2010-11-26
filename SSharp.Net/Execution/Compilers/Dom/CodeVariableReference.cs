namespace Scripting.SSharp.Execution.Compilers.Dom
{
  internal class CodeVariableReference : CodeExpression
  {
    public string Id { get; private set; }

    public CodeVariableReference(string id)
    {
      Id = id;
    }
  }
}
