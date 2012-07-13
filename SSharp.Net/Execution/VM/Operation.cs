using System.Collections.Generic;

namespace Scripting.SSharp.Execution.VM
{
  using Runtime;
  using Compilers.Dom;

  public interface IOperation
  {
    ExecutableMachine Machine { get; }

    CodeObject SourceObject { get; }

    int Execute(IScriptContext context);
  }

  public interface IStackOperation
  {
    Stack<object> Stack { get; set; }
  }

  public interface IOperationBuilder
  {
    IOperation Create();
  }

  public abstract class Operation : IOperation
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

  public class BaseOperationBuilder<T> : IOperationBuilder 
    where T : Operation, new()
  {
    protected readonly ExecutableMachine machine;

    public BaseOperationBuilder(ExecutableMachine machine)
    {
      this.machine = machine;
    }

    #region IOperationBuilder Members

    public IOperation Create()
    {
      Operation op = new T();
      op.Machine = machine;

      IStackOperation stack_op = op as IStackOperation;
      if (stack_op != null)
        stack_op.Stack = machine.Stack;

      return op;
    }

    #endregion
  }
}
