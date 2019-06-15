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

  public enum ScopeOperation
  {
      None,
      Get,
      Set,
      Create
  }

  public class ScopeArgs : EventArgs
  {
    public string Name { get; private set; }

    public object Value { get; set; }

    public bool Cancel { get; set; }

    public ScopeOperation Operation { get; private set; }

    public ScopeArgs(string name, object value, ScopeOperation operation)
    {
      Name = name;
      Value = value;
      Cancel = false;
      Operation = operation;
    }
  }

  public delegate void ScopeSetEvent(IScriptScope sender, ScopeArgs args);
}
