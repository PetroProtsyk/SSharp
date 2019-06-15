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
  /// <summary>
  /// Objects implementing this interface may be called from the script
  /// just like usual functions.
  /// </summary>
  public interface IInvokable
  {
    /// <summary>
    /// Indicates wether Invoke could be called
    /// </summary>
    /// <returns>boolean value</returns>
    bool CanInvoke();

    /// <summary>
    /// Executes call to the object.
    /// </summary>
    /// <param name="context">Current execution context</param>
    /// <param name="args">Arguments or empty list. Prefer passing empty array instead of null.</param>
    /// <returns>execution result</returns>
    object Invoke(IScriptContext context, object[] args);
  }
}
