using System;
using System.Reflection;

namespace Scripting.SSharp.Runtime
{
  using Configuration;
using System.Collections.Generic;

  public interface IAssemblyManager : IDisposable
  {
    void Initialize(ScriptConfiguration configuration);

    void AddAssembly(Assembly assembly);

    void RemoveAssembly(Assembly assembly);

    Type GetType(string name);

    IEnumerable<MethodInfo> GetExtensionMethods(Type type);

    IEnumerable<MethodInfo> GetExtensionMethods(Type type, string methodName);

    bool HasType(string name);

    bool HasNamespace(string name);

    void AddType(string alias, Type type);

    event EventHandler<AssemblyHandlerEventArgs> BeforeAddAssembly;

    event EventHandler<AssemblyTypeHandlerEventArgs> BeforeAddType;
  }

  public class AssemblyTypeHandlerEventArgs : EventArgs
  {
    public bool Cancel { get; set; }

    public Assembly Assembly { get; private set; }

    public Type Type { get; private set; }

    public AssemblyTypeHandlerEventArgs(Assembly assembly, Type type)
    {
      Cancel = false;
      Assembly = assembly;
      Type = type;
    }
  }

  public class AssemblyHandlerEventArgs : EventArgs
  {
    public bool Cancel { get; set; }

    public Assembly Assembly { get; private set; }

    public AssemblyHandlerEventArgs(Assembly assembly)
    {
      Cancel = false;
      Assembly = assembly;
    }
  }
}
