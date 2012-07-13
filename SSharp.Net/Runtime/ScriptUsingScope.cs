using System;
using System.Collections.Generic;
using System.Reflection;
using Scripting.SSharp.Runtime.Promotion;

namespace Scripting.SSharp.Runtime
{
  public class ScriptUsingScope : ScriptScope
  {
    Dictionary<string, IInvokable> bindings = new Dictionary<string, IInvokable>();
    Dictionary<string, IMemberBinding> members = new Dictionary<string, IMemberBinding>();
    object usingObject; 

    public ScriptUsingScope(IScriptScope parent, object usingObject):
        base(parent)
    {
      if (parent == null) throw new ArgumentNullException("parent");
      if (usingObject == null) throw new ArgumentNullException("usingObject");
      this.usingObject = usingObject;

      Type type = usingObject as Type;
      if (type == null) type = usingObject.GetType(); ;

      IEnumerable<MethodInfo> methods = type.GetMethods(ObjectBinding.MethodFilter);
      foreach (MethodInfo method in methods)
      {
        if (bindings.ContainsKey(method.Name)) continue;
        bindings.Add(method.Name, new DelayedMethodBinding(method.Name, usingObject));
      }

      IEnumerable<PropertyInfo> properties = type.GetProperties(ObjectBinding.PropertyFilter);     
      foreach (PropertyInfo property in properties)
      {
        if (members.ContainsKey(property.Name)) continue;
        members.Add(property.Name, RuntimeHost.Binder.BindToMember(usingObject, property.Name, true));
      }

      IEnumerable<FieldInfo> fields = type.GetFields(ObjectBinding.FieldFilter);
      foreach (FieldInfo field in fields)
      {
        if (members.ContainsKey(field.Name)) continue;
        members.Add(field.Name, RuntimeHost.Binder.BindToMember(usingObject, field.Name, true));
      }

    }

    protected override object GetVariableInternal(string id, bool searchHierarchy)
    {
      IInvokable result;
      if (bindings.TryGetValue(id, out result)) return result;
      IMemberBinding member;
      if (members.TryGetValue(id, out member)) return member.GetValue();
      
      return base.GetVariableInternal(id, searchHierarchy);
    }

    public override bool HasVariable(string id)
    {
      if (bindings.ContainsKey(id)) return true;
      if (members.ContainsKey(id)) return true;
      return base.HasVariable(id);
    }

    public override void SetItem(string id, object value)
    {
      IMemberBinding member;
      if (members.TryGetValue(id, out member))
      {
        member.SetValue(value);
        return;
      }

      if (bindings.ContainsKey(id)) throw new ScriptException("Can't assign value to existing binding");

      Parent.SetItem(id, value);
    }

    public override IValueReference Ref(string id)
    {
      if (bindings.ContainsKey(id)) return null;
      if (members.ContainsKey(id)) return null;
      return Parent.Ref(id);
    }
    
    public override void Clean()
    {
      base.Clean();
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
