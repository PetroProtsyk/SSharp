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
