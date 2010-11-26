using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Runtime.Promotion;
using Scripting.SSharp;
using Scripting.SSharp.Execution.VM;
using Scripting.SSharp.Execution.VM.Operations;

namespace ScriptNET.Execution.UnitTests
{
  /// <summary>
  /// Summary description for UnitTest1
  /// </summary>
  [TestClass]
  public class MachineTests
  {
    [TestInitialize]
    public void Setup()
    {
      RuntimeHost.Initialize();
    }

    [TestCleanup]
    public void TearDown()
    {
      RuntimeHost.CleanUp();
    }
   
    [TestMethod]
    public void MachineExection()
    {
      IScriptContext context = new ScriptContext();

      //Example 0: Machine

      ExecutableMachine machine = ExecutableMachine.Create();
      SetValueOperation sv = machine.CreateOperation<SetValueOperation>();
      sv.Id = "a";
      
      machine.AX = "Hello World";
      machine.CreateOperation<RetOperation>();
      machine.Execute(context);

      object rez = context.GetItem("a", true);

      Assert.AreEqual("Hello World", rez);
    }

    [TestMethod]
    public void Benchmark()
    {
      IScriptContext context = new ScriptContext();

      //Example 0: Machine

      ExecutableMachine machine = ExecutableMachine.Create();

      int iterations = 2;// 10000000;

      //loops = 10000000
      ValueOperation op0 = machine.CreateOperation<ValueOperation>();
      op0.Value = iterations;      
      SetValueOperation op1 = machine.CreateOperation<SetValueOperation>();
      op1.Id = "loops";

      // counter = 0
      ValueOperation op2 = machine.CreateOperation<ValueOperation>();
      op2.Value = 0;
      SetValueOperation op3 = machine.CreateOperation<SetValueOperation>();
      op3.Id = "counter";

      //while (loops > 0)
      RegisterOperation op4 = machine.CreateOperation<RegisterOperation>();
      op4.Destination = MachineRegisters.BX;
      op4.Value = 0;
      GetValueOperation op5 = machine.CreateOperation<GetValueOperation>();
      op5.Id = "loops";      
      CmpOperation op6 = machine.CreateOperation<CmpOperation>();
      JmpIfOperation op7 = machine.CreateOperation<JmpIfOperation>();
      op7.Offset = 8;
      //loops = loops -1;
      GetValueOperation op8 = machine.CreateOperation<GetValueOperation>();
      op8.Id = "loops";
      DecOperation op9 = machine.CreateOperation<DecOperation>();
      SetValueOperation op10 = machine.CreateOperation<SetValueOperation>();
      op10.Id = "loops";

      //counter = counter + 1;
      GetValueOperation op11 = machine.CreateOperation<GetValueOperation>();
      op11.Id = "counter";
      IncOperation op12 = machine.CreateOperation<IncOperation>();
      SetValueOperation op13 = machine.CreateOperation<SetValueOperation>();
      op13.Id = "counter";

      JmpOperation op14 = machine.CreateOperation<JmpOperation>();
      op14.Offset = -10;

      machine.CreateOperation<RetOperation>();
      machine.Execute(context);

      object rez = context.GetItem("counter", true);

      Assert.AreEqual(iterations, rez);
    }

  }
}
