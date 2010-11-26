using System;
using System.Reflection;
using Scripting.SSharp.Parser.Ast;

namespace Scripting.SSharp.Runtime.Promotion
{
  /// <summary>
  /// Binding to the method or constructor, also used for index getters
  /// </summary>
  internal class MethodBinding : IBinding
  {
    MethodBase Method { get; set; }
    object Target { get; set; }
    object[] Arguments { get; set; }

    /// <summary>
    /// Creates binding to the method
    /// </summary>
    /// <param name="method">method or constructor to invoke</param>
    /// <param name="target">method's owner or type if method is static</param>
    /// <param name="arguments">a list of converted arguments that will be directly passed to the method</param>
    public MethodBinding(MethodBase method, object target, object[] arguments)
    {
      Method = method;
      Arguments = arguments;
      Target = target;
    }

    #region IInvokable Members

    public bool CanInvoke()
    {
      return Method != null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="args">Ignored for strongly bound objects (null should be passed)</param>
    /// <returns></returns>
    public object Invoke(IScriptContext context, object[] args)
    {
      //object result = RuntimeHost.NullValue;
      //if (args != null)
      //{
      //  if (Target == null && !Method.IsStatic)
      //    result = Method.Invoke(args.First(), args.Skip(1).ToArray());
      //  else
      //    result = Method.Invoke(Target, args);
      //}
      //else
      //{
      //  result = Method.Invoke(Target, Arguments);
      //}
      //context.Result = result;     
      //return result;      

      if (args != null) Arguments = args;

      //NOTE: ref, out
      object[] callArguments = new object[Arguments.Length];
      for (int i = 0; i < callArguments.Length; i++)
      {
        ScopeValueReference vr = Arguments[i] as ScopeValueReference;
        if (vr != null){
            callArguments[i] = vr.ConvertedValue;
            continue;
        }

        FunctionDelegate d = Arguments[i] as FunctionDelegate;
        if (d != null) {
            d.ActiveContext = context;
            callArguments[i] = d.Method;
            continue;
        }

        callArguments[i] = Arguments[i];
      }

      var result = Method.Invoke(Target, callArguments);

      //NOTE: ref, out
      for (var i = 0; i < callArguments.Length; i++)
      {
        var vr = Arguments[i] as ScopeValueReference;
        if (vr != null)
          vr.Value = callArguments[i];
      }

      return result;
    }

    #endregion

#if !PocketPC && !SILVERLIGHT
    //TODO: Review this approach
    public static implicit operator IntPtr(MethodBinding invokableMethod)
    {
      return invokableMethod.Method.MethodHandle.GetFunctionPointer();
    }
#endif
  }
}
