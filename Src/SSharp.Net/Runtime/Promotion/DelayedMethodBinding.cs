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
using System.Reflection;

namespace Scripting.SSharp.Runtime.Promotion
{
  internal class DelayedMethodBinding : IBinding
  {
    public string MethodName { get; private set; }
    public object Target { get; private set; }
    public MethodInfo MethodInfo { get; private set; }

    public DelayedMethodBinding(string methodName, object target)
    {
      MethodName = methodName;
      Target = target;
    }

    public DelayedMethodBinding(MethodInfo methodInfo, object target)
    {
      MethodInfo = methodInfo;
      MethodName = methodInfo.Name;
      Target = target;
    }

    #region IInvokable Members

    public bool CanInvoke()
    {
      return Target != null;
    }

    public virtual object Invoke(IScriptContext context, object[] args)
    {
      var result = RuntimeHost.NullValue;

      IBinding bind;

      if (MethodInfo == null)
        bind = RuntimeHost.Binder.BindToMethod(Target, MethodName, null, args);
      else
        bind = RuntimeHost.Binder.BindToMethod(Target, MethodInfo, args);

      if (bind != null)
        result = bind.Invoke(context, null);
      else
        throw new ScriptMethodNotFoundException(MethodName);

      context.Result = result;
      return result;
    }
    #endregion

#if !PocketPC && !SILVERLIGHT
    //TODO: Review this approach
    public static implicit operator IntPtr(DelayedMethodBinding invokableMethod)
    {
      if (invokableMethod == null) throw new ArgumentNullException();
      if (invokableMethod.MethodName == null) throw new NotSupportedException();
      if (invokableMethod.Target == null) throw new NotSupportedException();

      return invokableMethod.Target.GetType().GetMethod(invokableMethod.MethodName).MethodHandle.GetFunctionPointer();
    }
#endif
  }
}
