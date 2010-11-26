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
  /// Summary description for Constructors
  /// </summary>
  [TestClass]
  public class Constructors
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
    public void ConstructorBinding()
    {
      object result = Script.RunCode(@"a = new CtrA(); return a.Code;");

      Assert.AreEqual("Default", result);

      result = Script.RunCode(@"a = new CtrA(2); return a.Code;");

      Assert.AreEqual("2", result);

      result = Script.RunCode(@"a = new CtrA(2,3.3); return a.Code;");

      Assert.AreEqual("2 d", result);

      result = Script.RunCode(@"a = new CtrA(2,3); return a.Code;");

      Assert.AreEqual("2 3", result);

      result = Script.RunCode(@"a = new CtrA(2,'a'); return a.Code;");

      Assert.AreEqual("2 a", result);
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
