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
  /// <summary>
  /// Is a result of binding to Property, Field or Event of an object
  /// </summary>
  public interface IMemberBinding : IBinding
  {
    /// <summary>
    /// Object to which this member belongs
    /// </summary>
    object Target { get; }

    Type TargetType { get; }

    MemberInfo Member { get; }

    /// <summary>
    /// Sets value to the member
    /// </summary>
    /// <param name="value"></param>
    void SetValue(object value);

    /// <summary>
    /// Returns the value of a member
    /// </summary>
    /// <returns></returns>
    object GetValue();

    void AddHandler(object value);

    void RemoveHandler(object value);
  }
}
