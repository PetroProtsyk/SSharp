using System;
using System.Threading;

namespace Scripting.SSharp
{
  /// <summary>
  /// This is a port of .NET 4.0 Lazy class.
  /// </summary> 
  public class Lazy<T>
  {
    private T _value = default(T);
    private volatile bool _isValueCreated = false;
    private Func<T> _valueFactory = null;
    private object _lock;

    public Lazy()
      : this(() => Activator.CreateInstance<T>())
    {
    }

    public Lazy(bool isThreadSafe)
      : this(() => Activator.CreateInstance<T>(), isThreadSafe)
    {
    }

    public Lazy(Func<T> valueFactory) :
      this(valueFactory, true)
    {
    }

    public Lazy(Func<T> valueFactory, bool isThreadSafe)
    {
      if (valueFactory == null) throw new ArgumentNullException("valueFactory");

      if (isThreadSafe)
      {
        this._lock = new object();
      }

      this._valueFactory = valueFactory;
    }


    public T Value
    {
      get
      {
        if (!this._isValueCreated)
        {
          if (this._lock != null)
          {
            Monitor.Enter(this._lock);
          }

          try
          {
            T value = this._valueFactory.Invoke();
            this._valueFactory = null;
            Thread.MemoryBarrier();
            this._value = value;
            this._isValueCreated = true;
          }
          finally
          {
            if (this._lock != null)
            {
              Monitor.Exit(this._lock);
            }
          }
        }
        return this._value;
      }
    }
  }
}
