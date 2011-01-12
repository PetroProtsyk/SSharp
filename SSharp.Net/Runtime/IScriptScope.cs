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

namespace Scripting.SSharp.Runtime
{
  /// <summary>
  /// Represents Scope. Scopes are used to store variables, types and functions
  /// </summary>
  public interface IScriptScope : IDisposable
  {
    /// <summary>
    /// Parent scope
    /// </summary>
    IScriptScope Parent { get; }

    /// <summary>
    /// Returns Item: variable, type or function
    /// </summary>
    /// <param name="id">id of item</param>
    /// <param name="throwException">throws exception if item not found</param>
    /// <returns>value of given ID</returns>
    object GetItem(string id, bool throwException);

    /// <summary>
    /// Sets Item: variable, type or function
    /// </summary>
    /// <param name="id">item's id</param>
    /// <param name="value">value</param>
    void SetItem(string id, object value);

    /// <summary>
    /// Returns true if excatly this scope has variable with given id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    bool HasVariable(string id);

    /// <summary>
    /// Creates reference to item with name id for caching. 
    /// Note: It should not be called directly, only through ScriptContext
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    IValueReference Ref(string id);

    /// <summary>
    /// Gets Invokable object (Function) by a given name
    /// </summary>
    /// <param name="name">Name</param>
    /// <returns></returns>
    IInvokable GetFunctionDefinition(string name);
  }
}
