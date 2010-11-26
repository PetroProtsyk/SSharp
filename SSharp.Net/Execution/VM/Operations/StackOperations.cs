using System.Collections.Generic;

namespace Scripting.SSharp.Execution.VM.Operations
{
  using Runtime;
  using Runtime.Operators;

  internal class PushOperation : Operation, IStackOperation
  {
    public override int Execute(IScriptContext context)
    {
      _stack.Push(machine.AX);
      return 1;
    }

    #region IStackOperation Members
    private Stack<object> _stack;

    public Stack<object> Stack
    {
      get
      {
        return _stack;
      }
      set
      {
        _stack = value;
      }
    }

    #endregion
  }

  internal class PopOperation : Operation, IStackOperation
  {
    public override int Execute(IScriptContext context)
    {
      machine.AX = _stack.Pop();
      return 1;
    }

    #region IStackOperation Members
    private Stack<object> _stack;

    public Stack<object> Stack
    {
      get
      {
        return _stack;
      }
      set
      {
        _stack = value;
      }
    }

    #endregion
  }

  internal class PeekOperation : Operation, IStackOperation
  {
    public override int Execute(IScriptContext context)
    {
      machine.AX = _stack.Peek();
      return 1;
    }

    #region IStackOperation Members
    private Stack<object> _stack;

    public Stack<object> Stack
    {
      get
      {
        return _stack;
      }
      set
      {
        _stack = value;
      }
    }

    #endregion
  }

  internal class AddOperation : GenericOperation
  {
    public AddOperation(): base("+")
    {
    }
  }

  internal class SubOperation : GenericOperation
  {
    public SubOperation()
      : base("-")
    {
    }
  }

  internal class DivOperation : GenericOperation
  {
    public DivOperation()
      : base("/")
    {
    }

  }

  internal class ModOperation : GenericOperation
  {
    public ModOperation()
      : base("%")
    {
    }
  }

  internal class MulOperation : GenericOperation
  {
    public MulOperation()
      : base("*")
    {
    }
  }

  internal class GenericOperation : Operation, IStackOperation
  {
    private IOperator _operation;
    private string _symbol;

    public string Symbol
    {
      get { return _symbol; }
      set
      {
        _symbol = value;
        _operation = RuntimeHost.GetBinaryOperator(Symbol);
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
      _stack.Push(_operation.Evaluate(_stack.Pop(), _stack.Pop()));
      return 1;
    }

    #region IStackOperation Members
    private Stack<object> _stack;

    public Stack<object> Stack
    {
      get
      {
        return _stack;
      }
      set
      {
        _stack = value;
      }
    }

    #endregion

  }
}
