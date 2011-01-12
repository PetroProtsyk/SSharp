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
using System.Collections;
using System.Collections.Generic;
using Scripting.SSharp.Diagnostics;

namespace Scripting.SSharp.Runtime
{
  public class FunctionTable : IEnumerable<KeyValuePair<string, Type>>
  {
    private readonly Dictionary<string, Type> _functions = new Dictionary<string, Type>();

    public FunctionTable AddFunction<T>(string name) where T : IInvokable, new()
    {      
      Requires.NotNullOrEmpty(name, "name");

      _functions[name] = typeof(T);                        

      return this;
    }

    public FunctionTable AddFunction(string name, Type functionType)
    {
      Requires.NotNullOrEmpty(name, "name");
      Requires.NotNull(functionType, "functionType");
      Requires.OfType<IInvokable>(functionType, "functionType");

      _functions[name] = functionType;

      return this;
    }

    public bool Contains(string name)
    {
      Requires.NotNullOrEmpty(name, "name");

      return _functions.ContainsKey(name);
    }

    #region IEnumerable<KeyValuePair<string,Type>> Members

    public IEnumerator<KeyValuePair<string, Type>> GetEnumerator()
    {
      return _functions.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
      return _functions.GetEnumerator();
    }

    #endregion
  }
}
