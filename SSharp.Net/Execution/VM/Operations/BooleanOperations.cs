namespace Scripting.SSharp.Execution.VM.Operations
{
  using Runtime;

  internal class CmpOperation : Operation
  {
    public string Id { get; set; }

    public override int Execute(IScriptContext context)
    {
      Machine.BBX =
        (bool)RuntimeHost.GetBinaryOperator(">").Evaluate(Machine.AX, Machine.BX);
      return 1;
    }
  }
}
