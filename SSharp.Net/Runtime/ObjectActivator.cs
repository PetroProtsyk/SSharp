using System;
using Scripting.SSharp.Runtime.Promotion;

namespace Scripting.SSharp.Runtime
{
  /// <summary>
  /// Base implementation of IObjectActivator interface. Default ObjectActivator used
  /// by RuntimeHost to create any instance from the script.
  /// </summary>
  public class ObjectActivator : IObjectActivator
  {
    #region IObjectActivator Members
    public object CreateInstance(Type type, object[] args)
    {
      return CreateInstance(null, RuntimeHost.Binder.BindToConstructor(type, args));
    }

    public object CreateInstance(IScriptContext context, IBinding bind)
    {
      return bind.Invoke(context, null);
    }

    #endregion
  }
}
