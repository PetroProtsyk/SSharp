using System;
using System.Reflection;

namespace Scripting.SSharp.Runtime.Promotion
{
  internal class NamespaceBinding : IMemberBinding
  {
    private readonly string _name;

    public NamespaceBinding(string name)
    {
      _name = name;
    }

    #region IMemberBind Members

    public object Target
    {
      get { throw new NotSupportedException(); }
    }

    public Type TargetType
    {
      get { throw new NotSupportedException(); }
    }

    public MemberInfo Member
    {
      get { throw new NotSupportedException(); }
    }

    public void SetValue(object value)
    {
      throw new NotSupportedException();
    }

    public object GetValue()
    {
      return RuntimeHost.HasType(_name) ? (object) RuntimeHost.GetType(_name) : new Namespace(_name);
    }

    public void AddHandler(object value)
    {
      throw new NotSupportedException();
    }

    public void RemoveHandler(object value)
    {
      throw new NotSupportedException();
    }

    #endregion

    #region IInvokable Members

    public bool CanInvoke()
    {
      throw new NotImplementedException();
    }

    public object Invoke(IScriptContext context, object[] args)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
