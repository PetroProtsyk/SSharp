using System;
using System.Diagnostics;

namespace Scripting.SSharp.Runtime
{
  [DebuggerDisplay("({Id},{Value})")]
  public class ValueReference : IValueReference
  {
    #region IValueReference Members

    public object Value
    {
      get;
      set;
    }

    public string Id
    {
      get;
      private set;
    }

    public IScriptScope Scope { get; set; }

    public void Reset()
    {
      Value = RuntimeHost.NoVariable;
    }

    public ValueReference(string id)
    {
      Id = id;
      Value = RuntimeHost.NoVariable;
    }

    public ValueReference(string id, object value)
    {
      Id = id;
      Value = value;
    }

    public event EventHandler<EventArgs> Removed;

    protected virtual void OnRemoved()
    {
      var handler = Removed;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public void Remove()
    {
      //Value = RuntimeHost.NoVariable;
      OnRemoved();
    }
    #endregion

    public static ValueReference Null = new ValueReference("Null") { Value = RuntimeHost.NoVariable };

  }
}
