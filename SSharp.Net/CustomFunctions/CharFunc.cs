using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.CustomFunctions
{
  internal class CharFunc : IInvokable
  {
    public static CharFunc FunctionDefinition = new CharFunc();
    public static string FunctionName = "char";

    private CharFunc()
    {
    }

    #region IInvokable Members

    public bool CanInvoke()
    {
      return true;
    }

    public object Invoke(IScriptContext context, object[] args)
    {
      if (args == null || args.Length == 0) return '\0';

      return ((string)args[0])[0];
    }

    #endregion
  }
}