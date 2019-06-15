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

namespace Scripting.SSharp.Runtime.Operators
{
  /// <summary>
  /// Base interface for all Operators
  /// </summary>
  public interface IOperator
  {
    /// <summary>
    /// Operator symbol: +,-,/,++, etc.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Indicates unarity of the operator
    /// </summary>
    bool Unary { get; }

    /// <summary>
    /// should be used by unary operator
    /// </summary>
    /// <param name="value"></param>
    /// <returns>result or throws exception in case Unary=false</returns>
    object Evaluate(object value);

    /// <summary>
    /// should be used by unary operator
    /// </summary>
    /// <returns>result or throws exception in case Unary=true</returns>
    object Evaluate(object left, object right);
  }
}
