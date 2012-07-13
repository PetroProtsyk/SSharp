using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime.Promotion;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Qualified Name
  /// </summary>
  internal class ScriptQualifiedName : ScriptExpr
  {
    #region Members
    private string identifier;
    private List<ScriptAst> modifiers;
    private ScriptQualifiedName namePart;

    private delegate void EvaluateFunction(IScriptContext context);
    private delegate void AssignFunction(object value, IScriptContext context);

    private EvaluateFunction evaluation;
    private AssignFunction assignment;

    public ReadOnlyCollection<ScriptAst> Modifiers
    {
      get
      {
        return new ReadOnlyCollection<ScriptAst>(modifiers);
      }
    }

    public ScriptQualifiedName NextPart
    {
      get
      {
        return namePart;
      }
    }

    public bool NextFirst
    {
      get;
      private set;
    }

    public string Identifier
    {
      get
      {
        return identifier;
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
        identifier = ExtractId(((TokenAst)ChildNodes[0]).Text);
        IsVariable = true;

        evaluation = EvaluateIdentifier;
        assignment = AssignIdentifier;
      }
      else
        if (ChildNodes[0] is TokenAst && ChildNodes[1].ChildNodes.Count != 0)
        {
          identifier = ExtractId(((TokenAst)ChildNodes[0]).Text);

          //NOTE: There might be two cases:
          //      1) a()[]...() 
          //      2) a<>.(NamePart)    
          modifiers = new List<ScriptAst>();
          foreach (ScriptAst node in ChildNodes[1].ChildNodes)
            modifiers.Add(node);

          ScriptGenericsPostfix generic = modifiers.FirstOrDefault() as ScriptGenericsPostfix;
          if (generic != null && modifiers.Count == 1)
          {
            //Case 2
            evaluation = EvaluateGenericType;
            assignment = null;
          }
          else
          {
            //Case 1
            evaluation = EvaluateFunctionCall;
            assignment = AssignArray;
          }
        }
        else
        {
          namePart = ChildNodes[0] as ScriptQualifiedName;
          identifier = ExtractId(((TokenAst)ChildNodes[2]).Text);
          NextFirst = true;
          if (ChildNodes.Count == 4 && ChildNodes[3].ChildNodes.Count != 0)
          {
            modifiers = new List<ScriptAst>();
            foreach (ScriptAst node in ChildNodes[3].ChildNodes)
            {
              modifiers.Add(node);
            }
          }
          evaluation = EvaluateNamePart;
          assignment = AssignNamePart;
        }
    }
    #endregion

    #region Public Methods
    public override void Evaluate(IScriptContext context)
    {
      evaluation(context);
    }

    public void Assign(object value, IScriptContext context)
    {
      assignment(value, context);
    }
    #endregion

    #region Identifier
    IValueReference variable = null;
    
    private void EvaluateIdentifier(IScriptContext context)
    {
      object result;

      if (variable != null)
      {
        result = variable.Value;
      }
      else
      {
        result = GetIndentifierValue(context, identifier);
      }

      context.Result = result;
    }

    private object GetIndentifierValue(IScriptContext context, string identifier)
    {
      //object result = context.GetItem(identifier, false);
      //if (result != RuntimeHost.NoVariable) return result;

      if (IsGlobal)
      {

        variable = null;
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
        object result;
        variable = CreateRef(identifier, context, true, out result);

        if (result != RuntimeHost.NoVariable)
        {
          return result;
        }
      }

      if (RuntimeHost.HasType(identifier))
        return RuntimeHost.GetType(identifier);
      else
        return NamespaceResolver.Get(identifier);
    }

    private void AssignIdentifier(object value, IScriptContext context)
    {
      if (IsGlobal)
      {
        SetToParentScope(context.Scope.Parent, identifier, value);
        variable = null;
        return;
      }

      LocalScope scope = context.Scope as LocalScope;
      if (IsVar && scope != null)
      {
        scope.CreateVariable(identifier, value);
        return;
      }

      if (variable != null)
      {
        variable.Value = value;
      }
      else
      {
        context.SetItem(identifier, value);
        
        object tmp;
        variable = CreateRef(identifier, context, false, out tmp);
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
          value = context.GetItem(identifier, false);
      }

      return result;
    }

    private void ReferenceRemoved(object sender, EventArgs e)
    {
      if (sender == variable)
      {
        variable.Removed -= ReferenceRemoved;
        variable = null;
      }
    }

    /// <summary>
    /// Sets variable to the first scope in hierarchy which already has this variable
    /// </summary>
    /// <param name="id"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    internal static void SetToParentScope(IScriptScope parent, string id, object value)
    {
      IScriptScope scope = parent;
      while (scope != null)
      {
        if (scope.HasVariable(id))
        {
          scope.SetItem(id, value);
          return;
        }
        scope = scope.Parent;
      }

      throw new ScriptIdNotFoundException(string.Format("Global name {0} was not found in scopes", id));
    }
    #endregion

    #region Single Call
    private void EvaluateGenericType(IScriptContext context)
    {
      ScriptGenericsPostfix genericPostfix = (ScriptGenericsPostfix)modifiers.First();

      Type genericType = GetIndentifierValue(context, genericPostfix.GetGenericTypeName(identifier)) as Type;
      if (genericType == null || !genericType.IsGenericType)
      {
        throw new ScriptException("Given type is not generic");
      }

      genericPostfix.Evaluate(context);
      context.Result = genericType.MakeGenericType((Type[])context.Result);
    }

    private void EvaluateFunctionCall(IScriptContext context)
    {
      EvaluateIdentifier(context);

      foreach (ScriptAst node in modifiers)
      {
        ScriptFunctionCall funcCall = node as ScriptFunctionCall;
        if (funcCall != null)
        {
          IInvokable function = context.Result as IInvokable;
          if (function == null)
            throw new ScriptException("Is not a function type");
          context.Result = CallFunction(function, funcCall, context);
          continue;
        }

        ScriptArrayResolution arrayResolution = node as ScriptArrayResolution;
        if (arrayResolution != null)
        {
          GetArrayValue(context.Result, arrayResolution, context);
          continue;
        }

        ScriptGenericsPostfix genericPostfix = node as ScriptGenericsPostfix;
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
      object obj = context.GetItem(identifier, true);

      foreach (ScriptAst node in modifiers)
      {
        ScriptFunctionCall functionCall = node as ScriptFunctionCall;
        if (functionCall != null)
        {
          obj = CallFunction(context.GetFunctionDefinition(identifier), functionCall, context);
          continue;
        }

        ScriptArrayResolution arrayResolution = node as ScriptArrayResolution;
        if (arrayResolution != null)
        {
          SetArrayValue(obj, arrayResolution, context, value);
          continue;
        }

        ScriptGenericsPostfix genericPostfix = node as ScriptGenericsPostfix;
        if (genericPostfix != null)
        {
          throw new NotSupportedException();
        }
      }
    }

    private static void SetArrayValue(object obj, ScriptArrayResolution scriptArrayResolution, IScriptContext context, object value)
    {
      scriptArrayResolution.Evaluate(context);

      object[] indexParameters = (object[])context.Result;
      object[] setterParameters = new object[indexParameters.Length + 1];
      indexParameters.CopyTo(setterParameters, 0);
      setterParameters[indexParameters.Length] = value;

      IBinding setter = RuntimeHost.Binder.BindToIndex(obj, setterParameters, true);
      if (setter != null)
      {
        setter.Invoke(context, null);
        return;
      }

      throw MethodNotFoundException("setter", indexParameters);
    }

    private static void GetArrayValue(object obj, ScriptArrayResolution scriptArrayResolution, IScriptContext context)
    {
      scriptArrayResolution.Evaluate(context);
      object[] param = (object[])context.Result;
      IBinding indexBind = RuntimeHost.Binder.BindToIndex(obj, param, false);

      if (indexBind != null)
      {
        context.Result = indexBind.Invoke(context, null);
      }
      else
      {
        throw MethodNotFoundException("indexer[]", param);
      }
    }
    #endregion

    #region Name Part
    private void EvaluateNamePart(IScriptContext context)
    {
      namePart.Evaluate(context);
      object obj = context.Result;
      bool firstResolution = false;

      if (modifiers == null)
      {
        context.Result = GetMemberValue(obj, identifier);
        return;
      }

      Type[] genericArguments = null;
      foreach (ScriptAst node in modifiers)
      {
        //NOTE: Generic modifier should be the first among other modifiers in the list
        ScriptGenericsPostfix generic = node as ScriptGenericsPostfix;
        if (generic != null)
        {
          if (genericArguments != null)
          {
            throw new ScriptException("Wrong modifiers sequence");
          }

          generic.Execute(context);
          genericArguments = (Type[])context.Result;
          continue;
        }

        ScriptFunctionCall functionCall = node as ScriptFunctionCall;
        if (functionCall != null)
        {
          CallClassMethod(obj, identifier, functionCall, genericArguments, context);
          obj = context.Result;
          continue;
        }

        ScriptArrayResolution arrayResolution = node as ScriptArrayResolution;
        if (arrayResolution != null)
        {
          if (!firstResolution)
          {
            obj = GetMemberValue(obj, identifier);
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
      namePart.Evaluate(context);
      object obj = context.Result;

      if (modifiers == null)
      {
        SetMember(context, obj, value);
        return;
      }

      //TODO: Bug, first evaluate get member, see unit test AssignmentToArrayObject
      string localIdentifier = identifier;
      for (int index=0; index<modifiers.Count; index++)
      {
        ScriptFunctionCall scriptFunctionCall = modifiers[index] as ScriptFunctionCall;
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
            IInvokable funcDef = obj as IInvokable;
            if (funcDef == null) throw new ScriptException("Attempt to invoke non IInvokable object.");
            obj = CallFunction(funcDef, scriptFunctionCall, context);
          }
          continue;
        }

        ScriptArrayResolution scriptArrayResolution = modifiers[index] as ScriptArrayResolution;
        if (scriptArrayResolution != null)
        {
          if (localIdentifier != null)
          {
            obj = GetMemberValue(obj, localIdentifier);
            localIdentifier = null;
          }

          if (index == modifiers.Count - 1)
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

    private void CallClassMethod(object obj, string memeberInfo, ScriptFunctionCall scriptFunctionCall, Type[] genericArguments, IScriptContext context)
    {
      scriptFunctionCall.Evaluate(context);
      context.Result = CallAppropriateMethod(context, obj, memeberInfo, genericArguments, (object[])context.Result);
    }

    private static object CallAppropriateMethod(IScriptContext context, object obj, string Name, Type[] genericArguments, object[] param)
    {
      IBinding methodBind = methodBind = RuntimeHost.Binder.BindToMethod(obj, Name, genericArguments, param);
      if (methodBind != null)
        return methodBind.Invoke(context, param);

      throw MethodNotFoundException(Name, param);
    }
    #endregion

    #region Helpers
    private string ExtractId(string id)
    {
      string[] parts = id.Split(":".ToCharArray());
      if (parts.Length == 1) return parts[0];

      IsGlobal = true;
      return parts[1];
    }

    private void SetMember(IScriptContext context, object obj, object value)
    {
      IMemberBinding bind = RuntimeHost.Binder.BindToMember(obj, identifier, true);
      if (bind == null)
        throw new ScriptIdNotFoundException(identifier);

      bind.SetValue(value);
      context.Result = value;
      
      //Context.Result = RuntimeHost.Binder.Set(Identifier, obj, value);
    }

    private static object GetMemberValue(object obj, string memberInfo)
    {
      IMemberBinding bind = RuntimeHost.Binder.BindToMember(obj, memberInfo, true);
      if (bind == null)
        throw new ScriptIdNotFoundException(memberInfo);

      return bind.GetValue();
     
      //return RuntimeHost.Binder.Get(memberInfo, obj);
    }

    private static ScriptMethodNotFoundException MethodNotFoundException(string Name, object[] param)
    {
      string message = "";
      foreach (object t in param)
      {
        if (t != null)
        {
          if (string.IsNullOrEmpty(message)) message += t.GetType().Name;
          else message += ", " + t.GetType().Name;
        }
      }
      return new ScriptMethodNotFoundException("Semantic: There is no method with such signature: " + Name + "(" + message + ")");
    }
    #endregion
  }
    
}