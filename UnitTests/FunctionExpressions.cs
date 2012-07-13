using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Runtime.Promotion;
using Scripting.SSharp;

namespace UnitTests
{
  /// <summary>
  /// Summary description for FunctionExpressions
  /// </summary>
  [TestClass]
  public class FunctionExpressions
  {
    public FunctionExpressions()
    {
    }

    [TestInitialize]
    public void Setup()
    {
      RuntimeHost.Initialize();
      EventBroker.ClearAllEvents();
    }

    [TestCleanup]
    public void TearDown()
    {
      RuntimeHost.CleanUp();
      EventBroker.ClearAllEvents();
    }

    [TestMethod]
    public void AnonimousFunctionWithoutParameters()
    {
      object rez = Script.RunCode(
         @"
            stack = [
                    Test -> function() { return 'Hello Test';}
                 ];
            return stack.Test();
          "
         );

      Assert.AreEqual("Hello Test", rez);
    }


    [TestMethod]
    [ExpectedException(typeof(ScriptIdNotFoundException))]
    public void RequestingMissingGlobalNameRaiseException()
    {
      Script result =
         Script.Compile(
         @"
            Test = function (item) global(y)
                         {
                           y = item;
                         };
            Test(2);
          "
         );

      object resultVal = result.Execute();
    }

  }
}
