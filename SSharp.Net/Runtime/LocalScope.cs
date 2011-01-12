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
  public class LocalScope : ScriptScope
  {
    public LocalScope(IScriptScope parent):
      base(parent)
    {
      if (parent == null)
        throw new NotSupportedException("Can't create stand-alone local scope");
    }

    public override void SetItem(string id, object value)
    {
      if (HasVariable(id))
        base.SetItem(id, value);
      else
        Parent.SetItem(id, value);
    }

    public void CreateVariable(string id, object value)
    {
      base.SetItem(id, value);
    }

    protected override void Cleanup()
    {
      try
      {
      }
      finally
      {
        base.Cleanup();
      }
    }
  }
}
