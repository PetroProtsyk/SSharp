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

using System.Collections.Generic;

namespace Scripting.SSharp.Execution.VM
{
  using Runtime;
  using Compilers.Dom;

  internal interface IOperation
  {
    ExecutableMachine Machine { get; }

    CodeObject SourceObject { get; }

    int Execute(IScriptContext context);
  }

  internal interface IStackOperation
  {
    Stack<object> Stack { get; set; }
  }

  internal interface IOperationBuilder
  {
    IOperation Create();
  }

  internal abstract class Operation : IOperation
  {
    /// <summary>
    /// Use it instead of property for performance
    /// </summary>
    protected ExecutableMachine machine;

    public ExecutableMachine Machine
    {
      get { return machine; }
      internal set { machine = value; }
    }

    public CodeObject SourceObject { get; set; }

    public abstract int Execute(IScriptContext context);
  }

  internal class BaseOperationBuilder<T> : IOperationBuilder 
    where T : Operation, new()
  {
    protected readonly ExecutableMachine Machine;

    public BaseOperationBuilder(ExecutableMachine machine)
    {
      Machine = machine;
    }

    #region IOperationBuilder Members

    public IOperation Create()
    {
      Operation op = new T { Machine = Machine };

      var stackOp = op as IStackOperation;
      if (stackOp != null)
        stackOp.Stack = Machine.Stack;

      return op;
    }

    #endregion
  }
}
