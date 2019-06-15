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

using System.Collections.Generic;
using System.Dynamic;

namespace Scripting.SSharp.Runtime
{
  public sealed class RuntimeObject : DynamicObject
  {
    private readonly Dictionary<string, object> _fields = new Dictionary<string, object>();

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
      // Converting the property name to lowercase so that property names become case-insensitive.
      string name = binder.Name.ToLower();

      // If the property name is found in a dictionary,
      // set the result parameter to the property value and return true.
      // Otherwise, return false.        
      return _fields.TryGetValue(name, out result);
    }

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
      // Converting the property name to lowercase
      // so that property names become case-insensitive.
      _fields[binder.Name.ToLower()] = value;

      // You can always add a value to a dictionary,
      // so this method always returns true.
      return true;
    }
  }
}
