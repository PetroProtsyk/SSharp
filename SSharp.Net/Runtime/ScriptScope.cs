using System;
using System.Collections.Generic;
using Scripting.SSharp.Parser;

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
  public class ScriptScope : IScriptScope
  {
    #region properties
    private IScriptScope parent;

    /// <summary>
    /// Parent Scope of the current scope. 
    /// Null if this scope is a global (root).
    /// </summary>
    public IScriptScope Parent
    {
      get { return parent; }
      set { parent = value; }
    }

    Dictionary<string, IValueReference> vars = new Dictionary<string, IValueReference>();
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
    /// <returns>Value of the variable</returns>
    public virtual object GetItem(string id, bool throwException)
    {
      object result = GetVariableInternal(id, true);

      if (result == RuntimeHost.NoVariable && throwException)
      {
        throw new ScriptIdNotFoundException(id + " is not found");
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
      return vars.ContainsKey(id);
    }

    /// <summary>
    /// Searches the scope hierarchy for the given id,
    /// should return NoVariable if it is not found
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    protected virtual object GetVariableInternal(string id, bool searchHierarchy)
    {
      IValueReference result;
      if (vars.TryGetValue(id, out result))
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
    /// <param name="contextItemType">type of item</param>
    /// <param name="value">value</param>
    public virtual void SetItem(string id, object value)
    {
      IValueReference reference = value as IValueReference;
      if (reference != null)
      {
        if (vars.ContainsKey(id))
          vars[id] = reference;
        else
          vars.Add(id, reference);

        return;
      }

      if (vars.ContainsKey(id))
      {
        vars[id].Value = value;
      }
      else
      {
        vars.Add(id, new ValueReference(id, value) { Scope = this });
      }
    }

    public virtual IValueReference Ref(string id)
    {
      if (!HasVariable(id)) throw new ScriptIdNotFoundException(id);

      return vars[id];
    }

    /// <summary>
    /// Cleans Scope (Removes items)
    /// </summary>
    /// <param name="cleanType">Type of cleanup</param>
    public virtual void Clean()
    {
      foreach (IValueReference vr in vars.Values)
        vr.Remove();
            
      vars.Clear();
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

      if (parent != null)
        return parent.GetFunctionDefinition(name);
      else
        throw new ScriptIdNotFoundException("Function " + name + " not found");
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
