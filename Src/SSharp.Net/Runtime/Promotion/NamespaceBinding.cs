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
  internal class NamespaceBinding : IMemberBinding
  {
    private readonly string _name;

    public NamespaceBinding(string name)
    {
      _name = name;
    }

    #region IMemberBind Members

    public object Target
    {
      get { throw new NotSupportedException(); }
    }

    public Type TargetType
    {
      get { throw new NotSupportedException(); }
    }

    public MemberInfo Member
    {
      get { throw new NotSupportedException(); }
    }

    public void SetValue(object value)
    {
      throw new NotSupportedException();
    }

    public object GetValue()
    {
      return RuntimeHost.HasType(_name) ? (object) RuntimeHost.GetType(_name) : new Namespace(_name);
    }

    public void AddHandler(object value)
    {
      throw new NotSupportedException();
    }

    public void RemoveHandler(object value)
    {
      throw new NotSupportedException();
    }

    #endregion

    #region IInvokable Members

    public bool CanInvoke()
    {
      throw new NotImplementedException();
    }

    public object Invoke(IScriptContext context, object[] args)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
