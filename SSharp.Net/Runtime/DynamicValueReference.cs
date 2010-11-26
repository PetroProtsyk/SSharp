using System;

namespace Scripting.SSharp.Runtime
{
  public class DynamicValueReference<T> : IValueReference
  {
    private readonly Func<T> _getter;
    private readonly Action<object> _setter;

    public IScriptScope Scope { get; set; }

    public DynamicValueReference(string id, Func<T> getter) : this(id, getter, null) { }

    public DynamicValueReference(string id, Func<T> getter, Action<object> setter)
    {
      Id = id;
      _getter = getter;
      _setter = setter;
    }

    #region IValueReference Members

    public string Id { get; protected set; }

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
        return (_getter != null) ? _getter() : RuntimeHost.NoVariable;
      }
      set
      {
        if (_setter != null) _setter(value);
      }
    }

    #endregion
  }
}
