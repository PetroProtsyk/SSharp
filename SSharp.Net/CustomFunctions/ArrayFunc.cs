using System;
using System.Linq;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.CustomFunctions
{
  internal class ArrayFunc : IInvokable
  {
    public static ArrayFunc FunctionDefinition = new ArrayFunc();
    public static string FunctionName = "array";

    private ArrayFunc()
    {
    }

    #region IInvokable Members

    public bool CanInvoke()
    {
      return true;
    }

    public object Invoke(IScriptContext context, object[] args)
    {
      if (args == null || args.Length == 0) return new object[0];

      Array result;
      var type = args[0] as Type;
      if (type != null)
      {
        result = Array.CreateInstance(type, args.Length - 1);
        Array.Copy(args, 1, result, 0, result.Length);

        return result;
      }

      foreach (var item in args.Where(item => item != null))
      {
        if (type == null) { type = item.GetType(); continue; }
        if (type != item.GetType()) return args.Clone();
      }

      if (type == null) return args.Clone();

      result = Array.CreateInstance(type, args.Length);
      Array.Copy(args, result, result.Length);

      return result;
    }

    #endregion
  }
}