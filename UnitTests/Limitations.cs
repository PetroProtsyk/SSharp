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
  /// Summary description for Limitations
  /// </summary>
  [TestClass]
  public class LimitationsTests
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

    public LimitationsTests()
    {
    }

    [TestMethod]
    [ExpectedException(typeof(ScriptMethodNotFoundException))]
    public void ParamsKeywordIsNotSupported()
    {
      RuntimeHost.AddType("ParamsLimitation", typeof(ParamsLimitation));
      Assert.IsFalse(
        (bool)Script.RunCode("ParamsLimitation.Test(1,2,3,4);"));
    }
  }

  internal class ParamsLimitation
  {

    public static bool Test(params int[] args)
    {
      return true;
    }

  }
}
