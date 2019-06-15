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

namespace Scripting.SSharp.Execution.Compilers.Dom
{
  internal class CodeObjectReference : CodeExpression
  {
    public string Id { get; set; }

    public CodeObject Next { get; set; }

    public List<CodeObject> Modifiers { get; private set; }

    public CodeObjectReference(string id, CodeObject next, IEnumerable<CodeObject> modifiers)
    {
      Id = id;
      Modifiers = new List<CodeObject>();
      if (modifiers != null)
        Modifiers.AddRange(modifiers);
      Next = next;
    }
  }
}
