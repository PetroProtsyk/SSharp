using System;

namespace Scripting.SSharp.Runtime
{
  public interface IValueReference
  {
    object Value { get; set; }
    string Id { get; }

    void Reset();

    event EventHandler<EventArgs> Removed;
    void Remove();
  }
}
