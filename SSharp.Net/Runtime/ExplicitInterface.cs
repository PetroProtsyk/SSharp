/*
 * Copyright © 2011, Petro Protsyk, Denys Vuika
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *  http://www.apache.org/licenses/LICENSE-2.0
 *  
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Linq;
using System.Reflection;
using Scripting.SSharp.Runtime.Promotion;

namespace Scripting.SSharp.Runtime
{
#if !PocketPC && !SILVERLIGHT
  public class ExplicitInterface : IScriptable
  {
    private InterfaceMapping _mapping;
    private readonly object _target;

    public ExplicitInterface(object target, Type interfaceType)
    {
      if (target == null) throw new ArgumentNullException("target");
      if (interfaceType == null) throw new ArgumentNullException("interfaceType");

      _mapping = target.GetType().GetInterfaceMap(interfaceType);
      _target = target;
    }

    #region IScriptable Members

    public object Instance
    {
      get { return _target; }
    }

    [PromoteAttribute(false)]
    public IMemberBinding GetMember(string name, params object[] arguments)
    {
      IBinding getter = null;
      MethodInfo getMethod = FindMethod("get_" + name);
      if (getMethod != null)
        getter = new DelayedMethodBinding(getMethod, _target);

      IBinding setter = null;
      MethodInfo setMethod = FindMethod("set_" + name);
      if (setMethod != null)
        setter = new DelayedMethodBinding(setMethod, _target);

      return new InterfaceMember(getter, setter, _target);
    }

    [PromoteAttribute(false)]
    public IBinding GetMethod(string name, params object[] arguments)
    {
      MethodInfo method = FindMethod(name);
      if (method == null)
        throw new ScriptMethodNotFoundException(name);

      return new DelayedMethodBinding(method, _target);
    }

    [PromoteAttribute(false)]
    private MethodInfo FindMethod(string name)
    {
      MethodInfo method = _mapping.InterfaceMethods.Where(i => i.Name == name).FirstOrDefault();
      return method;
    }

    #endregion

    #region Interface Property Bind
    private class InterfaceMember : IMemberBinding
    {
      private readonly IBinding _getter;
      private readonly IBinding _setter;

      public InterfaceMember(IBinding getter, IBinding setter, object target)
      {
        _getter = getter;
        _setter = setter;
        Target = target;
      }

      #region IMemberBind Members

      public object Target
      {
        get;
        set;
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
        if (_setter == null) throw new NotSupportedException();
        _setter.Invoke(null, new[] { value });
      }

      public object GetValue()
      {
        if (_getter == null) throw new NotSupportedException();
        return _getter.Invoke(null, new object[0]);
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
        return _getter != null;
      }

      public object Invoke(IScriptContext context, object[] args)
      {
        return GetValue();
      }

      #endregion
    }
    #endregion
  }
#endif
}
