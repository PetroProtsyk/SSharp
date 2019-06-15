using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Runtime.Promotion;

namespace Scripting.SSharp.UnitTests
{
  public class UsingScopeTests : IDisposable
  {
    public UsingScopeTests()
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
    public void UsingScopeAccessingStaticFields()
    {
      object rez = Script.RunCode(@"using(Math){ return PI; }");
      Assert.Equal(Math.PI, rez);
    }

    [Fact]
    public void UsingScopeAccessingProperties()
    {
      object rez = Script.RunCode(@"a='hello'; using(a){ return Length; }");
      Assert.Equal(5, rez);
      
      rez = Script.RunCode(@"a='hello'; using(a){ return ToUpper(); }");
      Assert.Equal("hello".ToUpper(), rez);
    }

    [Fact]
    public void UsingScopeAccessingMethods()
    {
      IScriptContext ctx = new ScriptContext();
      object rez = Script.RunCode(@"using(Math){ return Pow(2,10); }", ctx);
      
      Assert.Equal(Math.Pow(2,10), rez);
      Assert.Equal(RuntimeHost.NoVariable, ctx.Scope.GetItem("Pow", false));
    }

    [Fact]
    public void UsingScopeAccessingPropretySetter()
    {
      IScriptContext ctx = new ScriptContext();     
      UsingTest1 us = new UsingTest1();
      ctx.SetItem("a", us);

      object rez = Script.RunCode(@"a.Width = 25;", ctx);
      
      Assert.Equal(25, us.Width);
    }

  }

  class UsingTest1
  {
    public int Width { get; set; }
  }
}
