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


namespace Scripting.SSharp.Runtime
{
  internal class ScopeValueReference
  {
    IScriptScope Scope { get; set; }
    public string Id { get; private set; }

    public object Value
    {
      get
      {
        return Scope.GetItem(Id, true);
      }
      set
      {
        Scope.SetItem(Id, value);
      }
    }

    public object ConvertedValue { get; set; }

    public ScopeValueReference(IScriptScope scope, string id)
    {
      Scope = scope;
      Id = id;
    }
  }
}
