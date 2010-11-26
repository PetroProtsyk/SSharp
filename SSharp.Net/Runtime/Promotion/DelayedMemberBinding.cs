using System;
using System.Reflection;

namespace Scripting.SSharp.Runtime.Promotion
{
  internal class DelayedMemberBinding : IMemberBinding
  {
    private readonly ObjectBinding _binder;
    private readonly bool _throwNotFound;

    public DelayedMemberBinding(ObjectBinding binder, object target, string name, bool throwNotFound)
    {
      if (binder == null)
        throw new ArgumentNullException("binder");
      if (target == null)
        throw new ArgumentNullException("target");
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException("name");

      _binder = binder;
      _throwNotFound = throwNotFound;

      Target = target;
      TargetType = target as Type ?? target.GetType();
      MemberName = name;
    }

    #region IMemberBind Members

    public string MemberName
    {
      get;
      private set;
    }

    public object Target
    {
      get;
      private set;
    }

    public Type TargetType
    {
      get;
      private set;
    }

    public MemberInfo Member
    {
      get
      {
        throw new NotSupportedException("Member info is not supported on late bound");
      }
    }

    public void SetValue(object value)
    {
      _binder.Set(MemberName, Target, value, _throwNotFound, null);
    }

    public object GetValue()
    {
      return _binder.Get(MemberName, Target, _throwNotFound, null);
    }

    public void AddHandler(object value)
    {
      //This also may set value to property
      _binder.Set(MemberName, Target, value, true, null);
    }

    public void RemoveHandler(object value)
    {
      //This also may set value to property
      _binder.Set(MemberName, Target, value, true, null);
    }

    #endregion

    #region IInvokable Members

    public bool CanInvoke()
    {
      return Target != null && Member != null;
    }

    public object Invoke(IScriptContext context, object[] args)
    {
      return GetValue();
    }

    #endregion
  }
}