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

namespace Scripting.SSharp.Execution.VM.Operations
{
  using Runtime;

  internal class ObjectMemberOperation : Operation
  {
    public string MemberName { get; set; }

    public override int Execute(IScriptContext context)
    {
      var parameters = new object[(int)Machine.BX];
      var stack = Machine.Stack;

      for (int i = 0; i < parameters.Length; i++)
        parameters[parameters.Length - 1 - i] = stack.Pop();

      var target = stack.Pop();
      
      var bind = RuntimeHost.Binder.BindToMethod(target, MemberName, null, parameters);

      Machine.AX = bind.Invoke(context, parameters);

      return 1;
    }
  }

}
