using System.Diagnostics;
using System.Windows.Automation;

namespace Scripting.SSharp.UIAutomation
{
  internal static class UIAutomationHelper
  {
    public static void KillProcesses(string name)
    {
      Process[] procs = Process.GetProcessesByName(name);
      foreach (Process proc in procs)
        proc.Kill();
    }

    public static AutomationElement LaunchApplication(string fileName)
    {
      return LaunchApplication(fileName, "");
    }

    public static AutomationElement LaunchApplication(string fileName, string arguments)
    {
      Process process = new Process();
      process.StartInfo.FileName = fileName;
      process.StartInfo.Arguments = arguments;
      process.Start();
      process.WaitForInputIdle(1000);
      return (AutomationElement.FromHandle(process.MainWindowHandle));
    }

    public static AutomationElement GetElementByName(AutomationElement window, string name)
    {
      if (window == null)
        return null;

      return window.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, name));
    }

    public static AutomationElement GetElementByClassName(AutomationElement window, string className)
    {
      if (window == null)
        return null;

      return window.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, className));
    }

    public static AutomationElement GetElementById(AutomationElement window, string id)
    {
      if (window == null)
        return null;

      return window.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, id));
    }

    public static bool InvokeElementByName(AutomationElement window, string name)
    {
      return InvokeElement(window, new PropertyCondition(AutomationElement.NameProperty, name));
    }

    public static bool InvokeElementById(AutomationElement window, string id)
    {
      return InvokeElement(window, new PropertyCondition(AutomationElement.AutomationIdProperty, id));
    }

    public static bool InvokeElement(AutomationElement window, Condition condition)
    {
      if (window == null)
        return false;

      AutomationElement element = window.FindFirst(TreeScope.Descendants, condition);
      if (element != null)
      {
        InvokePattern pattern = element.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
        pattern.Invoke();
        return true;
      }

      return false;
    }
  }
}
