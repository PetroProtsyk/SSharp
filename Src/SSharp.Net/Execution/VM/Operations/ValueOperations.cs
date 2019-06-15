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

  internal class GetValueOperation : Operation
  {
    public string Id { get; set; }

    public override int Execute(IScriptContext context)
    {
      Machine.AX = context.GetItem(Id, true);
      return 1;
    }
  }

  internal class SetValueOperation : Operation
  {
    public string Id { get; set; }

    public override int Execute(IScriptContext context)
    {
      context.SetItem(Id, Machine.AX);
      return 1;
    }
  }

  internal class ValueOperation : Operation
  {
    public object Value { get; set; }

    public override int Execute(IScriptContext context)
    {
      Machine.AX = Value;
      return 1;
    }
  }
}
