using System.Windows.Automation;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.UIAutomation
{
  internal class InvokeByIdFunction : IInvokable
  {
    public bool CanInvoke()
    {
      return true;
    }

    public object Invoke(IScriptContext context, object[] args)
    {
      if (args == null || args.Length < 2) return null;

      AutomationElement element = args[0] as AutomationElement;
      if (element == null) return null;

      string id = args[1].ToString();
      if (string.IsNullOrEmpty(id)) return null;

      element.InvokeElementById(id);
      return null;
    }
  }
}
