using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;

namespace Scripting.SSharp.Runtime
{
  /// <summary>
  /// Provides creation and caching mechanisms for dynamic object members.
  /// </summary>
  internal static class CallSiteCache
  {
    private static readonly CSharpArgumentInfo[] GetterArgumentInfo = new[] 
    { 
      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) 
    };

    private static readonly CSharpArgumentInfo[] SetterArgumentInfo = new[] 
    { 
      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), 
      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) 
    };

    private static readonly Dictionary<string, CallSite<Func<CallSite, object, object>>> GettersCache =
      new Dictionary<string, CallSite<Func<CallSite, object, object>>>();

    private static readonly Dictionary<string, CallSite<Func<CallSite, object, object, object>>> SettersCache =
      new Dictionary<string, CallSite<Func<CallSite, object, object, object>>>();
    
    private static CallSite<Func<CallSite, object, object>> CreateGetter(string propertyName)
    {
      var getter = CallSite<Func<CallSite, object, object>>.Create(
        Binder.GetMember(CSharpBinderFlags.None, propertyName, typeof(CallSiteCache), GetterArgumentInfo));

      return getter;
    }

    private static CallSite<Func<CallSite, object, object, object>> CreateSetter(string propertyName)
    {
      var setter = CallSite<Func<CallSite, object, object, object>>.Create(
          Binder.SetMember(CSharpBinderFlags.None, propertyName, typeof(CallSiteCache), SetterArgumentInfo));

      return setter;
    }

    //getter.Target.Invoke(getter, obj);
    public static CallSite<Func<CallSite, object, object>> GetOrCreatePropertyGetter(string propertyName)
    {
      if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentNullException("propertyName");

      CallSite<Func<CallSite, object, object>> getter;
      if (GettersCache.TryGetValue(propertyName, out getter))
        return getter;

      getter = CreateGetter(propertyName);
      GettersCache.Add(propertyName, getter);
      return getter;
    }

    // site.Target(site, o, value);
    public static CallSite<Func<CallSite, object, object, object>> GetOrCreatePropertySetter(string propertyName)
    {
      if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentNullException("propertyName");

      CallSite<Func<CallSite, object, object, object>> setter;
      if (SettersCache.TryGetValue(propertyName, out setter))
        return setter;

      setter = CreateSetter(propertyName);
      SettersCache.Add(propertyName, setter);
      return setter;
    }
  }
}
