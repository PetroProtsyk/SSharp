using System;
using Scripting.SSharp.Diagnostics;

namespace Scripting.SSharp.Runtime
{
  public static class ScopeServices
  {
    public static IScriptContext SetFunction<TInvokable>(this IScriptContext context, string id) where TInvokable : IInvokable, new()
    {
      if (context == null) return null;
      context.SetItem(id, new TInvokable());
      return context;
    }

    public static T GetItemAs<T>(this IScriptContext context, string id) where T : class
    {
      return GetItemAs<T>(context, id, true);
    }

    public static T GetItemAs<T>(this IScriptContext context, string id, bool throwException) where T : class
    {
      if (context == null) throw new ArgumentNullException("context");
      return context.GetItem(id, throwException) as T;
    }

    public static IScriptContext Import(this IScriptContext context, FunctionTable functions)
    {
      Requires.NotNull<IScriptContext>(context, "context");
      Requires.NotNull<FunctionTable>(functions, "functions");

      foreach (var functionInfo in functions)
        context.SetItem(functionInfo.Key, Activator.CreateInstance(functionInfo.Value));

      return context;
    }

    public static IScriptContext Import<TTable>(this IScriptContext context) where TTable : FunctionTable, new()
    {
      Import(context, new TTable());            
      return context;
    }
  }
}