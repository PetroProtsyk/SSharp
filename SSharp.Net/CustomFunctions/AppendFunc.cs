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

using Scripting.SSharp.Runtime;
using Scripting.SSharp.Parser.Ast;

namespace Scripting.SSharp.CustomFunctions
{
  internal class AppendFunc : IInvokable
  {
    public static AppendFunc FunctionDefinition = new AppendFunc();
    public static string FunctionName = "AppendAst";

    private AppendFunc()
    {
    }

    #region IInvokable Members

    public bool CanInvoke()
    {
      return true;
    }

    public object Invoke(IScriptContext context, object[] args)
    {
      var node = (ScriptProg)args[0];

      //Get Prog
      var prog = node.Parent;
      while (prog != null && !(prog is ScriptProg))
        prog = prog.Parent;

      if (prog != null)
      {
        foreach (var scriptnode in node.Elements.ChildNodes)
          prog.ChildNodes[0].AddChild(scriptnode);
      }

      return node;
    }

    #endregion
  }
}
