using System.Collections.Generic;

namespace Scripting.SSharp.Execution.VM.Operations
{
  using Runtime;
  using Runtime.Operators;

  public class PushOperation : Operation, IStackOperation
  {
    public override int Execute(IScriptContext context)
    {
      stack.Push(machine.AX);
      return 1;
    }

    #region IStackOperation Members
    private Stack<object> stack;

    public Stack<object> Stack
    {
      get
      {
        return stack;
      }
      set
      {
        stack = value;
      }
    }

    #endregion
  }

  public class PopOperation : Operation, IStackOperation
  {
    public override int Execute(IScriptContext context)
    {
      machine.AX = stack.Pop();
      return 1;
    }

    #region IStackOperation Members
    private Stack<object> stack;

    public Stack<object> Stack
    {
      get
      {
        return stack;
      }
      set
      {
        stack = value;
      }
    }

    #endregion
  }

  public class PeekOperation : Operation, IStackOperation
  {
    public override int Execute(IScriptContext context)
    {
      machine.AX = stack.Peek();
      return 1;
    }

    #region IStackOperation Members
    private Stack<object> stack;

    public Stack<object> Stack
    {
      get
      {
        return stack;
      }
      set
      {
        stack = value;
      }
    }

    #endregion
  }

  public class AddOperation : GenericOperation
  {
    public AddOperation(): base("+")
    {
    }
  }

  public class SubOperation : GenericOperation
  {
    public SubOperation()
      : base("-")
    {
    }
  }

  public class DivOperation : GenericOperation
  {
    public DivOperation()
      : base("/")
    {
    }

  }

  public class ModOperation : GenericOperation
  {
    public ModOperation()
      : base("%")
    {
    }
  }

  public class MulOperation : GenericOperation
  {
    public MulOperation()
      : base("*")
    {
    }
  }

  public class GenericOperation : Operation, IStackOperation
  {
    private IOperator operation;
    private string symbol;

    public string Symbol
    {
      get { return symbol; }
      set
      {
        symbol = value;
        operation = RuntimeHost.GetBinaryOperator(Symbol);
      }
    }

    public GenericOperation()
    {
    }

    public GenericOperation(string symbol)
    {
      Symbol = symbol;
    }

    public override int Execute(IScriptContext context)
    {
      stack.Push(operation.Evaluate(stack.Pop(), stack.Pop()));
      return 1;
    }

    #region IStackOperation Members
    private Stack<object> stack;

    public Stack<object> Stack
    {
      get
      {
        return stack;
      }
      set
      {
        stack = value;
      }
    }

    #endregion

  }
}
