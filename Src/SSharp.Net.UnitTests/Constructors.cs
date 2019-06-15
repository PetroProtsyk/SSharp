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
  /// Summary description for Constructors
  /// </summary>
  public class Constructors : IDisposable
  {
    public Constructors()
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
    public void ConstructorBinding()
    {
      object result = Script.RunCode(@"a = new CtrA(); return a.Code;");

      Assert.Equal("Default", result);

      result = Script.RunCode(@"a = new CtrA(2); return a.Code;");

      Assert.Equal("2", result);

      result = Script.RunCode(@"a = new CtrA(2,3.3); return a.Code;");

      Assert.Equal("2 d", result);

      result = Script.RunCode(@"a = new CtrA(2,3); return a.Code;");

      Assert.Equal("2 3", result);

      result = Script.RunCode(@"a = new CtrA(2,'a'); return a.Code;");

      Assert.Equal("2 a", result);
    }
  
  }

  public class CtrA
  {
    public string Code { get; private set; }

    public CtrA()
    {
      Code = "Default";
    }

    public CtrA(int x)
    {
      Code = x.ToString();
    }

    public CtrA(int x, double y)
    {
      Code = x.ToString()+" d";
    }
    
    public CtrA(int x, int y)
    {
      Code = x.ToString()+" "+y.ToString();
    }

    public CtrA(int x, string y)
    {
      Code = x.ToString() + " " + y;
    }

  }
}
