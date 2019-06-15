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

namespace Scripting.SSharp.Runtime.Promotion
{
  public partial class ObjectBinding
  {
    protected abstract class BaseHandler
    {
      private readonly IObjectBinding _binder;

      protected BaseHandler(IObjectBinding parent)
      {
        _binder = parent;
      }

      protected bool CanBind(MemberInfo member)
      {
        return _binder.CanBind(member);
      }
    }

    protected class PropertyHandler : BaseHandler, IHandler
    {
      public PropertyHandler(IObjectBinding parent)
        : base(parent)
      {
      }

      #region IGetter Members
      public object Get(string name, object instance, Type type, params object[] arguments)
      {
        PropertyInfo pi = GetProperty(type, name);
        if (pi == null) return NoResult;
        if (!CanBind(pi)) return NoResult;

        return pi.GetValue(instance, arguments);
      }
      #endregion

      #region ISetter Members

      //private static Dictionary<Type, Dictionary<string, PropertyInfo>> propertyCache = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

      //private static PropertyInfo GetPropertyInfo(Type ownerType, string propertyName)
      //{
      //  Dictionary<string, PropertyInfo> props = null;
      //  if (!propertyCache.TryGetValue(ownerType, out props))
      //  {
      //    props = new Dictionary<string, PropertyInfo>();
      //    propertyCache.Add(ownerType, props);
      //  }

      //  PropertyInfo pi = null;
      //  if (!props.TryGetValue(propertyName, out pi))
      //  {
      //    pi = ownerType.GetProperty(propertyName, ObjectBinding.PropertyFilter);
      //    if (pi != null) props.Add(propertyName, pi);
      //  }

      //  return pi;
      //}

      
      public object Set(string name, object instance, Type type, object value, params object[] arguments)
      {                
        PropertyInfo pi = GetProperty(type, name);        
        // TODO: Denis: quick and dirty optimization, needs review and Concurrency support
        //PropertyInfo pi = GetPropertyInfo(type, name);
        if (pi == null) return NoResult;
        if (!CanBind(pi)) return NoResult;
                

        if (value != null && pi.PropertyType.IsAssignableFrom(value.GetType()))
        {       
          pi.SetValue(instance, value, null);
        }
        else
        {
          pi.SetValue(instance, RuntimeHost.Binder.ConvertTo(value, pi.PropertyType), arguments);
        }
        return value;
      }

      private static PropertyInfo GetProperty(Type type, string name)
      {        
        return type.GetProperty(name, PropertyFilter);
      }

      #endregion
    }

    protected class FieldHandler : BaseHandler, IHandler
    {
      public FieldHandler(IObjectBinding parent)
        : base(parent)
      {
      }

      #region IGetter Members
      public object Get(string name, object instance, Type type, params object[] arguments)
      {
        var fi = type.GetField(name, FieldFilter);
        if (fi == null) return NoResult;
        return !CanBind(fi) ? NoResult : fi.GetValue(instance);
      }
      #endregion

      #region ISetter Members

      public object Set(string name, object instance, Type type, object value, params object[] arguments)
      {
        var fi = type.GetField(name, FieldFilter);
        if (fi == null) return NoResult;
        if (!CanBind(fi)) return NoResult;

        fi.SetValue(instance, RuntimeHost.Binder.ConvertTo(value, fi.FieldType));
        return value;
      }

      #endregion
    }

    protected class EventHandler : BaseHandler, IHandler
    {
      public EventHandler(IObjectBinding parent)
        : base(parent)
      {
      }

      #region IGetter Members
      public object Get(string name, object instance, Type type, params object[] arguments)
      {
        var ei = type.GetEvent(name, PropertyFilter);
        if (ei == null) return NoResult;
        return !CanBind(ei) ? NoResult : ei;

        //Type eventHelper = typeof(EventHelper<>);
        //Type actualHelper = eventHelper.MakeGenericType(ei.EventHandlerType);
      }
      #endregion

      #region ISetter Members

      public object Set(string name, object instance, Type type, object value, params object[] arguments)
      {
        EventInfo ei = type.GetEvent(name, PropertyFilter);
        if (ei == null) return NoResult;
        if (!CanBind(ei)) return NoResult;

        RemoveDelegate remove_delegate = value as RemoveDelegate;

        if (remove_delegate==null)
          EventBroker.AssignEvent(ei, instance, (IInvokable)value);
        else
          EventBroker.RemoveEvent(ei, instance, remove_delegate.OriginalMethod);

        return value;
      }

      #endregion
    }

    protected class MethodGetter : BaseHandler, IGetter
    {
      public MethodGetter(IObjectBinding parent)
        : base(parent)
      {
      }

      #region IGetter Members
      public object Get(string name, object instance, Type type, params object[] arguments)
      {
        // TODO: Denis: this should be cached to improve performance        
        var methods = type.GetMethods(MethodFilter).Where(m => m.Name == name && CanBind(m)).ToArray();
        if (methods == null || methods.Length == 0) return NoResult;

        return new DelayedMethodBinding(name, instance);
      }
      #endregion
    }
   
    protected class ScriptableHandler : BaseHandler, IHandler
    {
      public ScriptableHandler(IObjectBinding parent)
        : base(parent)
      {
      }

      #region IGetter Members
      public object Get(string name, object instance, Type type, params object[] arguments)
      {
        IScriptable dm = instance as IScriptable;
        if (dm == null) return NoResult;

        return dm.GetMember(name, arguments).GetValue();
      }
      #endregion

      #region ISetter Members

      public object Set(string name, object instance, Type type, object value, params object[] arguments)
      {
        IScriptable dm = instance as IScriptable;
        if (dm == null) return NoResult;

        dm.GetMember(name, arguments).SetValue(value);
        return value;
      }

      #endregion
    }

    protected class NestedTypeGetter : BaseHandler, IGetter
    {
      public NestedTypeGetter(IObjectBinding parent)
        : base(parent)
      {
      }

      #region IGetter Members
      public object Get(string name, object instance, Type type, params object[] arguments)
      {
        var nested = type.GetNestedType(name, NestedTypeFilter);
        return nested ?? NoResult;
      }
      #endregion
    }  
  }
}