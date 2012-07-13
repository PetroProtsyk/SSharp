using System.Collections.Generic;

namespace Scripting.SSharp.Execution.VM.Operations
{
  using Runtime;
  using Scripting.SSharp.Runtime.Promotion;

  public class ObjectMemberOperation : Operation
  {
    public string MemberName { get; set; }

    public override int Execute(IScriptContext context)
    {
      object[] parameters = new object[(int)Machine.BX];
      Stack<object> stack = Machine.Stack;

      for (int i = 0; i < parameters.Length; i++)
        parameters[parameters.Length - 1 - i] = stack.Pop();

      object target = stack.Pop();
      
      IBinding bind = RuntimeHost.Binder.BindToMethod(target, MemberName, null, parameters);

      Machine.AX = bind.Invoke(context, parameters);

      return 1;
    }
  }

}
