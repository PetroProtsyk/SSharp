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
  /// Summary description for FunctionScopes
  /// </summary>
  [TestClass]
  public class FunctionScopes
  {
    public FunctionScopes()
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

      Assert.AreEqual(scope, context.Scope);
      Assert.AreEqual(5, context.GetItem("c", true));
      Assert.AreEqual(4, context.GetItem("a", true));
      Assert.AreEqual(2, context.GetItem("b", true));
    }

    [TestMethod]
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

      Assert.AreEqual(scope, context.Scope);
      Assert.AreEqual(5, context.GetItem("c", true));
      Assert.AreEqual(4, context.GetItem("a", true));
      Assert.AreEqual(2, context.GetItem("b", true));
    }

  }
}
