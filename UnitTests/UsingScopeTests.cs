using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Runtime.Promotion;

namespace Scripting.SSharp.UnitTests
{
  [TestClass]
  public class UsingScopeTests
  {
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
    public void UsingScopeAccessingStaticFields()
    {
      object rez = Script.RunCode(@"using(Math){ return PI; }");
      Assert.AreEqual(Math.PI, rez);
    }

    [TestMethod]
    public void UsingScopeAccessingProperties()
    {
      object rez = Script.RunCode(@"a='hello'; using(a){ return Length; }");
      Assert.AreEqual(5, rez);
      
      rez = Script.RunCode(@"a='hello'; using(a){ return ToUpper(); }");
      Assert.AreEqual("hello".ToUpper(), rez);
    }

    [TestMethod]
    public void UsingScopeAccessingMethods()
    {
      IScriptContext ctx = new ScriptContext();
      object rez = Script.RunCode(@"using(Math){ return Pow(2,10); }", ctx);
      
      Assert.AreEqual(Math.Pow(2,10), rez);
      Assert.AreEqual(RuntimeHost.NoVariable, ctx.Scope.GetItem("Pow", false));
    }

    [TestMethod]
    public void UsingScopeAccessingPropretySetter()
    {
      IScriptContext ctx = new ScriptContext();     
      UsingTest1 us = new UsingTest1();
      ctx.SetItem("a", us);

      object rez = Script.RunCode(@"a.Width = 25;", ctx);
      
      Assert.AreEqual(25, us.Width);
    }

  }

  class UsingTest1
  {
    public int Width { get; set; }
  }
}
