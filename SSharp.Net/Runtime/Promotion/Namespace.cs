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

namespace Scripting.SSharp.Runtime.Promotion
{
  /// <summary>
  /// Represents a root for namespace
  /// </summary>
  internal class Namespace : IScriptable
  {
    /// <summary>
    /// Name of the namespace
    /// </summary>
    public string Name
    {
      get;
      set;
    }

    /// <summary>
    /// Base Contructor
    /// </summary>
    /// <param name="name"></param>
    internal Namespace(string name)
    {
      Name = name;

      if (!RuntimeHost.AssemblyManager.HasNamespace(name))
        throw new ScriptIdNotFoundException(string.Format(Strings.NamespaceNotFound, name));
    }

    public override string ToString()
    {
      return string.Format("ns:{0}", Name);
    }

    #region IScriptable Members
    [Promote(false)]
    public object Instance
    {
      get { return this; }
    }

    [Promote(false)]
    public IMemberBinding GetMember(string name, params object[] arguments)
    {
      return new NamespaceBinding(string.Format("{0}.{1}", Name, name));
    }

    [Promote(false)]
    public IBinding GetMethod(string name, params object[] arguments)
    {
      throw new NotSupportedException();
    }

    #endregion
  }
}
