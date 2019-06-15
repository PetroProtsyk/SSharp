using Xunit;
using Scripting.SSharp;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Runtime.Promotion;
using System;

namespace UnitTests
{
  /// <summary>
  /// Summary description for Expressions
  /// </summary>
  public class Expressions : IDisposable
  {
    public Expressions()
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
    public void BinaryExpression()
    {
      Assert.Equal(5, Script.RunCode("3+2", true));
    }

    [Fact]
    public void CallingToQualifiedName()
    {
      Assert.Equal(1024, Script.RunCode("(int)Math.Pow(2,10)", true));
        }

    // Ensures that expression returns valid result after context was changed.
    [Fact]
    public void ChangingTheScope()
    {
      Script sc = Script.Compile("return A == 1 && B == 10;");
      ScriptContext c1 = new ScriptContext();
      c1.SetItem("A", 1);
      c1.SetItem("B", 2);
      sc.Context = c1;
      bool b1 = (bool)sc.Execute();// should give false
      Assert.False(b1);

      c1 = new ScriptContext();
      c1.SetItem("A", 1);
      c1.SetItem("B", 10);
      sc.Context = c1;
      bool b2 = (bool)sc.Execute(); // should give true
      Assert.True(b2);      
    }
  }
}
