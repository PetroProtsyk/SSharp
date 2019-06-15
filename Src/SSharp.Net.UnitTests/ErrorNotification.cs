using Xunit;
using Scripting.SSharp;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Runtime.Promotion;
using System;

namespace UnitTests
{
  public class ErrorNotification : IDisposable
  {
    public ErrorNotification()
    {
      RuntimeHost.Initialize();
      EventBroker.ClearAllSubscriptions();
    }

    public void Dispose()
    {
      RuntimeHost.CleanUp();
      EventBroker.ClearAllSubscriptions();
    }

  
    [Fact]
    public void ExecutingNonFunctionObject()
    {
      Assert.Throws<ScriptExecutionException>( () => Script.RunCode("a=[1,2,3]; a();"));
    }

    [Fact]
    public void ExecutingFunctionWithContractBroken()
    {
      Assert.Throws<ScriptVerificationException>( () => 
       Script.RunCode(@"

        function f(a)[pre(a is int & a>0); post(); invariant();]{

        }

        f(1.2);
      "));
    }
  }
}
