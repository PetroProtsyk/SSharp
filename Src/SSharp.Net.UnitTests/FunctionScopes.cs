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
  /// Summary description for FunctionScopes
  /// </summary>
  public class FunctionScopes : IDisposable
  {
    public FunctionScopes()
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
    public void SimpleFunctionScope()
    {
      ScriptContext context = new ScriptContext();
      IScriptScope scope = context.Scope;
      Script.RunCode(@"
        a = 4; b = 2; c = 3;
        function test1(a,b)
          global(c)
        {
          c = a+b;
          a = 15;
        }

        test1(2,3);", context);

      Assert.Equal(scope, context.Scope);
      Assert.Equal(5, context.GetItem("c", true));
      Assert.Equal(4, context.GetItem("a", true));
      Assert.Equal(2, context.GetItem("b", true));
    }

    [Fact]
    public void SimpleFunctionScopeWithContract()
    {
      ScriptContext context = new ScriptContext();
      IScriptScope scope = context.Scope;
      Script.RunCode(@"
        a = 4; b = 2; c = 3;
        function test1(a,b)
          global(c)
          [
           pre(a>0);
           post();
           invariant();
          ]
        {
          c = a+b;
          a = 15;
        }

        test1(2,3);", context);

      Assert.Equal(scope, context.Scope);
      Assert.Equal(5, context.GetItem("c", true));
      Assert.Equal(4, context.GetItem("a", true));
      Assert.Equal(2, context.GetItem("b", true));
    }

    [Fact]
    public void RedefineGlobalVariableInFunctionNotAllowed()
    {
      Assert.Throws<ScriptExecutionException>(() => 
        Script.RunCode(@"
         var x = 5;
         function f() global(x)
         {
             var x;
         } 
         f();
         return x;
      "));
    }

    [Fact]
    public void ReassignGlobalVariableInFunctionNotAllowed()
    {
      Assert.Throws<ScriptExecutionException>(() => {
          ScriptContext context = new ScriptContext();
          var result = Script.RunCode(@"
                 var x = 5;
                 function f() global (x)
                 {
                     var x = 10;
                 } 
                 f();
                 return x;
              ", context);

          Assert.Equal(5, context.GetItem("x", true));
      });
    }

    [Fact]
    public void RedefineVariableInFunction() {
        ScriptContext context = new ScriptContext();
        var result = Script.RunCode(@"
         var x = 5;
         function f()
         {
             var x;
             x = 10;
         } 
         f();
         return x;
      ", context);

      Assert.Equal(5, context.GetItem("x", true));
    }
  }
}
