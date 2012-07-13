using System;

namespace Scripting.SSharp.Runtime
{
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

    public object Scope { get; set; }

    public void Reset()
    {
      Value = RuntimeHost.NoVariable;
    }

    public ValueReference(string id)
    {
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
      EventHandler<EventArgs> handler = Removed;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public void Remove()
    {
      Value = RuntimeHost.NoVariable;
      OnRemoved();
    }
    #endregion

    public static ValueReference Null = new ValueReference("Null") { Value = RuntimeHost.NoVariable };

  }
}
