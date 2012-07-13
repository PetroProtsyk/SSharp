using System.Windows.Automation;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.UIAutomation
{
  internal sealed class FocusEditorFunction : IInvokable
  {
    public bool CanInvoke()
    {
      return true;
    }

    public object Invoke(IScriptContext context, object[] args)
    {
      if (args == null || args.Length < 1) return null;

      AutomationElement element = args[0] as AutomationElement;
      if (element == null) return false;

      TextPattern pattern = element.GetPattern<TextPattern>(TextPattern.Pattern);
      if (pattern == null) return false;

      pattern.DocumentRange.Select();
      return true;
    }
  }
}
