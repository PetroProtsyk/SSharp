
namespace Scripting.SSharp.Execution.VM.Operations
{
  using Runtime;

  public class IncOperation : Operation
  {
    public string Id { get; set; }

    public override int Execute(IScriptContext context)
    {
      Machine.AX =
        RuntimeHost.GetBinaryOperator("+").Evaluate(Machine.AX, 1);
      return 1;
    }
  }
  
  public class DecOperation : Operation
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
