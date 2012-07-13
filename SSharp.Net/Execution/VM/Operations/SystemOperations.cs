using System;

namespace Scripting.SSharp.Execution.VM.Operations
{
  using Runtime;

  public class RetOperation : Operation
  {
    public override int Execute(IScriptContext context)
    {
      context.Result = Machine.AX;
      Machine.SetFlag(MachineFlags.Ret);
      return 0;
    }
  }

  public class JmpOperation : Operation
  {
    public int Offset { get; set; }

    public override int Execute(IScriptContext context)
    {
      return Offset;
    }
  }

  public class JmpIfOperation : Operation
  {
    public int Offset { get; set; }

    public override int Execute(IScriptContext context)
    {
      //if (Machine.Test(MachineFlags.Cond))
      if (Machine.BBX)
      {
        Machine.BBX = false;
        return 1;
      }

      return Offset;
    }
  }

  public class JmpIfFalseOperation : Operation
  {
    public int Offset { get; set; }

    public override int Execute(IScriptContext context)
    {
      //if (Machine.Test(MachineFlags.Cond))
      if (!Machine.BBX)
      {
        Machine.BBX = false;
        return 1;
      }

      return Offset;
    }
  }

  public class ClearFlagOperation : Operation
  {
    public MachineFlags Flag { get; set; }

    public override int Execute(IScriptContext context)
    {
      Machine.ClearFlag(Flag);
      return 1;
    }
  }

  public class SetFlagOperation : Operation
  {
    public MachineFlags Flag { get; set; }

    public override int Execute(IScriptContext context)
    {
      Machine.SetFlag(Flag);
      return 1;
    }
  }

  public class RegisterOperation : Operation
  {
    private MachineRegisters source, destination;

    public MachineRegisters Source
    {
      get
      {
        return source;
      }
      set
      {
        source = value;
        switch (value)
        {
          case MachineRegisters.AX:
            getSource = () => machine.AX;
            setSource = (x) => machine.AX = x;
            internalExecute = Exchange;
            break;
          case MachineRegisters.BX:
            getSource = () => machine.BX;
            setSource = (x) => machine.BX = x;
            internalExecute = Exchange;
            break;
          case MachineRegisters.DX:
            getSource = () => machine.DX;
            setSource = (x) => machine.DX = x;
            internalExecute = Exchange;
            break;
          case MachineRegisters.BBX:
            getSource = () => machine.BBX;
            setSource = (x) => machine.BBX = (bool)x;
            internalExecute = Exchange;
            break;
          case MachineRegisters.None:
            getSource = null;
            setSource = null;
            internalExecute = SetValue;
            break;
          default:
            throw new NotSupportedException("Register is not supported");       
        }
      }
    }

    public MachineRegisters Destination
    {
      get
      {
        return destination;
      }
      set
      {
        destination = value;
        switch (value)
        {
          case MachineRegisters.AX:
            getDestination = () => machine.AX;
            setDestination = (x) => machine.AX = x;
            return;
          case MachineRegisters.BX:
            getDestination = () => machine.BX;
            setDestination = (x) => machine.BX = x;
            return;
          case MachineRegisters.DX:
            getDestination = () => machine.DX;
            setDestination = (x) => machine.DX = x;
            return;
          case MachineRegisters.BBX:
            getDestination = () => machine.BBX;
            setDestination = (x) => machine.BBX = (bool)x;
            return;
          case MachineRegisters.None:
            getDestination = null;
            setDestination = null;
            break;
          default:
            throw new NotSupportedException("Register is not supported");
        }
      }
    }

    public object Value { get; set; }

    private Func<object> getSource, getDestination;
    private Func<object, object> setSource, setDestination;
    private ExecuteFunction internalExecute;
    private delegate void ExecuteFunction();

    public RegisterOperation()
    {
      internalExecute = SetValue;
    }

    public override int Execute(IScriptContext context)
    {
      internalExecute();
      return 1;
    }

    private void SetValue()
    {
      setDestination(Value);
    }

    private void Exchange()
    {
      object temp = getDestination(); //GetRegister(Destination);
      setDestination(getSource());    //SetRegister(Destination, GetRegister(Source));
      setSource(temp);                //SetRegister(Source, temp);   
    }

    private void SetRegister(MachineRegisters destination, object value)
    {
      switch (destination)
      {
        case MachineRegisters.AX:
          Machine.AX = value;
          return;
        case MachineRegisters.BX:
          Machine.BX = value;
          return;
        case MachineRegisters.DX:
          Machine.DX = value;
          return;
        case MachineRegisters.BBX:
          Machine.BBX = (bool)value;
          return;
        default:
          throw new NotSupportedException("Register is not supported");
      }
    }

    private object GetRegister(MachineRegisters destination)
    {
      switch (destination)
      {
        case MachineRegisters.AX:
          return Machine.AX;
        case MachineRegisters.BX:
          return Machine.BX;
        case MachineRegisters.DX:
          return Machine.DX;
        case MachineRegisters.BBX:
          return Machine.BBX;
        default:
          throw new NotSupportedException("Register is not supported");
      }
    }
  }
}
