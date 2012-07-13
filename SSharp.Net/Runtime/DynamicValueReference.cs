using System;

namespace Scripting.SSharp.Runtime
{
  public class DynamicValueReference<T> : IValueReference
  {
    private string id;
    private Func<T> getter;
    private Action<object> setter;

    public DynamicValueReference(string id, Func<T> getter) : this(id, getter, null) { }

    public DynamicValueReference(string id, Func<T> getter, Action<object> setter)
    {
      this.id = id;
      this.getter = getter;
      this.setter = setter;
    }

    #region IValueReference Members

    public string Id
    {
      get { return id; }
      protected set { id = value; }
    }

    public event EventHandler<EventArgs> Removed;

    protected virtual void OnRemoved()
    {
      var handler = Removed;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public virtual void Remove()
    {
      OnRemoved();
    }

    public virtual void Reset()
    {
      // do nothing
    }

    public virtual object Value
    {
      get
      {
        return (getter != null) ? getter() : RuntimeHost.NoVariable;
      }
      set
      {
        if (setter != null) setter(value);
      }
    }

    #endregion
  }
}
