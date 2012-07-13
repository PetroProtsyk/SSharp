using System;
using System.Reflection;

namespace Scripting.SSharp.Runtime.Promotion
{
  internal class NamespaceBinding : IMemberBinding
  {
    string name;

    public NamespaceBinding(string name)
    {
      this.name = name;
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
      if (RuntimeHost.HasType(name))
      {
        return RuntimeHost.GetType(name);
      }
      else
      {
        return new Namespace(name);
      }
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
