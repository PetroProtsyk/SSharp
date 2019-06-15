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
using System.Reflection;
using System.Linq;
using Scripting.SSharp.Runtime.Promotion;

namespace Scripting.SSharp.Runtime.Reflection
{
  // TODO: make concurrent
  internal class MethodProvider
  {
    #region IBinding cache

    private static readonly Dictionary<Type, Dictionary<string, IBinding>> Cache = new Dictionary<Type, Dictionary<string, IBinding>>();

    public static IBinding GetBinding(Type targetType, string methodName)
    {
      Dictionary<string, IBinding> bindings;
      if (!Cache.TryGetValue(targetType, out bindings)) return null;

      IBinding binding;
      return !bindings.TryGetValue(methodName, out binding) ? null : binding;
    }

    public static void AddBinding(Type targetType, string methodName, IBinding binding)
    {
      Dictionary<string, IBinding> bindings;
      if (!Cache.TryGetValue(targetType, out bindings))
      {
        bindings = new Dictionary<string, IBinding>();
        Cache.Add(targetType, bindings);
      }
      bindings[methodName] = binding;
    } 

    #endregion

    #region Conversion cachce

    private static readonly Dictionary<Type, Dictionary<Type, MethodInfo>> ConversionCache = new Dictionary<Type, Dictionary<Type, MethodInfo>>();

    public static MethodInfo GetConversionMethod(Type valueType,Type targetType)
    {
      Dictionary<Type, MethodInfo> conversions;
      MethodInfo method;

      if (!ConversionCache.TryGetValue(valueType, out conversions))
      {
        conversions = new Dictionary<Type, MethodInfo>();
        ConversionCache.Add(valueType,conversions);
      }

      if (!conversions.TryGetValue(targetType, out method))
      {
        // Try gettting implicit converter
          IEnumerable<MethodInfo> methods=valueType
                                        .GetMethods(BindingFlags.Static | BindingFlags.Public)
                                        .Where(m=>m.ReturnType==targetType);

          method = methods
                     .Where(m => m.Name=="op_Implicit" && m.GetParameters().First().ParameterType == valueType)
                     .FirstOrDefault();

        // Try getting explicit converter
          if (method == null) 
          {
              method = methods
                     .Where(m => m.Name == "op_Explicit" && m.GetParameters().First().ParameterType == valueType)
                     .FirstOrDefault();
          }
         // put the value into the cache regarldess it's state (null value preserving will allow avoid crawling types that do not have conversion methods at all)

        conversions.Add(targetType, method);
      }

      return method;
    }

    #endregion
  }
}
