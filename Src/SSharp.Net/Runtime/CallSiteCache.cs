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
