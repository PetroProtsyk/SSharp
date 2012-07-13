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
  /// UnitTest operators,  OperatorsDispatching
  /// </summary>
  [TestClass]
  public class OperatorsDispatching
  {
    public OperatorsDispatching()
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
    public void BaseOperators_Plus()
    {
      object rez = null;

      rez = Script.RunCode("return 1+1;");
      Assert.AreEqual(2, rez);

      rez = Script.RunCode("return 1.2+1;");
      Assert.AreEqual(2.2, rez);

      rez = Script.RunCode("return '1'+1;");
      Assert.AreEqual("11", rez);

      rez = Script.RunCode("return 'Hello '+1+' Text';");
      Assert.AreEqual("Hello 1 Text", rez);
    }

    [TestMethod]
    public void BaseOperators_Minus()
    {
      object rez = null;

      rez = Script.RunCode("return 10-1;");
      Assert.AreEqual(9, rez);

      rez = Script.RunCode("return 1.2-1;");
      Assert.AreEqual((double)1.2 - 1, rez);
    }

    [ExpectedException(typeof(NotSupportedException))]
    [TestMethod]
    public void BaseOperators_Minus1()
    {
      object rez = null;

      rez = Script.RunCode("return '1'-1;");
      Assert.AreEqual("11", rez);
    }

    [TestMethod]
    public void BaseOperators_Mul()
    {
      object rez = null;

      rez = Script.RunCode("return 10*12;");
      Assert.AreEqual(10*12, rez);

      rez = Script.RunCode("return 3.2*3;");
      Assert.AreEqual((double)3.2*3, rez);

      rez = Script.RunCode("return 3.5*21.5;");
      Assert.AreEqual((double)3.5 * 21.5, rez);
    }

    [TestMethod]
    public void BaseOperators_Div()
    {
      object rez = null;

      rez = Script.RunCode("return 6/2;");
      Assert.AreEqual(6 / 2, rez);

      rez = Script.RunCode("return 10/12;");
      Assert.AreEqual(10 / 12, rez);

      rez = Script.RunCode("return 45.43/12.3;");
      Assert.AreEqual((double)45.43 / 12.3, rez);

      rez = Script.RunCode("return 3.5/21;");
      Assert.AreEqual((double)3.5 / 21, rez);

      rez = Script.RunCode("return 3/21.2;");
      Assert.AreEqual(3 / (double)21.2, rez);
    }

    [TestMethod]
    public void BaseOperators_Mod()
    {
      object rez = null;

      rez = Script.RunCode("return 6%2;");
      Assert.AreEqual(6 % 2, rez);

      rez = Script.RunCode("return 10%12;");
      Assert.AreEqual(10 % 12, rez);

      rez = Script.RunCode("return 45.43%12.3;");
      Assert.AreEqual((double)45.43 % 12.3, rez);
    }

    [TestMethod]
    public void BaseOperators_Pow()
    {
      object rez = null;

      rez = Script.RunCode("return 6^2;");
      Assert.AreEqual(Math.Pow(6, 2), rez);

      rez = Script.RunCode("return 10^12;");
      Assert.AreEqual(Math.Pow(10, 12), rez);
    }
  }
}
