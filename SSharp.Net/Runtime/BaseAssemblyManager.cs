using System;
using System.Collections.Generic;
using System.Reflection;
using Scripting.SSharp.Diagnostics;
using Scripting.SSharp.Runtime.Configuration;

namespace Scripting.SSharp.Runtime
{
  public class BaseAssemblyManager : IAssemblyManager
  {
    #region Properties
    /// <summary>
    /// List of assemblies which are in use for the current moment.
    /// NOTE: This list may be changed during run-time (new assemblies may be added, some of them may be removed)
    /// </summary>
    protected  readonly List<Assembly> WorkingAssemblies = new List<Assembly>();

    /// <summary>
    /// Types cache. Contains all loaded types
    /// </summary>
    protected readonly Dictionary<string, Type> Types = new Dictionary<string, Type>();

    /// <summary>
    /// Types by short names
    /// </summary>
    protected readonly Dictionary<string, Type> ShortTypes = new Dictionary<string, Type>();

    /// <summary>
    /// Cache of Namesapces
    /// </summary>
    protected readonly Dictionary<string, List<Type>> Namespaces = new Dictionary<string, List<Type>>();

    protected ScriptConfiguration Configuration { get; private set; }

    #endregion

    #region Initialization
    [Promote(false)]
    public BaseAssemblyManager()
    {
    }
    
    [Promote(false)]
    public virtual void Initialize(ScriptConfiguration configuration)
    {
      Requires.NotNull<ScriptConfiguration>(configuration, "configuration");

      this.Configuration = configuration;
      FindAliasTypes();

      LoadAssemblies();
      ScanAssemblies();
    }
    #endregion

    #region Methods
    /// <summary>
    /// Loads assemblies from configuration to memory and generate 
    /// LoadedAssemblies list which will be scanned for types
    /// </summary>
    protected virtual void LoadAssemblies()
    {
      if (Configuration == null) return;

      foreach (Reference reference in Configuration.References)
      {
        Assembly assembly = reference.Load();

        if (WorkingAssemblies.Contains(assembly))
          throw new NotSupportedException("Duplicate assembly in configuration");
        
        WorkingAssemblies.Add(assembly);
      }
    }

    /// <summary>
    /// Scans types in Loaded assemblies
    /// </summary>
    protected virtual void ScanAssemblies()
    {
      foreach (Assembly assembly in WorkingAssemblies.ToArray())
        AddAssembly(assembly);
    }

    protected virtual void RegisterType(Type type)
    {
      Requires.NotNull<Type>(type, "type");

      if (!Types.ContainsKey(type.FullName))
      {
        Types.Add(type.FullName, type);

        //Register in namespaces
        if (!string.IsNullOrEmpty(type.Namespace))
        {
          if (Namespaces.ContainsKey(type.Namespace))
          {
            Namespaces[type.Namespace].Add(type);
          }
          else
          {
            List<Type> types = new List<Type>() { type };
            Namespaces.Add(type.Namespace, types);
          }
        }
      }

      if (!ShortTypes.ContainsKey(type.Name))
      {
        ShortTypes.Add(type.Name, type);
      }
    }

    protected virtual void UnRegisterType(Type type)
    {
      Requires.NotNull<Type>(type, "type");

      if (Types.ContainsKey(type.FullName))
      {
        Types.Remove(type.FullName);

        //Clear namespace cache
        if (!string.IsNullOrEmpty(type.Namespace))
        {
          List<Type> types = Namespaces[type.Namespace];
          types.Remove(type);
          if (types.Count == 0)
            Namespaces.Remove(type.Namespace);
        }
      }

      if (ShortTypes.ContainsKey(type.Name))
      {
        ShortTypes.Remove(type.Name);
      }

      //Remove all aliases
      if (ShortTypes.ContainsValue(type))
      {
        List<string> keysToRemove = new List<string>();
        foreach (KeyValuePair<string, Type> value in ShortTypes)
        {
          if (value.Value == type) keysToRemove.Add(value.Key);
        }

        foreach (string key in keysToRemove)
          ShortTypes.Remove(key);
      }
    }

