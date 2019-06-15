using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Runtime.Promotion;
using Scripting.SSharp;

namespace UnitTests
{
  /// <summary>
  /// Summary description for FunctionExpressions
  /// </summary>
  public class FunctionExpressions : IDisposable
  {
    public FunctionExpressions()
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

      Assert.Equal("Hello Test", rez);
    }


    [Fact]
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
      Assert.Throws<ScriptIdNotFoundException>(() => result.Execute());
    }

    [Fact]
    public void VoidFunction() {
        Script result =
           Script.Compile(
           @"
            function f(){
            };

            i = 1;
            f();
          "
           );

        object resultVal = result.Execute();

        Assert.Equal(RuntimeHost.NullValue, resultVal);
    }

  }
}
