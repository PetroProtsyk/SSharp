using System;
using System.Collections.Generic;

namespace Scripting.SSharp.Runtime
{
  /// <summary>
  /// Represents Scope. Scopes are used to store variables, types and functions
  /// </summary>
  public interface IScriptScope
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
    /// Creates reference to item with name id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    IValueReference Ref(string id);

    /// <summary>
    /// Cleans Scope (Removes items)
    /// </summary>
    /// <param name="cleanType">Type of cleanup</param>
    void Clean();

    /// <summary>
    /// Gets Invokable object (Function) by a given name
    /// </summary>
    /// <param name="name">Name</param>
    /// <returns></returns>
    IInvokable GetFunctionDefinition(string name);
  }
}
