using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scripting.SSharp;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Runtime.Promotion;

namespace UnitTests
{
  [TestClass]
  public class ErrorNotification
  {
    [TestInitialize]
    public void Setup()
    {
      RuntimeHost.Initialize();
      EventBroker.ClearAllSubscriptions();
    }

    [TestCleanup]
    public void TearDown()
    {
      RuntimeHost.CleanUp();
      EventBroker.ClearAllSubscriptions();
    }

  
    [TestMethod]
    [ExpectedException(typeof(ScriptExecutionException))]
    public void ExecutingNonFunctionObject()
    {
      Script.RunCode("a=[1,2,3]; a();");
    }

    [TestMethod]
    [ExpectedException(typeof(ScriptVerificationException))]
    public void ExecutingFunctionWithContractBroken()
    {
      Script.RunCode(@"

        function f(a)[pre(a is int & a>0); post(); invariant();]{

        }

        f(1.2);
      ");
    }
  }
}
