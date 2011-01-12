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

namespace Scripting.SSharp.Runtime.Promotion
{
  /// <summary>
  /// Default object binder used by Runtime
  /// </summary>
  public class DefaultObjectBinding : ObjectBinding, IObjectBinding
  {
      IBinding IObjectBinding.BindToMethod(object target, string methodName, Type[] genericParameters, object[] arguments) {
          var scriptable = target as IScriptable;
          if (scriptable != null) {
              var bind = scriptable.GetMethod(methodName, null);
              if (bind == null)
                  bind = BindToMethod(scriptable.Instance, methodName, genericParameters, arguments);
              else
                  return new DynamicMethodBind(scriptable, bind, arguments);

              if (bind != null) return bind;
          }

          IBinding result = BindToMethod(target, methodName, genericParameters, arguments);          
          //Check extension Methods
          if (result == null) {
              Type type = target as Type ?? target.GetType();
              object[] extArguments = new object[arguments.Length + 1];
              extArguments[0] = target;
              arguments.CopyTo(extArguments, 1);

              foreach (Type t in new Type[] { type }.Concat(type.GetInterfaces())) {
                  result = BindToMethods(t, genericParameters, extArguments, RuntimeHost.AssemblyManager.GetExtensionMethods(t, methodName));
                  if (result != null) return result;
              }
          }

          return result;
      }

      IBinding IObjectBinding.BindToIndex(object target, object[] arguments, bool setter) {
          IScriptable scriptable = target as IScriptable;
          if (scriptable != null) {
              IBinding bind = base.BindToIndex(scriptable.Instance, arguments, setter);
              if (bind != null) return bind;
          }

          string stringValue = target as string;
          if (stringValue != null) {
              return new StringIndexerBinding(stringValue, (int)arguments[0]);
          }

          return base.BindToIndex(target, arguments, setter);
      }

    #region DynamicMethodBind
    protected class DynamicMethodBind : IBinding
    {
      private readonly IScriptable _scriptable;
      private readonly IBinding _dynamicMethod;
      private readonly object[] _arguments;

      public DynamicMethodBind(IScriptable scriptable, IBinding dynamicMethod, object[] arguments)
      {
        _scriptable = scriptable;
        _dynamicMethod = dynamicMethod;
        _arguments = arguments;
      }

      #region IInvokable Members

      public bool CanInvoke()
      {
        return _scriptable != null && _dynamicMethod != null;
      }

      public object Invoke(IScriptContext context, object[] args)
      {
        context.CreateScope();
        context.SetItem("me", _scriptable.Instance);
        context.SetItem("body", _scriptable);

        var rez = RuntimeHost.NullValue;
        try
        {
          rez = _dynamicMethod.Invoke(context, _arguments);
        }
        finally
        {
          context.RemoveLocalScope();
        }

        if (rez != RuntimeHost.NullValue) return rez;
        throw new ScriptExecutionException(string.Format(Strings.DynamicObjectMethodCallError, _scriptable));
      }

      #endregion
    }
    #endregion
  }
}
