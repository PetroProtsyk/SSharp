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
  /// Public interface for Object Bind. It is used to bind arguments to:
  /// * indexers
  /// * constructors
  /// * methods
  /// * interfaces
  /// </summary>
  public interface IObjectBinding
  {
    /// <summary>
    /// Binds to constructor
    /// </summary>
    /// <param name="target"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    IBinding BindToConstructor(Type target, object[] arguments);

    /// <summary>
    /// Binds to method
    /// </summary>
    /// <param name="target"></param>
    /// <param name="methodName"></param>
    /// <param name="genericParameters"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    IBinding BindToMethod(object target, string methodName, Type[] genericParameters, object[] arguments);

    /// <summary>
    /// Binds to method
    /// </summary>
    /// <param name="target"></param>
    /// <param name="method"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    IBinding BindToMethod(object target, MethodInfo method, object[] arguments);

    /// <summary>
    /// Binds to indexer
    /// </summary>
    /// <param name="target"></param>
    /// <param name="arguments"></param>
    /// <param name="setter"></param>
    /// <returns></returns>
    IBinding BindToIndex(object target, object[] arguments, bool setter);
    
    /// <summary>
    /// Binds to Field, Property of Event of a given object
    /// </summary>
    /// <param name="target"></param>
    /// <param name="memberName"></param>
    /// <returns></returns>
    IMemberBinding BindToMember(object target, string memberName, bool throwNotFound);
        
    /// <summary>
    /// Converts value to target type
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <returns></returns>
    object ConvertTo(object value, Type targetType);

    /// <summary>
    /// Evaluates if object binder could run binding procedure for the given member.
    /// <b>Default</b> object binders uses BindableAttribute.
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    bool CanBind(MemberInfo member);
  }
}
