using System.Windows.Automation;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.UIAutomation
{ 
  internal sealed class ExpandFunction : IInvokable
  {
    public bool CanInvoke()
    {
      return true;
    }

    public object Invoke(IScriptContext context, object[] args)
    {
      if (args == null || args.Length < 1) return null;

      AutomationElement element = args[0] as AutomationElement;
      if (element == null) return null;

      ExpandCollapsePattern pattern = element.GetPattern<ExpandCollapsePattern>(ExpandCollapsePattern.Pattern);
      if (pattern != null) pattern.Expand();

      return null;
    }
  }
}
