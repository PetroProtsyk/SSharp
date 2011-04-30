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
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace Scripting.SSharp.Runtime
{
  /// <summary>
  /// Script Scope represents a tree of local scopes.
  /// Scopes stores variables and types tables.
  /// 
  /// Run-time queries ScriptScope through ScriptContext for:
  /// <list type="bullet">
  ///   <item>Resolving names of types and variables;</item>
  ///   <item>Resolving names of functions;</item>
  ///   <item>Adding new function into scope;</item>
  ///   <item>Assigning values to variables.</item>
  /// </list>
  /// </summary>
  [DebuggerDisplay("Scope, Parent={Parent}")]
  [DebuggerTypeProxy(typeof(ScriptScopeDebugViewer))]
  public class ScriptScope : IScriptScope
  {
    #region properties

    /// <summary>
    /// Parent Scope of the current scope. 
    /// Null if this scope is a global (root).
    /// </summary>
    public IScriptScope Parent { get; set; }

    Dictionary<string, IValueReference> _vars = new Dictionary<string, IValueReference>();
    #endregion

    #region constructors
    /// <summary>
    /// Default Constructor
    /// </summary>
    public ScriptScope():
        this(null)
    {
    }

    public ScriptScope(IScriptScope parent)
    {
      Parent = parent;
    }
    #endregion

    #region IScriptScope

    /// <summary>
    /// Returns value of the variable. Throws ScriptIdNotFoundException
    /// </summary>
    /// <param name="id">Variable ID</param>
    /// <param name="throwException"></param>
    /// <returns>Value of the variable</returns>
    public virtual object GetItem(string id, bool throwException)
    {
      object result = GetVariableInternal(id, true);

      if (result == RuntimeHost.NoVariable && throwException)
      {
        throw new ScriptIdNotFoundException(string.Format(Strings.VariableNotFound, id));
      }

      return result;
    }

    /// <summary>
    /// Returns true if excatly this scope has variable with given id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public virtual bool HasVariable(string id)
    {
      return _vars.ContainsKey(id);
    }

    /// <summary>
    /// Searches the scope hierarchy for the given id,
    /// should return NoVariable if it is not found
    /// </summary>
    /// <param name="id"></param>
    /// <param name="searchHierarchy"></param>
    /// <returns></returns>
    protected virtual object GetVariableInternal(string id, bool searchHierarchy)
    {
      IValueReference result;
      if (_vars.TryGetValue(id, out result))
      {
        return result.Value;
      }

      if (!searchHierarchy) return RuntimeHost.NoVariable;

      IScriptScope scope = Parent;
      while(scope != null)
      {
        if (scope.HasVariable(id))
          return scope.GetItem(id, true);

        scope = scope.Parent;
      }

      return RuntimeHost.NoVariable;
    }

    /// <summary>
    /// Sets Item: variable or type
    /// </summary>
    /// <param name="id">item's id</param>
    /// <param name="value">value</param>
    public virtual void SetItem(string id, object value)
    {
      IValueReference reference = value as IValueReference;
      if (reference != null)
      {
        _vars[id] = reference;

        //if (_vars.ContainsKey(id))
        //  _vars[id] = reference;
        //else
        //  _vars.Add(id, reference);

        return;
      }

      if (_vars.TryGetValue(id, out reference))
      {
        reference.Value = value;
      }
      else
      {
        _vars.Add(id, new ValueReference(id, value) { Scope = this });
      }
    }

    public virtual IValueReference Ref(string id)
    {
      if (!_vars.ContainsKey(id)) throw new ScriptIdNotFoundException(id);

      return _vars[id];
    }

    #endregion

    #region Functions
    /// <summary>
    /// Gets Invokable object (Function) by a given name
    /// </summary>
    /// <param name="name">Name</param>
    /// <returns></returns>
    public virtual IInvokable GetFunctionDefinition(string name)
    {
      object result = GetVariableInternal(name, false);

      IInvokable function = result as IInvokable;
      if (function != null) return function;

      if (Parent != null)
        return Parent.GetFunctionDefinition(name);
      else
        throw new ScriptIdNotFoundException(string.Format(Strings.FunctionNotFound, name));
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

    ~ScriptScope()
    {
      Cleanup();
    }

    protected virtual void Cleanup()
    {
      if (_vars != null)
      {
        foreach (IValueReference vr in _vars.Values)
          vr.Remove();

        _vars.Clear();
        _vars = null;
      }

      Parent = null;
    }
    #endregion

    #region VS Debugging
    private class ScriptScopeDebugViewer
    {
      private readonly ScriptScope _scope;

      public ScriptScopeDebugViewer(ScriptScope s)
      {
        _scope = s;
      }

      [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
      public IValueReference[] Variables
      {
        get
        {
          return _scope == null ? new IValueReference[0] : _scope._vars.Select(v => v.Value).ToArray();
        }
      }

      private string FormatValue(IValueReference iValueReference)
      {
        if (iValueReference == null) return "Null";
        if (iValueReference.Value == null) return "Null";

        return iValueReference.Value.ToString();
      }
    }
    #endregion
  }

  /// <summary>
  /// Default scope activator - used by framework to create ScriptScope by default
  /// may be overriden in xml configuration
  /// </summary>
  public class ScriptScopeActivator : IScopeActivator
  {
    #region IScopeActivator Members

    public IScriptScope Create(IScriptScope parent, params object[] args)
    {
      if (args == null || args.Length == 0)
      {
        ScriptScope result = new ScriptScope(parent);
        SetBaseItems(result);
        return result;
      }

      throw new NotSupportedException();
    }

    private void SetBaseItems(ScriptScope result)
    {
      //Process only root scopes
      if (result.Parent != null) return;        

      //Variables
      result.SetItem("Scope", result);
      result.SetItem("Compiler", RuntimeHost.Parser);

      ////Custom Functions
      //AppendAst
      result.SetItem(CustomFunctions.AppendFunc.FunctionName, CustomFunctions.AppendFunc.FunctionDefinition);
      //ReplaceAst
      result.SetItem(CustomFunctions.ReplaceFunc.FunctionName, CustomFunctions.ReplaceFunc.FunctionDefinition);
      //eval
      result.SetItem(CustomFunctions.EvalFunc.FunctionName, CustomFunctions.EvalFunc.FunctionDefinition);
      //Console
      result.SetItem(CustomFunctions.RunConsole.FunctionName, CustomFunctions.RunConsole.FunctionDefinition);
      //Array
      result.SetItem(CustomFunctions.ArrayFunc.FunctionName, CustomFunctions.ArrayFunc.FunctionDefinition);
      //Char
      result.SetItem(CustomFunctions.CharFunc.FunctionName, CustomFunctions.CharFunc.FunctionDefinition);
    }
    #endregion
  }
}
