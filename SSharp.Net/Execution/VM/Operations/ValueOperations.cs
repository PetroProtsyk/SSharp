namespace Scripting.SSharp.Execution.VM.Operations
{
  using Runtime;

  public class GetValueOperation : Operation
  {
    public string Id { get; set; }

    public override int Execute(IScriptContext context)
    {
      Machine.AX = context.GetItem(Id, true);
      return 1;
    }
  }

  public class SetValueOperation : Operation
  {
    public string Id { get; set; }

    public override int Execute(IScriptContext context)
    {
      context.SetItem(Id, Machine.AX);
      return 1;
    }
  }

  public class ValueOperation : Operation
  {
    public object Value { get; set; }

    public override int Execute(IScriptContext context)
    {
      Machine.AX = Value;
      return 1;
    }
  }
}
