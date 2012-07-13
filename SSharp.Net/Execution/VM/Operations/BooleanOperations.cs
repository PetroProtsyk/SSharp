
namespace Scripting.SSharp.Execution.VM.Operations
{
  using Runtime;

  public class CmpOperation : Operation
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
