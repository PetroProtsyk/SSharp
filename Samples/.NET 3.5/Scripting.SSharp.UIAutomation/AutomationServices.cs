using System.Threading;
using System.Windows.Automation;

namespace Scripting.SSharp.UIAutomation
{
  internal static class AutomationServices
  {
    public static TPattern GetPattern<TPattern>(this AutomationElement element, AutomationPattern pattern) where TPattern : class
    {
      if (element == null) return default(TPattern);
      return element.GetCurrentPattern(pattern) as TPattern;
    }

    public static AutomationElement FindElementByClassName(this AutomationElement element, string name)
    {
      if (element == null) return null;
      return element.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, name));
    }

    public static AutomationElement FindElementByName(this AutomationElement element, string name)
    {
      if (element == null) return null;
      return element.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, name));
    }

    public static AutomationElement FindElementById(this AutomationElement element, string id)
    {
      if (element == null) return null;
      return element.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, id));
    }

    public static bool InvokeElementById(this AutomationElement element, string id)
    {
      AutomationElement target = FindElementById(element, id);
      if (target == null) return false;

      InvokePattern pattern = GetPattern<InvokePattern>(target, InvokePattern.Pattern);
      if (pattern == null) return false;

      pattern.Invoke();
      return true;
    }

    public static bool InvokeByElementName(this AutomationElement element, string name)
    {
      AutomationElement target = FindElementByName(element, name);
      if (target == null) return false;

      InvokePattern pattern = GetPattern<InvokePattern>(target, InvokePattern.Pattern);
      if (pattern == null) return false;

      pattern.Invoke();
      return true;
    }

    public static AutomationElement Expand(this AutomationElement element)
    {
      if (element == null) return null;
      ExpandCollapsePattern pattern = GetPattern<ExpandCollapsePattern>(element, ExpandCollapsePattern.Pattern);
      if (pattern != null) pattern.Expand();
      return element;
    }

    public static AutomationElement Wait(this AutomationElement element, int timeout)
    {
      Thread.Sleep(timeout);
      return element;
    }
  }
}
