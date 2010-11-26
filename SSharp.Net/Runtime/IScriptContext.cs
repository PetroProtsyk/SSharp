using System;

namespace Scripting.SSharp.Runtime
{
  /// <summary>
  /// Base interface for Script Context obect.
  /// ScriptContext object stores run-time information during script's execution
  /// This information containts:
  ///    Scopes - which stores variables, types and functions
  ///    Execution Flow Flags - break, return, continue
  ///    
  /// ScriptContext objects also evaluates operators
  /// </summary>
  public interface IScriptContext : IDisposable
  {
    #region Scopes
    /// <summary>
    /// Create scope
    /// </summary>
    IScriptScope CreateScope();
    /// <summary>
    /// Add given scope to hierarchy
    /// </summary>
    /// <param name="scope">new scope</param>
    IScriptScope CreateScope(IScriptScope scope);
    /// <summary>
    /// Removes local scope
    /// </summary>
    void RemoveLocalScope();
    /// <summary>
    /// Current scope
    /// </summary>
    IScriptScope Scope { get; }

    /// <summary>
    /// Returns item from scope hierarchy
    /// </summary>
    /// <param name="id">name</param>
    /// <param name="throwException"></param>
    /// <returns>value</returns>
    object GetItem(string id, bool throwException);

    /// <summary>
    /// Sets item to scope hierarchy
    /// </summary>
    /// <param name="id">name</param>
    /// <param name="value">value</param>
    void SetItem(string id, object value);

    /// <summary>
    /// Create reference to an exising variable in scope hierarchy
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    IValueReference Ref(string id);
  
    /// <summary>
    /// Occurs before referencing process, giving ability to cancel it and return custom result
    /// </summary>
    event EventHandler<ReferencingEventArgs> Referencing;

    /// <summary>
    /// Occurs when referencing successful just before returning value
    /// </summary>
    event EventHandler<ReferencingEventArgs> Referenced;

    /// <summary>
    /// Finds function definition
    /// </summary>
    /// <param name="name">name</param>
    /// <returns>function object</returns>
    IInvokable GetFunctionDefinition(string name);
    #endregion

    #region Break-Continue-Return
    void SetReturn(bool val);
    void SetBreak(bool val);
    void SetContinue(bool val);
    bool IsReturn();
    bool IsBreak();
    bool IsContinue();
    #endregion

    /// <summary>
    /// Result of script execution
    /// </summary>
    object Result
    {
      get;
      set;
    }

    Script Owner
    {
      get;
    }
  }
}
