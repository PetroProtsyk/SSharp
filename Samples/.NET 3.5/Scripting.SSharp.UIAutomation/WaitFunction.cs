using System;
using System.Threading;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.UIAutomation
{
  internal sealed class WaitFunction : IInvokable
  {
    public bool CanInvoke()
    {
      return true;
    }

    public object Invoke(IScriptContext context, object[] args)
    {
      if (args == null || args.Length == 0) return null;

      int timeout = Convert.ToInt32(args[0]);
      Thread.Sleep(timeout);

      return null;
    }
  }
}
