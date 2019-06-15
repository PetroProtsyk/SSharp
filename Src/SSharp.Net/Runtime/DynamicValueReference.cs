/*
 * Copyright © 2011, Petro Protsyk, Denys Vuika
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *  http://www.apache.org/licenses/LICENSE-2.0
 *  
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