    private void FindAliasTypes()
    {
      if (Configuration == null) return;

      foreach (TypeXml typeXml in Configuration.Types)
      {
        Type type = RuntimeHost.GetNativeType(typeXml.QualifiedName);
        if (type == null) throw new NullReferenceException(string.Format("Type {0} is not found", typeXml.QualifiedName));
        AddType(typeXml.Alias, type);
      }
    }

    protected virtual AssemblyHandlerEventArgs OnBeforeAddAssembly(Assembly assembly)
    {
      Requires.NotNull<Assembly>(assembly, "assembly");

      AssemblyHandlerEventArgs args = new AssemblyHandlerEventArgs(assembly);

      var hanlder = BeforeAddAssembly;
      if (hanlder != null) hanlder(this, args);

      return args;
    }

    public event EventHandler<AssemblyHandlerEventArgs> BeforeAddAssembly;

    protected virtual AssemblyTypeHandlerEventArgs OnBeforeAddType(Assembly assembly, Type type)
    {
      Requires.NotNull<Assembly>(assembly, "assembly");
      Requires.NotNull<Type>(type, "type");

      AssemblyTypeHandlerEventArgs args = new AssemblyTypeHandlerEventArgs(assembly, type);
      var handler = BeforeAddType;
      if (handler != null) handler(this, args);

      return args;
    }

    public event EventHandler<AssemblyTypeHandlerEventArgs> BeforeAddType;

    #endregion

    #region Public Interface
    public virtual void AddAssembly(Assembly assembly)
    {
      Requires.NotNull<Assembly>(assembly, "assembly");

      if (OnBeforeAddAssembly(assembly).Cancel)
      {
        WorkingAssemblies.Remove(assembly);
        return;
      }

      if (!WorkingAssemblies.Contains(assembly))
      {
        WorkingAssemblies.Add(assembly);
      }

      foreach (Type type in assembly.GetTypes())
      {
        if (!type.IsPublic) continue;

        if (OnBeforeAddType(assembly, type).Cancel)
          continue;

        RegisterType(type);
      }
    }

    public virtual void RemoveAssembly(Assembly assembly)
    {
      Requires.NotNull<Assembly>(assembly, "assembly");

      if (!WorkingAssemblies.Contains(assembly)) return;

      foreach (Type type in assembly.GetTypes())
      {
        if (!type.IsPublic) continue;

        UnRegisterType(type);
      }

      WorkingAssemblies.Remove(assembly);
    }
    /// <summary>
    /// Returns type by given name
    /// </summary>
    /// <param name="name">Short, Alias or FullType name</param>
    /// <returns>Type</returns>
    /// <exception cref="ScriptNET.ScriptException">
    ///  If type not found
    /// </exception>
    public Type GetType(string name)
    {
      Requires.NotNullOrEmpty(name, "name");

      Type result;

      if (ShortTypes.TryGetValue(name, out result)) return result;
      if (Types.TryGetValue(name, out result)) return result;

      throw new ScriptException(string.Format("Type with given name \"{0}\"is not found", name));
    }

    public bool HasType(string name)
    {
      Requires.NotNullOrEmpty(name, "name");

      return ShortTypes.ContainsKey(name) || Types.ContainsKey(name);
    }

    /// <summary>
    /// Adds type to a manager
    /// </summary>
    /// <param name="alias"></param>
    /// <param name="type"></param>
    public void AddType(string alias, Type type)
    {
      Requires.NotNullOrEmpty(alias, "alias");
      Requires.NotNull<Type>(type, "type");

      RuntimeHost.Lock();
      try
      {
        if (ShortTypes.ContainsKey(alias))
        {
          ShortTypes[alias] = type;
        }
        else
        {
          ShortTypes.Add(alias, type);
        }
      }
      finally
      {
        RuntimeHost.UnLock();
      }
    }

    public bool HasNamespace(string name)
    {
      Requires.NotNullOrEmpty(name, "name");

      return Namespaces.ContainsKey(name);
    }
    #endregion

    #region IDisposable Members
    [Promote(false)]
    public virtual void Dispose()
    {
      ShortTypes.Clear();
      Types.Clear();
    }

    #endregion
  }
}
