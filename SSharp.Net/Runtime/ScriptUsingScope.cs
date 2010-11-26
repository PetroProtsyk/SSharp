using System;
using System.Collections.Generic;
using System.Linq;
using Scripting.SSharp.Runtime.Promotion;

namespace Scripting.SSharp.Runtime
{
  public class ScriptUsingScope : ScriptScope
  {
    private Dictionary<string, IInvokable> _bindings = new Dictionary<string, IInvokable>();
    private Dictionary<string, IMemberBinding> _members = new Dictionary<string, IMemberBinding>();
    private object _usingObject;

    public ScriptUsingScope(IScriptScope parent, object usingObject):
        base(parent)
    {
      if (parent == null) throw new ArgumentNullException("parent");
      if (usingObject == null) throw new ArgumentNullException("usingObject");
      _usingObject = usingObject;

      var type = usingObject as Type ?? usingObject.GetType();

      var methods = type.GetMethods(ObjectBinding.MethodFilter);
      foreach (var method in methods.Where(method => !_bindings.ContainsKey(method.Name)))
        _bindings.Add(method.Name, new DelayedMethodBinding(method.Name, usingObject));

      var properties = type.GetProperties(ObjectBinding.PropertyFilter);     
      foreach (var property in properties.Where(property => !_members.ContainsKey(property.Name)))
        _members.Add(property.Name, RuntimeHost.Binder.BindToMember(usingObject, property.Name, true));

      var fields = type.GetFields(ObjectBinding.FieldFilter);
      foreach (var field in fields.Where(field => !_members.ContainsKey(field.Name)))
        _members.Add(field.Name, RuntimeHost.Binder.BindToMember(usingObject, field.Name, true));
    }

    protected override object GetVariableInternal(string id, bool searchHierarchy)
    {
      IInvokable result;
      if (_bindings.TryGetValue(id, out result)) return result;
      IMemberBinding member;
      if (_members.TryGetValue(id, out member)) return member.GetValue();
      
      return base.GetVariableInternal(id, searchHierarchy);
    }

    public override bool HasVariable(string id)
    {
      if (_bindings.ContainsKey(id)) return true;
      if (_members.ContainsKey(id)) return true;
      return base.HasVariable(id);
    }

    public override void SetItem(string id, object value)
    {
      IMemberBinding member;
      if (_members.TryGetValue(id, out member))
      {
        member.SetValue(value);
        return;
      }

      if (_bindings.ContainsKey(id)) throw new ScriptRuntimeException(string.Format(Strings.UsingScopeBindingErrorDuringSetOperation, id));

      Parent.SetItem(id, value);
    }

    public override IValueReference Ref(string id)
    {
      if (_bindings.ContainsKey(id)) return null;
      if (_members.ContainsKey(id)) return null;
      return Parent.Ref(id);
    }

    protected override void Cleanup()
    {
      try
      {
        _usingObject = null;

        if (_bindings != null)
        {
          _bindings.Clear();
          _bindings = null;
        }

        if (_members != null)
        {
          _members.Clear();
          _members = null;
        }
      }
      finally
      {
        base.Cleanup();
      }
    }
  }

  public class ScriptUsingScopeActivator : IScopeActivator
  {
    #region IScopeActivator Members

    public IScriptScope Create(IScriptScope parent, params object[] args)
    {
      if (args.Length == 1)
        return new ScriptUsingScope(parent, args[0]);

      throw new NotSupportedException();
    }

    #endregion
  }

}
