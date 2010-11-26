namespace Scripting.SSharp.Execution.VM.Operations
{
  using Runtime;

  internal class ObjectMemberOperation : Operation
  {
    public string MemberName { get; set; }

    public override int Execute(IScriptContext context)
    {
      var parameters = new object[(int)Machine.BX];
      var stack = Machine.Stack;

      for (int i = 0; i < parameters.Length; i++)
        parameters[parameters.Length - 1 - i] = stack.Pop();

      var target = stack.Pop();
      
      var bind = RuntimeHost.Binder.BindToMethod(target, MemberName, null, parameters);

      Machine.AX = bind.Invoke(context, parameters);

      return 1;
    }
  }

}
