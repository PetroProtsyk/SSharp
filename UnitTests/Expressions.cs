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
  /// Summary description for Expressions
  /// </summary>
  [TestClass]
  public class Expressions
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

    public Expressions()
    {
    }

    [TestMethod]
    public void BinaryExpression()
    {
      Assert.AreEqual(5, Script.RunCode("3+2", true));
    }

    [TestMethod]
    public void CallingToQualifiedName()
    {
      Assert.AreEqual(1024, Script.RunCode("(int)Math.Pow(2,10)", true));
    }
  }
}
