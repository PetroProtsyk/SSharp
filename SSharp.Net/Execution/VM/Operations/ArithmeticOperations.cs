namespace Scripting.SSharp.Execution.VM.Operations
{
  using Runtime;

  internal class IncOperation : Operation
  {
    public string Id { get; set; }

    public override int Execute(IScriptContext context)
    {
      Machine.AX =
        RuntimeHost.GetBinaryOperator("+").Evaluate(Machine.AX, 1);
      return 1;
    }
  }

  internal class DecOperation : Operation
  {
    public string Id { get; set; }

    public override int Execute(IScriptContext context)
    {
      Machine.AX =
        RuntimeHost.GetBinaryOperator("-").Evaluate(Machine.AX, 1);
      return 1;
    }
  }
}
