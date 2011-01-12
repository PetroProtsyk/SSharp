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

namespace Scripting.SSharp.Runtime
{
  using Configuration;
using System.Collections.Generic;

  public interface IAssemblyManager : IDisposable
  {
    void Initialize(ScriptConfiguration configuration);

    void AddAssembly(Assembly assembly);

    void RemoveAssembly(Assembly assembly);

    Type GetType(string name);

    IEnumerable<MethodInfo> GetExtensionMethods(Type type);

    IEnumerable<MethodInfo> GetExtensionMethods(Type type, string methodName);

    bool HasType(string name);

    bool HasNamespace(string name);

    void AddType(string alias, Type type);

    event EventHandler<AssemblyHandlerEventArgs> BeforeAddAssembly;

    event EventHandler<AssemblyTypeHandlerEventArgs> BeforeAddType;
  }

  public class AssemblyTypeHandlerEventArgs : EventArgs
  {
    public bool Cancel { get; set; }

    public Assembly Assembly { get; private set; }

    public Type Type { get; private set; }

    public AssemblyTypeHandlerEventArgs(Assembly assembly, Type type)
    {
      Cancel = false;
      Assembly = assembly;
      Type = type;
    }
  }

  public class AssemblyHandlerEventArgs : EventArgs
  {
    public bool Cancel { get; set; }

    public Assembly Assembly { get; private set; }

    public AssemblyHandlerEventArgs(Assembly assembly)
    {
      Cancel = false;
      Assembly = assembly;
    }
  }
}
