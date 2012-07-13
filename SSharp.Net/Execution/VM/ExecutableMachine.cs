using System;
using System.Collections.Generic;

namespace Scripting.SSharp.Execution.VM
{
  using Operations;
  using Runtime;

  public class ExecutableMachine
  {
    #region Fields
    private readonly List<IOperation> commands = new List<IOperation>();

    private int current;
    private MachineFlags flags;
    #endregion

    #region Registers
    public object AX { get; set; }
    public object BX { get; set; }
    public object DX { get; set; }

    public bool BBX { get; set; }

    //Should be read-only, see stack commands
    public readonly Stack<object> Stack = new Stack<object>();

    internal protected int CommandCount { get { return commands.Count; } }
    #endregion

    #region Properties
    public IOperation Current
    {
      get
      {
        if (current >= 0 && current < commands.Count)
          return commands[current];

        return null;
      }
    }
    #endregion

    #region Execution
    public void Reset()
    {
      current = 0;
      flags = MachineFlags.Clear;

      AX = null;
      BX = null;
      DX = null;
      BBX = false;

      Stack.Clear();
    }

    public void Execute(IScriptContext context)
    {
      IOperation[] arCommands = commands.ToArray();

      while ((flags & MachineFlags.Ret) != MachineFlags.Ret)
        current += arCommands[current].Execute(context);
      
      //NOTE: Peformance      
      //while (!Test(MachineFlags.Ret))
      //  Step(context);

      Reset();
    }

    public void Step(IScriptContext context)
    {
      if (Test(MachineFlags.Ret))
        throw new MachineException("Ret command has been already executed, can't continue evaluation");

      if (current > commands.Count - 1)
        throw new MachineException("Command execution flow exception");

      current += commands[current].Execute(context);
    }

    public bool Test(MachineFlags flag)
    {
      return (flags & flag) == flag;
    }

    public void SetFlag(MachineFlags flag)
    {
      flags |= flag;
    }

    public void ClearFlag(MachineFlags flag)
    {
      flags ^= flag;
    }
    #endregion

    #region Operations
    readonly Dictionary<Type, IOperationBuilder> registeredOperations = new Dictionary<Type, IOperationBuilder>();

    public void RegisterOperation(Type operationType, IOperationBuilder builder)
    {
      registeredOperations.Add(operationType, builder);
    }

    public void RegisterOperation<T>(IOperationBuilder builder) where T : IOperation
    {
      RegisterOperation(typeof(T), builder);
    }

    public IOperation CreateOperation(Type operationType)
    {
      IOperation op = registeredOperations[operationType].Create();
      commands.Add(op);
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
      ExecutableMachine machine = new ExecutableMachine();

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
  public enum MachineFlags
  {
    Clear = 0,
    Ret = 1 << 0,
    Break = 1 << 1,
    Cond = 1 << 2
  }
  
  [Flags]
  public enum MachineRegisters
  {
    None = 0,
    AX = 1 << 0,
    BX = 1 << 1,
    DX = 1 << 2,
    BBX = 1 << 3
  }

  public class MachineException : Exception
  {
    public MachineException(string message)
      : base(message)
    {
    }
  }
}
