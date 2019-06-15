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
      Requires.NotNull(context, "context");
      Requires.NotNull(functions, "functions");

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