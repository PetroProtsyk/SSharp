using System;
using System.Reflection;

namespace Scripting.SSharp.Runtime.Promotion
{
  internal class ConstructorBinding : IBinding
  {
    ConstructorInfo Method { get; set; }
    object[] Arguments { get; set; }

    public Type Type { get; private set; }

    public ConstructorBinding(ConstructorInfo method, object[] arguments)
    {
      Method = method;
      Arguments = arguments;
      Type = method.DeclaringType;
    }

    public ConstructorBinding(Type type, ConstructorInfo method, object[] arguments)
    {
      Method = method;
      Arguments = arguments;
      Type = type;
    }
    #region IInvokable Members

    public bool CanInvoke()
    {
      return Method != null;
    }

    public object Invoke(IScriptContext context, object[] args)
    {
      return Method.Invoke(Arguments);
    }

    #endregion
  }
}
