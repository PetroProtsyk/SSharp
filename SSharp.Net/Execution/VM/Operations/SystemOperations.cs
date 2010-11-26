using System;

namespace Scripting.SSharp.Execution.VM.Operations
{
  using Runtime;

  internal class RetOperation : Operation
  {
    public override int Execute(IScriptContext context)
    {
      context.Result = Machine.AX;
      Machine.SetFlag(MachineFlags.Ret);
      return 0;
    }
  }

  internal class JmpOperation : Operation
  {
    public int Offset { get; set; }

    public override int Execute(IScriptContext context)
    {
      return Offset;
    }
  }

  internal class JmpIfOperation : Operation
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

  internal class JmpIfFalseOperation : Operation
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

  internal class ClearFlagOperation : Operation
  {
    public MachineFlags Flag { get; set; }

    public override int Execute(IScriptContext context)
    {
      Machine.ClearFlag(Flag);
      return 1;
    }
  }

  internal class SetFlagOperation : Operation
  {
    public MachineFlags Flag { get; set; }

    public override int Execute(IScriptContext context)
    {
      Machine.SetFlag(Flag);
      return 1;
    }
  }

  internal class RegisterOperation : Operation
  {
    private MachineRegisters _source, _destination;

    public MachineRegisters Source
    {
      get
      {
        return _source;
      }
      set
      {
        _source = value;
        switch (value)
        {
          case MachineRegisters.AX:
            _getSource = () => machine.AX;
            _setSource = x => machine.AX = x;
            _internalExecute = Exchange;
            break;
          case MachineRegisters.BX:
            _getSource = () => machine.BX;
            _setSource = x => machine.BX = x;
            _internalExecute = Exchange;
            break;
          case MachineRegisters.DX:
            _getSource = () => machine.DX;
            _setSource = x => machine.DX = x;
            _internalExecute = Exchange;
            break;
          case MachineRegisters.BBX:
            _getSource = () => machine.BBX;
            _setSource = x => machine.BBX = (bool)x;
            _internalExecute = Exchange;
            break;
          case MachineRegisters.None:
            _getSource = null;
            _setSource = null;
            _internalExecute = SetValue;
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
        return _destination;
      }
      set
      {
        _destination = value;
        switch (value)
        {
          case MachineRegisters.AX:
            _getDestination = () => machine.AX;
            _setDestination = x => machine.AX = x;
            return;
          case MachineRegisters.BX:
            _getDestination = () => machine.BX;
            _setDestination = x => machine.BX = x;
            return;
          case MachineRegisters.DX:
            _getDestination = () => machine.DX;
            _setDestination = x => machine.DX = x;
            return;
          case MachineRegisters.BBX:
            _getDestination = () => machine.BBX;
            _setDestination = x => machine.BBX = (bool)x;
            return;
          case MachineRegisters.None:
            _getDestination = null;
            _setDestination = null;
            break;
          default:
            throw new NotSupportedException("Register is not supported");
        }
      }
    }

    public object Value { get; set; }

    private Func<object> _getSource, _getDestination;
    private Func<object, object> _setSource, _setDestination;
    private ExecuteFunction _internalExecute;
    private delegate void ExecuteFunction();

    public RegisterOperation()
    {
      _internalExecute = SetValue;
    }

    public override int Execute(IScriptContext context)
    {
      _internalExecute();
      return 1;
    }

    private void SetValue()
    {
      _setDestination(Value);
    }

    private void Exchange()
    {
      object temp = _getDestination(); //GetRegister(Destination);
      _setDestination(_getSource());    //SetRegister(Destination, GetRegister(Source));
      _setSource(temp);                //SetRegister(Source, temp);   
    }

/*
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
*/

/*
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
*/
  }
}
