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
using Scripting.SSharp.Runtime.Promotion;

namespace Scripting.SSharp.Runtime
{
  /// <summary>
  /// Creates instances by given type information
  /// </summary>
  public interface IObjectActivator
  {
    object CreateInstance(Type type, object[] args);

    object CreateInstance(IScriptContext context, IBinding bind);
  }

  public static class ObjectActivatorExtensions
  {
    public static object CreateInstance(this IObjectActivator activator, IScriptContext context, IBinding bind)
    {
      return activator.CreateInstance(null, bind);
    }

    public static object CreateInstance(this IObjectActivator activator, Type type)
    {
      return activator.CreateInstance(type, null);
    }

    public static T CreateInstance<T>(this IObjectActivator activator)
    {
      return (T)CreateInstance(activator, typeof(T));
    }

    public static T CreateInstance<T>(this IObjectActivator activator, object[] args)
    {
      return (T)activator.CreateInstance(typeof(T), args);
    }

  }
}
