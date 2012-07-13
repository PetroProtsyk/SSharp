using System;

namespace Scripting.SSharp.Runtime
{
  /// <summary>
  /// Base implementation of ScriptContext object
  /// </summary>
  public class ScriptContext : IScriptContext
  {
    #region Properties
    private IScriptScope scope;

    /// <summary>
    /// Scope object
    /// </summary>
    public IScriptScope Scope
    {
      get { return scope; }
      private set { scope = value; }
    }

    /// <summary>
    /// Script source code. Should be provided before executing the script
    /// </summary>
    public string SourceCode { get; private set; }

    private ContextFlags flags = ContextFlags.Empty;

    bool IsSkip
    {
      get
      {
        return IsBreak() || IsContinue() || IsReturn();
      }
    }

    private object result;

    /// <summary>
    /// Script Result object
    /// </summary>
    public object Result
    {
      get { return result; }
      set { result = value; }
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Creates new Script Context with Default scope
    /// </summary>
    public ScriptContext()
    {      
      CreateScope();
    }
    #endregion

    #region Scope
    /// <summary>
    /// Creates new default nested scope
    /// </summary>
    public void CreateScope()
    {
      Scope = RuntimeHost.ScopeFactory.Create(ScopeTypes.Default, Scope);
    }

    /// <summary>
    /// Replace existing scope with new one
    /// </summary>
    /// <param name="scope"></param>
    public void CreateScope(IScriptScope scope)
    {
      if (scope.Parent != Scope)
        throw new ScriptException("Wrong scope structure");
      Scope = scope;
    }

    /// <summary>
    /// Remove Local Scope
    /// </summary>
    public void RemoveLocalScope()
    {
      if (Scope.Parent != null)
      {
        IScriptScope scopeToRemove = Scope;
        Scope = Scope.Parent;
        scopeToRemove.Clean();
      }
      else
        throw new Exception("Can't remove global scope, use Scope.Clean");
    }

    #region IScriptScope     
    public object GetItem(string id, bool throwException)
    {
      return Scope.GetItem(id, throwException);
    }

    public void SetItem(string id, object value)
    {
        Scope.SetItem(id, value);
    }

    public IValueReference Ref(string id)
    {
      IScriptScope scope = Scope;

      while (scope != null)
      {
        if (scope.HasVariable(id)) return scope.Ref(id);
        scope = scope.Parent;
      }

      return null;
    }
    #endregion
    #endregion

    #region Break-Continue-Return
    /// <summary>
    /// Set return state of run-time
    /// </summary>
    /// <param name="val">true or false</param>
    public void SetReturn(bool val)
    {
      if (val && IsContinue()) throw new ScriptException("Implementation: Implementation error, consult with developer");
      if (val && IsBreak()) throw new ScriptException("Implementation: Implementation error, consult with developer");

      if (val)
        flags = flags | ContextFlags.Return;
      else
        flags = flags & ~ContextFlags.Return;
    }

    /// <summary>
    /// Set break state of run-time
    /// </summary>
    /// <param name="val">true or false</param>
    public void SetBreak(bool val)
    {
      if (val && IsContinue()) throw new ScriptException("Implementation: Implementation error, consult with developer");

      if (val)
        flags = flags | ContextFlags.Break;
      else
        flags = flags & ~ContextFlags.Break;
    }

    /// <summary>
    /// Set continue state of run-time
    /// </summary>
    /// <param name="val">true or false</param>
    public void SetContinue(bool val)
    {
      if (val && IsBreak()) throw new ScriptException("Implementation: Implementation error, consult with developer");

      if (val)
        flags = flags | ContextFlags.Continue;
      else
        flags = flags & ~ContextFlags.Continue;
    }

    /// <summary>
    /// Return state
    /// </summary>
    /// <returns>true or false</returns>
    public bool IsReturn()
    {
      return (flags & ContextFlags.Return) == ContextFlags.Return;
    }

    /// <summary>
    /// Break state
    /// </summary>
    /// <returns>true or false</returns>
    public bool IsBreak()
    {
      return (flags & ContextFlags.Break) == ContextFlags.Break;
    }

    /// <summary>
    /// Continue state
    /// </summary>
    /// <returns>true or false</returns>
    public bool IsContinue()
    {
      return (flags & ContextFlags.Continue) == ContextFlags.Continue;
    }
    #endregion

    #region Function Defs
    /// <summary>
    /// Finds function definition in current scope
    /// </summary>
    /// <param name="name">function name</param>
    /// <returns>IInvokable object</returns>
    public IInvokable GetFunctionDefinition(string name)
    {
      return scope.GetFunctionDefinition(name);
    }
    #endregion
  }

  /// <summary>
  /// Specify context state
  /// </summary>
  [Flags()]
  public enum ContextFlags
  {
    /// <summary>
    /// Initial state
    /// </summary>
    Empty = 0,
    /// <summary>
    /// Brake operator executed
    /// </summary>
    Break = 2,
    /// <summary>
    /// Continue operator executed
    /// </summary>    
    Continue = 4,
    /// <summary>
    /// Return statement executed
    /// </summary>
    Return = 8
  }

}
