using System;

namespace Scripting.SSharp.Runtime
{
  /// <summary>
  /// Scopes implementing this interface should raise change events
  /// </summary>
  public interface INotifyingScope : IScriptScope
  {
    event ScopeSetEvent BeforeGetItem;

    event ScopeSetEvent AfterGetItem;

    event ScopeSetEvent BeforeSetItem;

    event ScopeSetEvent AfterSetItem;
  }

  public class ScopeArgs : EventArgs
  {
    public string Name { get; private set; }

    public object Value { get; set; }

    public bool Cancel { get; set; }

    public ScopeArgs(string name, object value)
    {
      Name = name;
      Value = value;
      Cancel = false;
    }
  }

  public delegate void ScopeSetEvent(IScriptScope sender, ScopeArgs args);
}
