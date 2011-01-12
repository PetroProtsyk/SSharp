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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Runtime.Promotion;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Qualified Name
  /// </summary>
  internal class ScriptQualifiedName : ScriptExpr
  {
    #region Members
    private readonly string _identifier;
    private readonly List<ScriptAst> _modifiers;
    private readonly ScriptQualifiedName _namePart;

    private delegate void EvaluateFunction(IScriptContext context);
    private delegate void AssignFunction(object value, IScriptContext context);

    private readonly EvaluateFunction _evaluation;
    private readonly AssignFunction _assignment;

    public ReadOnlyCollection<ScriptAst> Modifiers
    {
      get
      {
        return new ReadOnlyCollection<ScriptAst>(_modifiers);
      }
    }

    public ScriptQualifiedName NextPart
    {
      get
      {
        return _namePart;
      }
    }

    internal bool NextFirst
    {
      get;
      private set;
    }

    public string Identifier
    {
      get
      {
        return _identifier;
      }
    }

    /// <summary>
    /// Identifies that node represents single variable
    /// </summary>
    public bool IsVariable
    {
      get;
      private set;
    }

    public bool IsGlobal
    {
      get;
      private set;
    }

    public bool IsVar
    {
      get;
      internal set;
    }
    #endregion

    #region Constructor
    public ScriptQualifiedName(AstNodeArgs args)
      : base(args)
    {
      IsVariable = false;
      NextFirst = false;
      IsVar = false;

      if (ChildNodes.Count == 2 && ChildNodes[1].ChildNodes.Count == 0)
      {
        _identifier = ExtractId(((TokenAst)ChildNodes[0]).Text);
        IsVariable = true;

        _evaluation = EvaluateIdentifier;
        _assignment = AssignIdentifier;
      }
      else
        if (ChildNodes[0] is TokenAst && ChildNodes[1].ChildNodes.Count != 0)
        {
          _identifier = ExtractId(((TokenAst)ChildNodes[0]).Text);

          //NOTE: There might be two cases:
          //      1) a()[]...() 
          //      2) a<>.(NamePart)    
          _modifiers = new List<ScriptAst>();
          foreach (ScriptAst node in ChildNodes[1].ChildNodes)
            _modifiers.Add(node);

          var generic = _modifiers.FirstOrDefault() as ScriptGenericsPostfix;
          if (generic != null && _modifiers.Count == 1)
          {
            //Case 2
            _evaluation = EvaluateGenericType;
            _assignment = null;
          }
          else
          {
            //Case 1
            _evaluation = EvaluateFunctionCall;
            _assignment = AssignArray;
          }
        }
        else
        {
          _namePart = ChildNodes[0] as ScriptQualifiedName;
          _identifier = ExtractId(((TokenAst)ChildNodes[2]).Text);
          NextFirst = true;
          if (ChildNodes.Count == 4 && ChildNodes[3].ChildNodes.Count != 0)
          {
            _modifiers = new List<ScriptAst>();
            foreach (ScriptAst node in ChildNodes[3].ChildNodes)
            {
              _modifiers.Add(node);
            }
          }
          _evaluation = EvaluateNamePart;
          _assignment = AssignNamePart;
        }
    }
    #endregion

    #region Public Methods
    public override void Evaluate(IScriptContext context)
    {
      _evaluation(context);
    }

    public void Assign(object value, IScriptContext context)
    {
      _assignment(value, context);
    }
    #endregion

    #region Identifier
    IValueReference _variable;
    
    private void EvaluateIdentifier(IScriptContext context)
    {
      object result;

      // if variable was previously cached and it still belongs to the given scope return the cached value
      if (_variable != null && _variable.Scope.Equals(context.Scope))
      {
        result = _variable.Value;
      }
      else
      {
        result = GetIndentifierValue(context, _identifier);
      }

      context.Result = result;
    }

    private object GetIndentifierValue(IScriptContext context, string identifier)
    {
      //object result = context.GetItem(identifier, false);
      //if (result != RuntimeHost.NoVariable) return result;

      if (IsGlobal)
      {
        _variable = null;
        IScriptScope scope = context.Scope.Parent;
        while (scope != null)
        {
          if (scope.HasVariable(identifier))
          {
            return scope.GetItem(identifier, true);
          }
          scope = scope.Parent;
        }
      }
      else
      {
        if (_variable != null && _variable.Value != null) return _variable.Value;

        object result;         
        _variable = CreateRef(identifier, context, true, out result);

        if (result != RuntimeHost.NoVariable)
        {
          return result;
        }
      }

      return RuntimeHost.HasType(identifier)
               ? (object) RuntimeHost.GetType(identifier)
               : NamespaceResolver.Get(identifier);
    }

    private void AssignIdentifier(object value, IScriptContext context)
    {
      if (IsGlobal)
      {
        SetToParentScope(context.Scope.Parent, _identifier, value);
        _variable = null;
        return;
      }

      var scope = context.Scope as LocalScope;
      if (IsVar && scope != null)
      {
        scope.CreateVariable(_identifier, value);
        return;
      }

      if (_variable != null)
      {
        _variable.Value = value;
      }
      else
      {
        context.SetItem(_identifier, value);
        
        object tmp;
        _variable = CreateRef(_identifier, context, false, out tmp);
      }
    }

    private IValueReference CreateRef(string id, IScriptContext context, bool resolve, out object value)
    {
      IValueReference result = context.Ref(id);
      value = RuntimeHost.NoVariable;

      if (result != null)
      {
        result.Removed += ReferenceRemoved;

        if (resolve)
          value = result.Value;
      }
      else
      {
        if (resolve)
          value = context.GetItem(_identifier, false);
      }

      return result;
    }

    private void ReferenceRemoved(object sender, EventArgs e)
    {
      if (sender == _variable)
      {
        _variable.Removed -= ReferenceRemoved;
        _variable = null;
      }
    }

    /// <summary>
    /// Sets variable to the first scope in hierarchy which already has this variable
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="id"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    internal static void SetToParentScope(IScriptScope parent, string id, object value)
    {
      var scope = parent;
      while (scope != null)
      {
        if (scope.HasVariable(id))
        {
          scope.SetItem(id, value);
          return;
        }
        scope = scope.Parent;
      }

      throw new ScriptIdNotFoundException(string.Format(Strings.GlobalNameNotFound, id));
    }
    #endregion

    #region Single Call
    private void EvaluateGenericType(IScriptContext context)
    {
      var genericPostfix = (ScriptGenericsPostfix)_modifiers.First();

      var genericType = GetIndentifierValue(context, genericPostfix.GetGenericTypeName(_identifier)) as Type;
      if (genericType == null || !genericType.IsGenericType)
      {
        throw new ScriptExecutionException(string.Format(Strings.TypeIsNotGeneric, Code(context), genericType.Name));
      }

      genericPostfix.Evaluate(context);
      context.Result = genericType.MakeGenericType((Type[])context.Result);
    }

    private void EvaluateFunctionCall(IScriptContext context)
    {
      EvaluateIdentifier(context);

      foreach (var node in _modifiers)
      {
        var funcCall = node as ScriptFunctionCall;
        if (funcCall != null)
        {
          var function = context.Result as IInvokable;
          if (function == null)
            throw new ScriptExecutionException(string.Format(Strings.ObjectDoesNotImplementIInvokable, Code(context), node.Code(context), Span.Start.Line, Span.Start.Position));
          context.Result = CallFunction(function, funcCall, context);
          continue;
        }

        var arrayResolution = node as ScriptArrayResolution;
        if (arrayResolution != null)
        {
          GetArrayValue(context.Result, arrayResolution, context);
          continue;
        }

        var genericPostfix = node as ScriptGenericsPostfix;
        if (genericPostfix != null)
        {
          throw new NotSupportedException();
          //genericPostfix.Evaluate(Context);
          //continue;
        }

      }
    }

    private void AssignArray(object value, IScriptContext context)
    {
      var obj = context.GetItem(_identifier, true);

      foreach (var node in _modifiers)
      {
        var functionCall = node as ScriptFunctionCall;
        if (functionCall != null)
        {
          obj = CallFunction(context.GetFunctionDefinition(_identifier), functionCall, context);
          continue;
        }

        var arrayResolution = node as ScriptArrayResolution;
        if (arrayResolution != null)
        {
          SetArrayValue(obj, arrayResolution, context, value);
          continue;
        }

        var genericPostfix = node as ScriptGenericsPostfix;
        if (genericPostfix != null)
        {
          throw new NotSupportedException();
        }
      }
    }

    private static void SetArrayValue(object obj, ScriptArrayResolution scriptArrayResolution, IScriptContext context, object value) {
        scriptArrayResolution.Evaluate(context);

        object[] indexParameters = (object[])context.Result;
        object[] setterParameters = new object[indexParameters.Length + 1];
        indexParameters.CopyTo(setterParameters, 0);
        setterParameters[indexParameters.Length] = value;

        IBinding setter = RuntimeHost.Binder.BindToIndex(obj, setterParameters, true);
        if (setter != null) {
            setter.Invoke(context, null);
            return;
        }

        throw MethodNotFoundException("setter", indexParameters);
    }

    private static void GetArrayValue(object obj, ScriptArrayResolution scriptArrayResolution, IScriptContext context) {
        scriptArrayResolution.Evaluate(context);
        object[] param = (object[])context.Result;
        IBinding indexBind = RuntimeHost.Binder.BindToIndex(obj, param, false);

        if (indexBind != null) {
            context.Result = indexBind.Invoke(context, null);
        } else {
            throw MethodNotFoundException("indexer[]", param);
        }
    }
      
    //NOTE: This code contains bug, when dealing with multi-dimensional arrays
    //private static void SetArrayValue(object obj, ScriptArrayResolution scriptArrayResolution, IScriptContext context, object value)
    //{
    //  scriptArrayResolution.Evaluate(context);

    //  var indexParameters = (object[])context.Result;     
    //  // Denis: don't remove (dynamic) casting whatever R# tells you
    //  ((dynamic)obj)[(dynamic)indexParameters[0]] = (dynamic)value;
    //}

    //private static void GetArrayValue(object obj, ScriptArrayResolution scriptArrayResolution, IScriptContext context)
    //{
    //  scriptArrayResolution.Evaluate(context);

    //  var param = (object[])context.Result;            
    //  context.Result = ((dynamic)obj)[(dynamic)param[0]];      
    //}
    #endregion

    #region Name Part
    private void EvaluateNamePart(IScriptContext context)
    {
      _namePart.Evaluate(context);
      object obj = context.Result;
      bool firstResolution = false;

      if (_modifiers == null)
      {
        context.Result = GetMemberValue(obj, _identifier);
        return;
      }

      Type[] genericArguments = null;
      foreach (ScriptAst node in _modifiers)
      {
        //NOTE: Generic modifier should be the first among other modifiers in the list
        var generic = node as ScriptGenericsPostfix;
        if (generic != null)
        {
          if (genericArguments != null)
          {
            throw new ScriptSyntaxErrorException(string.Format(Strings.WrongSequenceOfModifiers, Code(context)));
          }

          generic.Execute(context);
          genericArguments = (Type[])context.Result;
          continue;
        }

        var functionCall = node as ScriptFunctionCall;
        if (functionCall != null)
        {
          CallClassMethod(obj, _identifier, functionCall, genericArguments, context);
          obj = context.Result;
          continue;
        }

        var arrayResolution = node as ScriptArrayResolution;
        if (arrayResolution != null)
        {
          if (!firstResolution)
          {
            obj = GetMemberValue(obj, _identifier);
            firstResolution = true;
          }

          GetArrayValue(obj, arrayResolution, context);
          obj = context.Result;
          continue;
        }
      }
    }

    private void AssignNamePart(object value, IScriptContext context)
    {
      _namePart.Evaluate(context);
      var obj = context.Result;

      if (_modifiers == null)
      {
        SetMember(context, obj, value);
        return;
      }

      //TODO: Bug, first evaluate get member, see unit test AssignmentToArrayObject
      string localIdentifier = _identifier;
      for (int index=0; index<_modifiers.Count; index++)
      {
        var scriptFunctionCall = _modifiers[index] as ScriptFunctionCall;
        if (scriptFunctionCall != null)
        {
          if (localIdentifier != null)
          {
            CallClassMethod(obj, localIdentifier, scriptFunctionCall, null, context);
            obj = context.Result;
            localIdentifier = null;
          }
          else
          {
            var funcDef = obj as IInvokable;
            if (funcDef == null) throw new ScriptExecutionException(string.Format(Strings.ObjectDoesNotImplementIInvokable, Code(context), "", Span.Start.Line, Span.Start.Position));
            obj = CallFunction(funcDef, scriptFunctionCall, context);
          }
          continue;
        }

        var scriptArrayResolution = _modifiers[index] as ScriptArrayResolution;
        if (scriptArrayResolution != null)
        {
          if (localIdentifier != null)
          {
            obj = GetMemberValue(obj, localIdentifier);
            localIdentifier = null;
          }

          if (index == _modifiers.Count - 1)
          {
            SetArrayValue(obj, scriptArrayResolution, context, value);
          }
          else
          {
            GetArrayValue(obj, scriptArrayResolution, context);
            obj = context.Result;
          }

          continue;
        }
      }
    }
    #endregion

    #region Call Function
    private static object CallFunction(IInvokable functionDefinition, ScriptFunctionCall scriptFunctionCall, IScriptContext context)
    {
      scriptFunctionCall.Evaluate(context);
      return functionDefinition.Invoke(context, (object[])context.Result);
    }

    private static void CallClassMethod(object obj, string memeberInfo, ScriptFunctionCall scriptFunctionCall, Type[] genericArguments, IScriptContext context)
    {
      scriptFunctionCall.Evaluate(context);
      context.Result = CallAppropriateMethod(context, obj, memeberInfo, genericArguments, (object[])context.Result);
    }

    private static object CallAppropriateMethod(IScriptContext context, object obj, string name, Type[] genericArguments, object[] param)
    {
      var methodBind = RuntimeHost.Binder.BindToMethod(obj, name, genericArguments, param);
      if (methodBind != null)
        return methodBind.Invoke(context, null);

      throw MethodNotFoundException(name, param);
    }
    #endregion

    #region Helpers

    private static readonly char[] IdSeparator = new[] { ':' };

    private string ExtractId(string id)
    {      
      var parts = id.Split(IdSeparator);
      if (parts.Length == 1) return parts[0];

      IsGlobal = true;
      return parts[1];
    }

    private void SetMember(IScriptContext context, object obj, object value)
    {
      // Check whether we are setting member value for a dynamic object
      var dynamicObject = obj as DynamicObject;
      if (dynamicObject != null)
      {
        // Get or create a new call site for a setter
        var setter = CallSiteCache.GetOrCreatePropertySetter(_identifier);
        if (setter == null) throw new ScriptIdNotFoundException(_identifier);
        // set property value for the dynamic object
        setter.Target(setter, obj, value);
      }
      else
      {
        IMemberBinding bind = RuntimeHost.Binder.BindToMember(obj, _identifier, true);
        if (bind == null) throw new ScriptIdNotFoundException(_identifier);
        bind.SetValue(value);
      }

      context.Result = value;     
    }

    private static object GetMemberValue(object obj, string memberInfo)
    {
      // Check whether we are getting member value from a dynamic object
      var dynamicObject = obj as DynamicObject;
      if (dynamicObject != null)
      {
        // Get or create a new call site for property getter
        var getter = CallSiteCache.GetOrCreatePropertyGetter(memberInfo);
        if (getter == null) throw new ScriptIdNotFoundException(memberInfo);
        // get property value from dynamic object
        return getter.Target(getter, obj);    
      }

      var bind = RuntimeHost.Binder.BindToMember(obj, memberInfo, true);
      if (bind == null)
        throw new ScriptIdNotFoundException(memberInfo);

      return bind.GetValue();
    }

    private static ScriptMethodNotFoundException MethodNotFoundException(string name, IEnumerable<object> param)
    {
      string message = "";
      foreach (var t in param.Where(t => t != null))
      {
        if (string.IsNullOrEmpty(message)) message += t.GetType().Name;
        else message += ", " + t.GetType().Name;
      }
      return new ScriptMethodNotFoundException(string.Format(Strings.MethodSignatureNotFound, name, message));
    }
    #endregion
  }
    
}