using System;
using System.Collections.Generic;
using System.Reflection;
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

    private static readonly Dictionary<Type, MethodInfo> ConversionCache = new Dictionary<Type, MethodInfo>();

    public static MethodInfo GetConversionMethod(Type valueType)
    {
      MethodInfo method;

      if (!ConversionCache.TryGetValue(valueType, out method))
      {
        // Try gettting implicit converter
        method = valueType.GetMethod("op_Implicit", BindingFlags.Static | BindingFlags.Public, null, new[] { valueType }, null);
        // Try getting explicit converter
        if (method == null) method = valueType.GetMethod("op_Explicit", BindingFlags.Static | BindingFlags.Public, null, new[] { valueType }, null);
        // put the value into the cache regarldess it's state (null value preserving will allow avoid crawling types that do not have conversion methods at all)
        ConversionCache.Add(valueType, method);
      }

      return method;
    }

    #endregion
  }
}
