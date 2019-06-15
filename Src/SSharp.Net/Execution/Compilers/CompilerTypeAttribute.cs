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

namespace Scripting.SSharp.Execution.Compilers
{
  // Denis Vuyka: attribute usage can be extended to support structs and interfaces as well
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  internal class CompilerTypeAttribute : Attribute
  {
    public Type NodeType { get; private set; }

    public CompilerTypeAttribute(Type nodeType)
    {
      NodeType = nodeType;
    }

    public override bool Equals(object obj)
    {
      return ((obj == this) || (((obj != null) && (obj is CompilerTypeAttribute)) && (((CompilerTypeAttribute)obj).NodeType == this.NodeType)));
    }

    public override int GetHashCode()
    {
      return NodeType.GetHashCode();
    }
  }
}
