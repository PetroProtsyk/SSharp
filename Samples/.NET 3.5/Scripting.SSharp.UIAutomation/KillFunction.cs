using System.Diagnostics;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.UIAutomation
{
  internal sealed class KillFunction : IInvokable
  {
    public bool CanInvoke()
    {
      return true;
    }

    public object Invoke(IScriptContext context, object[] args)
    {
      if (args == null || args.Length == 0) return null;

      string processName = args[0].ToString();
      foreach (Process proc in Process.GetProcessesByName(processName))
        proc.Kill();

      return null;
    }
  }
}
