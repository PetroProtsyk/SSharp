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

using Scripting.SSharp.Runtime.Promotion;

namespace Scripting.SSharp.Runtime
{
  /// <summary>
  /// Expose dynamic members of an Instance to the script.
  /// This require using of DefaultObjectBinder class as default object binder.
  /// </summary>
  public interface IScriptable
  {
    /// <summary>
    /// Should return object wrapped by IScriptable or this
    /// </summary>
    [Promote(false)]
    object Instance { get; }

    /// <summary>
    /// Gets a binding to an instance's member (field, property)
    /// </summary>
    [Promote(false)]
    IMemberBinding GetMember(string name, params object[] arguments);

    /// <summary>
    /// Gets a binding to an instance's method
    /// </summary>
    [Promote(false)]
    IBinding GetMethod(string name, params object[] arguments);
  }
}
