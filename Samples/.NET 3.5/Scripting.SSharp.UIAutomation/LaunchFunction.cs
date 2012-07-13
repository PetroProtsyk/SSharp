using System.Diagnostics;
using System.Windows.Automation;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.UIAutomation
{
  internal sealed class LaunchFunction : IInvokable
  {
    public bool CanInvoke()
    {
      return true;
    }

    public object Invoke(IScriptContext context, object[] args)
    {
      if (args == null || args.Length == 0) return null;

      Process process = new Process();
      process.StartInfo.FileName = args[0].ToString();
      if (args.Length > 1) process.StartInfo.Arguments = args[1].ToString();
      process.Start();
      process.WaitForInputIdle(1000);
      return (AutomationElement.FromHandle(process.MainWindowHandle));
    }
  }
}
