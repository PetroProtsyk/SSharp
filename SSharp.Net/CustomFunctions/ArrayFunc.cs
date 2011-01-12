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
using System.Linq;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.CustomFunctions
{
  internal class ArrayFunc : IInvokable
  {
    public static ArrayFunc FunctionDefinition = new ArrayFunc();
    public static string FunctionName = "array";

    private ArrayFunc()
    {
    }

    #region IInvokable Members

    public bool CanInvoke()
    {
      return true;
    }

    public object Invoke(IScriptContext context, object[] args)
    {
      if (args == null || args.Length == 0) return new object[0];

      Array result;
      var type = args[0] as Type;
      if (type != null)
      {
        result = Array.CreateInstance(type, args.Length - 1);
        Array.Copy(args, 1, result, 0, result.Length);

        return result;
      }

      foreach (var item in args.Where(item => item != null))
      {
        if (type == null) { type = item.GetType(); continue; }
        if (type != item.GetType()) return args.Clone();
      }

      if (type == null) return args.Clone();

      result = Array.CreateInstance(type, args.Length);
      Array.Copy(args, result, result.Length);

      return result;
    }

    #endregion
  }
}