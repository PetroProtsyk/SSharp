using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scripting.SSharp;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Runtime.Promotion;

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
      EventBroker.ClearAllSubscriptions();
    }

    [TestCleanup]
    public void TearDown()
    {
      RuntimeHost.CleanUp();
      EventBroker.ClearAllSubscriptions();
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

    [TestMethod]
    [Description("Ensures that expression returns valid result after context was changed.")]
    public void ChangingTheScope()
    {
      Script sc = Script.Compile("return A == 1 && B == 10;");
      ScriptContext c1 = new ScriptContext();
      c1.SetItem("A", 1);
      c1.SetItem("B", 2);
      sc.Context = c1;
      bool b1 = (bool)sc.Execute();// should give false
      Assert.IsFalse(b1);

      c1 = new ScriptContext();
      c1.SetItem("A", 1);
      c1.SetItem("B", 10);
      sc.Context = c1;
      bool b2 = (bool)sc.Execute(); // should give true
      Assert.IsTrue(b2);      
    }
  }
}
