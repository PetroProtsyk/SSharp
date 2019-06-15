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
  /// Base implementation of ScriptContext object
  /// </summary>
  public class ScriptContext : IScriptContext
  {
    #region Fields
    private ContextFlags _flags = ContextFlags.Empty;
    #endregion

    #region Properties
    /// <summary>
    /// Scope object
    /// </summary>
    public IScriptScope Scope
    {
      get;
      private set;
    }

    public Script Owner
    {
      get;
      private set;
    }

    /// <summary>
    /// Script Result object
    /// </summary>
    public object Result
    {
      get;
      set;
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
    public IScriptScope CreateScope()
    {
      Scope = RuntimeHost.ScopeFactory.Create(ScopeTypes.Default, Scope);
      return Scope;
    }

    /// <summary>
    /// Replace existing scope with new one
    /// </summary>
    /// <param name="scope"></param>
    public IScriptScope CreateScope(IScriptScope scope)
    {
      if (scope.Parent != Scope)
        throw new ScriptRuntimeException(Strings.ScopeParentIsNotValid);
      Scope = scope;
      return Scope;
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
        scopeToRemove.Dispose();
      }
      else
        throw new Exception("Can't remove global scope, use Scope.Clean");
    }

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
      var args = new ReferencingEventArgs(false, null);
      if (OnReferencing(args)) return args.Ref;

      var scope = Scope;

      while (scope != null)
      {
        //TODO: Figure out more maintainable solution
        if (scope is FunctionScope) return null;

        if (scope.HasVariable(id))
        {
          args = new ReferencingEventArgs(false, scope.Ref(id));
          if (args.Ref != null && OnReferenced(args))
          {
            if (Owner != null)
            {
              Owner.NotifyReferenceCreated(args.Ref);
            }

            return args.Ref;
          }
          return null;
        }
        scope = scope.Parent;
      }

      return null;
    }

    public event EventHandler<ReferencingEventArgs> Referencing;
  
    protected virtual bool OnReferencing(ReferencingEventArgs args)
    {
      if (Referencing != null)
        Referencing.Invoke(this, args);

      return args.Cancel;
    }

    public event EventHandler<ReferencingEventArgs> Referenced;

    protected virtual bool OnReferenced(ReferencingEventArgs args)
    {
      if (Referenced != null)
        Referenced.Invoke(this, args);

      return !args.Cancel;
    }
    #endregion

    #region Break-Continue-Return
    /// <summary>
    /// Set return state of run-time
    /// </summary>
    /// <param name="val">true or false</param>
    public void SetReturn(bool val)
    {
      if (val && IsContinue()) throw new ScriptRuntimeException(Strings.ContextHasCorruptedFlagsError);
      if (val && IsBreak()) throw new ScriptRuntimeException(Strings.ContextHasCorruptedFlagsError);

      if (val)
        _flags = _flags | ContextFlags.Return;
      else
        _flags = _flags & ~ContextFlags.Return;
    }

    /// <summary>
    /// Set break state of run-time
    /// </summary>
    /// <param name="val">true or false</param>
    public void SetBreak(bool val)
    {
      if (val && IsContinue()) throw new ScriptRuntimeException(Strings.ContextHasCorruptedFlagsError);

      if (val)
        _flags = _flags | ContextFlags.Break;
      else
        _flags = _flags & ~ContextFlags.Break;
    }

    /// <summary>
    /// Set continue state of run-time
    /// </summary>
    /// <param name="val">true or false</param>
    public void SetContinue(bool val)
    {
      if (val && IsBreak()) throw new ScriptRuntimeException(Strings.ContextHasCorruptedFlagsError);

      if (val)
        _flags = _flags | ContextFlags.Continue;
      else
        _flags = _flags & ~ContextFlags.Continue;
    }

    /// <summary>
    /// Reset all flags that control execution. Called on each context 
    /// before and after script execution
    /// </summary>
    public void ResetControlFlags()
    {
      _flags = ContextFlags.Empty;
    }

    /// <summary>
    /// Return state
    /// </summary>
    /// <returns>true or false</returns>
    public bool IsReturn()
    {
      return (_flags & ContextFlags.Return) == ContextFlags.Return;
    }

    /// <summary>
    /// Break state
    /// </summary>
    /// <returns>true or false</returns>
    public bool IsBreak()
    {
      return (_flags & ContextFlags.Break) == ContextFlags.Break;
    }

    /// <summary>
    /// Continue state
    /// </summary>
    /// <returns>true or false</returns>
    public bool IsContinue()
    {
      return (_flags & ContextFlags.Continue) == ContextFlags.Continue;
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
      return Scope.GetFunctionDefinition(name);
    }
    #endregion

    #region Internal Methods
    internal void SetOwner(Script owner)
    {
      //Remove context from previous owner
      if (Owner != null && owner != null && Owner != owner)
        Owner.Context = null;

      Owner = owner;
    }
    #endregion

    #region IDisposable Members
    private bool _disposed;
    protected bool Disposed
    {
      get
      {
        lock (this)
        {
          return _disposed;
        }
      }
    }

    public void Dispose()
    {
      lock (this)
      {
        if (_disposed == false)
        {
          Cleanup();
          _disposed = true;
          GC.SuppressFinalize(this);
        }
      }
    }

    ~ScriptContext()
    {
      Cleanup();
    }

    protected virtual void Cleanup()
    {
      IScriptScope s = Scope;
      while (s != null)
      {
        IScriptScope toRemove = s;
        s = s.Parent;
        toRemove.Dispose();
      }

      Result = null;
      Scope = null;
    }
    #endregion
  }

  /// <summary>
  /// Specify context state
  /// </summary>
  [Flags]
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

  public class ReferencingEventArgs : EventArgs
  {
    public bool Cancel { get; set; }

    public IValueReference Ref { get; set; }

    public ReferencingEventArgs() : this(false, null)
    {
    }

    public ReferencingEventArgs(bool cancel, IValueReference reference)
    {
      Cancel = cancel;
      Ref = reference;
    }
  }
}
