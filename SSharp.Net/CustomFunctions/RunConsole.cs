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

namespace Scripting.SSharp.CustomFunctions
{
  internal class RunConsole : IInvokable
  {
    public static RunConsole FunctionDefinition = new RunConsole();
    public static string FunctionName = "RunConsole";

    private RunConsole()
    {
    }

    #region IInvokable Members

    public bool CanInvoke()
    {
      return true;
    }

    public object Invoke(IScriptContext context, object[] args)
    {
      const string code = @"Console.WriteLine('Please Input Script.NET program. Press Ctrl+Z when finish.');
            s = Console.In.ReadToEnd();
            astNode = Compiler.Parse(s);
            if (astNode != null)
              astNode.Execute(new ScriptContext()).Value;
            else      
              throw new ScriptException('Syntax Error');";

      var prog = Script.Compile(code);
      return prog.Execute();
    }

    #endregion
  }
}