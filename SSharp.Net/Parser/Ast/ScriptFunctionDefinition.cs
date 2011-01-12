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

using System.Collections.Generic;
using Scripting.SSharp.Runtime;
using System;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;

namespace Scripting.SSharp.Parser.Ast
{  
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptFunctionDefinition : ScriptAst, IInvokable
  {
    public string Name { get; private set; }
    private readonly ScriptFuncParameters _parameters;
    private readonly ScriptFuncContract _contract;
    private readonly ScriptGlobalList _globalList;
    private readonly ScriptCompoundStatement _body;
    private IScriptContext _activeContext;

    //Field used to ensure consistency
    internal Script _owner;

    public ScriptFunctionDefinition(AstNodeArgs args)
        : base(args)
    {
      var funcName = ChildNodes[1] as TokenAst;
      var index = 0;

      if (funcName != null)
      {
        Name = funcName.Text;
      }
      else
      //Function expression
      {
        Name = null;
        index = 1;
      }

      if (ChildNodes.Count == 5-index)
      {
        _contract = ChildNodes[3 - index] as ScriptFuncContract;
        _parameters = ChildNodes[3 - index] as ScriptFuncParameters;
      }

      if (ChildNodes.Count == 6 - index)
      {
        _parameters = ChildNodes[2 - index] as ScriptFuncParameters;
        _globalList = ChildNodes[3 - index] as ScriptGlobalList;
        _contract = ChildNodes[4 - index] as ScriptFuncContract;
      }

      _body = (ScriptCompoundStatement)ChildNodes[ChildNodes.Count - 1];
      _body.ShouldCreateScope = false;

      if (_contract != null) _contract._function = this;
    }

    public override void Evaluate(IScriptContext context)
    {
      if (Name != null)
        context.SetItem(Name, this);

      context.Result = this;
    }

    #region IInvokable Members

    public bool CanInvoke()
    {
      return true;
    }

    //TODO: Review this approach
    public object ThreadInvoke(IScriptContext context)
    {
     return Invoke(context, null);
    }

    public object Invoke(IScriptContext context, object[] args)
    {
        _activeContext = context;
        var result = RuntimeHost.NullValue;

        var functionScope = (INotifyingScope)RuntimeHost.ScopeFactory.Create(ScopeTypes.Function, context.Scope, context);
        context.CreateScope(functionScope);
      
        try
        {
          if (_parameters != null)
          {
            context.Result = args;
            _parameters.Evaluate(context);
          }

          functionScope.BeforeSetItem += ScopeBeforeSetItem;

          if (_contract != null)
          {
            functionScope.AfterSetItem += CheckContractInvariant;
            _contract.CheckPre(context);
            _contract.CheckInv(context);
          }

          context.Result = RuntimeHost.NullValue;
          _body.Evaluate(context);
          result = context.Result;

          if (_contract != null)
          {
            functionScope.AfterSetItem -= CheckContractInvariant;
            _contract.CheckInv(context);
            _contract.CheckPost(context);
          }
        }
        finally
        {
          context.RemoveLocalScope();
          context.SetBreak(false);
          context.SetContinue(false);
          context.SetReturn(false);
          context.Result = result;

          functionScope.BeforeSetItem -= ScopeBeforeSetItem;
          _activeContext = null;
        }

        return result;
    }

    private void ScopeBeforeSetItem(IScriptScope sender, ScopeArgs args)
    {
      //TODO: Performance improvement. Should be evaluated once per function call
      List<string> globalNames = GetGlobalNames(_activeContext);

      if (globalNames.Contains(args.Name))
      {
        ScriptQualifiedName.SetToParentScope(sender.Parent, args.Name, args.Value);        
        args.Cancel = true;
      }

      //if (!sender.HasVariable(args.Name))
      //{
      //  args.Cancel = SetToParentScope(sender.Parent, args.Name, args.Value);
      //}
    }

    private void CheckContractInvariant(object sender, ScopeArgs args)
    {
      _contract.CheckInv(_activeContext);
    }

    private List<string> GetGlobalNames(IScriptContext context)
    {
      var globalNames = new List<string>();
      if (_globalList != null)
      {
        _globalList.Evaluate(context);
        globalNames = new List<string>((string[]) context.Result);
      }

      return globalNames;
    }

    internal FunctionDelegate AsDelegate(Type delegateType) {
        if (!typeof(Delegate).IsAssignableFrom(delegateType)) return null;
        MethodInfo delegateInvokeMethod = delegateType.GetMethod("Invoke");
        Type[] tParameters = new Type[] { typeof(FunctionDelegate) }.Concat(delegateInvokeMethod.GetParameters().Select(p => p.ParameterType)).ToArray();

        DynamicMethod dm = new DynamicMethod(ToString(),
            delegateInvokeMethod.ReturnType,
            tParameters,
            typeof(FunctionDelegate).Module);

        MethodInfo invokeMethod;

        if (delegateInvokeMethod.ReturnType == typeof(void))
            invokeMethod = typeof(FunctionDelegate).GetMethod("VoidInvoke");
        else
            invokeMethod = typeof(FunctionDelegate).GetMethod("Invoke").MakeGenericMethod(delegateInvokeMethod.ReturnType);

        ILGenerator il = dm.GetILGenerator();
        il.DeclareLocal(typeof(object[]));

        //Create array
        il.Emit(OpCodes.Ldc_I4, tParameters.Length - 1);
        il.Emit(OpCodes.Newarr, typeof(object));
        il.Emit(OpCodes.Stloc_0);

        for (int i = 0; i < tParameters.Length - 1; i++) {
           il.Emit(OpCodes.Ldloc_0);

           il.Emit(OpCodes.Ldc_I4, i);
           il.Emit(OpCodes.Ldarg, i+1);
           if (tParameters[i + 1].IsValueType) {
               il.Emit(OpCodes.Box, tParameters[i + 1]);
           }

           il.Emit(OpCodes.Stelem_Ref);
        }

        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldloc_0);
        il.EmitCall(OpCodes.Call, invokeMethod, null);
                
        //convert to return type
        //il.Emit(OpCodes.Castclass, delegateInvokeMethod.ReturnType);
        //il.Emit(OpCodes.Unbox_Any);      
        il.Emit(OpCodes.Ret);

        FunctionDelegate result = new FunctionDelegate();
        result.Function = this;
        result.Method = dm.CreateDelegate(delegateType, result);
        
        return result; 

        //DynamicMethod dm = new DynamicMethod(ToString(),
        //    typeof(object),
        //    new Type[] { typeof(IInvokable), typeof(IScriptContext), typeof(object[]) },
        //    typeof(IInvokable).Module);

        //MethodInfo invokeMethod = typeof(IInvokable)
        //    .GetMethod("Invoke");
                
        //ILGenerator il = dm.GetILGenerator(256);
        //il.Emit(OpCodes.Ldarg_0);
        //il.Emit(OpCodes.Ldarg_1);
        //il.Emit(OpCodes.Ldarg_2);
        //il.EmitCall(OpCodes.Call, invokeMethod, null);
        //il.Emit(OpCodes.Ret);

        //return dm.CreateDelegate(delegateType, this);
    }
    #endregion

    public override string ToString()
    {
      return Name == null ? "_anonimous_func" : "_func:" + Name;
    }    
    }

  internal class FunctionDelegate {
      public ScriptFunctionDefinition Function { get; set; }
      public Delegate Method { get; set; }
      public IScriptContext ActiveContext { get; set; }

      public T Invoke<T>(object[] args) {
          object rez = Function.Invoke(ActiveContext, args);
          return (T)rez;
      }

      public void VoidInvoke(object[] args) {
          object rez = Function.Invoke(ActiveContext, args);
      }
  }
}