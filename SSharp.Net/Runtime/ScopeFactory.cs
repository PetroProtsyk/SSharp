using System.Collections.Generic;

namespace Scripting.SSharp.Runtime
{
  /// <summary>
  /// Base implementation of IScopeFactory interface. Creates instances of defined scope types
  /// </summary>
  public class ScopeFactory : IScopeFactory
  {
    private readonly Dictionary<int, IScopeActivator> _scopeTypes = new Dictionary<int, IScopeActivator>();

    public void RegisterType(int id, IScopeActivator instance)
    {
      if (_scopeTypes.ContainsKey(id))
      {
        _scopeTypes[id] = instance;
      }
      else
      {
        _scopeTypes.Add(id, instance);
      }
    }

    public void RegisterType(ScopeTypes id, IScopeActivator instance)
    {
      RegisterType((int)id, instance);
    }

    public IScriptScope Create()
    {
      return Create((int)ScopeTypes.Default);
    }

    public IScriptScope Create(ScopeTypes id)
    {
      return Create((int)id);
    }

    public IScriptScope Create(ScopeTypes id, params object[] args)
    {
      return Create((int)id, args);
    }

    public IScriptScope Create(int id, params object[] args)
    {
      return Create(id, null, args);
    }

    public IScriptScope Create(ScopeTypes id, IScriptScope parent)
    {
      return Create((int)id, parent);
    }

    public IScriptScope Create(ScopeTypes id, IScriptScope parent, params object[] args)
    {
      return Create((int)id, parent, args);
    }

    public IScriptScope Create(int id, IScriptScope parent, params object[] args)
    {
      return _scopeTypes[id].Create(parent, args);
    }
  }

}
