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
