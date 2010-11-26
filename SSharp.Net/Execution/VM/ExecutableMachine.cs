using System;
using System.Collections.Generic;

namespace Scripting.SSharp.Execution.VM
{
  using Operations;
  using Runtime;

  internal class ExecutableMachine
  {
    #region Fields
    private readonly List<IOperation> _commands = new List<IOperation>();

    private int _current;
    private MachineFlags _flags;
    #endregion

    #region Registers
    public object AX { get; set; }
    public object BX { get; set; }
    public object DX { get; set; }

    public bool BBX { get; set; }

    //Should be read-only, see stack commands
    public readonly Stack<object> Stack = new Stack<object>();

    internal protected int CommandCount { get { return _commands.Count; } }
    #endregion

    #region Properties
    public IOperation Current
    {
      get
      {
        if (_current >= 0 && _current < _commands.Count)
          return _commands[_current];

        return null;
      }
    }
    #endregion

    #region Execution
    public void Reset()
    {
      _current = 0;
      _flags = MachineFlags.Clear;

      AX = null;
      BX = null;
      DX = null;
      BBX = false;

      Stack.Clear();
    }

    public void Execute(IScriptContext context)
    {
      IOperation[] arCommands = _commands.ToArray();

      while ((_flags & MachineFlags.Ret) != MachineFlags.Ret)
        _current += arCommands[_current].Execute(context);
      
      //NOTE: Peformance      
      //while (!Test(MachineFlags.Ret))
      //  Step(context);

      Reset();
    }

    public void Step(IScriptContext context)
    {
      if (Test(MachineFlags.Ret))
        throw new MachineException("Ret command has been already executed, can't continue evaluation");

      if (_current > _commands.Count - 1)
        throw new MachineException("Command execution flow exception");

      _current += _commands[_current].Execute(context);
    }

    public bool Test(MachineFlags flag)
    {
      return (_flags & flag) == flag;
    }

    public void SetFlag(MachineFlags flag)
    {
      _flags |= flag;
    }

    public void ClearFlag(MachineFlags flag)
    {
      _flags ^= flag;
    }
    #endregion

    #region Operations
    readonly Dictionary<Type, IOperationBuilder> _registeredOperations = new Dictionary<Type, IOperationBuilder>();

    public void RegisterOperation(Type operationType, IOperationBuilder builder)
    {
      _registeredOperations.Add(operationType, builder);
    }

    public void RegisterOperation<T>(IOperationBuilder builder) where T : IOperation
    {
      RegisterOperation(typeof(T), builder);
    }

    public IOperation CreateOperation(Type operationType)
    {
      IOperation op = _registeredOperations[operationType].Create();
      _commands.Add(op);
      return op;
    }

    public T CreateOperation<T>() where T : IOperation
    {
      return (T)CreateOperation(typeof(T));
    }
    #endregion

    #region Construction
    protected ExecutableMachine()
    {
    }

    public static ExecutableMachine Create()
    {
      var machine = new ExecutableMachine();

      machine.RegisterOperation<RetOperation>(new BaseOperationBuilder<RetOperation>(machine));
      machine.RegisterOperation<GetValueOperation>(new BaseOperationBuilder<GetValueOperation>(machine));
      machine.RegisterOperation<SetValueOperation>(new BaseOperationBuilder<SetValueOperation>(machine));
      machine.RegisterOperation<ValueOperation>(new BaseOperationBuilder<ValueOperation>(machine));
      machine.RegisterOperation<JmpOperation>(new BaseOperationBuilder<JmpOperation>(machine));
      machine.RegisterOperation<JmpIfOperation>(new BaseOperationBuilder<JmpIfOperation>(machine));
      machine.RegisterOperation<JmpIfFalseOperation>(new BaseOperationBuilder<JmpIfFalseOperation>(machine));
      machine.RegisterOperation<ClearFlagOperation>(new BaseOperationBuilder<ClearFlagOperation>(machine));
      machine.RegisterOperation<SetFlagOperation>(new BaseOperationBuilder<SetFlagOperation>(machine));
      machine.RegisterOperation<RegisterOperation>(new BaseOperationBuilder<RegisterOperation>(machine));
      machine.RegisterOperation<CmpOperation>(new BaseOperationBuilder<CmpOperation>(machine));

      machine.RegisterOperation<PushOperation>(new BaseOperationBuilder<PushOperation>(machine));
      machine.RegisterOperation<PopOperation>(new BaseOperationBuilder<PopOperation>(machine));
      machine.RegisterOperation<PeekOperation>(new BaseOperationBuilder<PeekOperation>(machine));

      machine.RegisterOperation<ObjectMemberOperation>(new BaseOperationBuilder<ObjectMemberOperation>(machine));

      machine.RegisterOperation<AddOperation>(new BaseOperationBuilder<AddOperation>(machine));
      machine.RegisterOperation<SubOperation>(new BaseOperationBuilder<SubOperation>(machine));
      machine.RegisterOperation<MulOperation>(new BaseOperationBuilder<MulOperation>(machine));
      machine.RegisterOperation<DivOperation>(new BaseOperationBuilder<DivOperation>(machine));
      machine.RegisterOperation<ModOperation>(new BaseOperationBuilder<ModOperation>(machine));
      machine.RegisterOperation<GenericOperation>(new BaseOperationBuilder<GenericOperation>(machine));
    
      machine.RegisterOperation<IncOperation>(new BaseOperationBuilder<IncOperation>(machine));
      machine.RegisterOperation<DecOperation>(new BaseOperationBuilder<DecOperation>(machine));
      
      return machine;
    }

    #endregion
  }

  [Flags]
  internal enum MachineFlags
  {
    Clear = 0,
    Ret = 1 << 0,
    Break = 1 << 1,
    Cond = 1 << 2
  }
  
  [Flags]
  internal enum MachineRegisters
  {
    None = 0,
    /// <summary>
    /// Evaluation result register
    /// </summary>
    AX = 1 << 0,
    /// <summary>
    /// Evaluation parameter register
    /// </summary>
    BX = 1 << 1,
    /// <summary>
    /// Reserved evaluation register
    /// </summary>
    DX = 1 << 2,
    /// <summary>
    /// Boolean result register
    /// </summary>
    BBX = 1 << 3
  }

  internal class MachineException : Exception
  {
    public MachineException(string message)
      : base(message)
    {
    }
  }
}
