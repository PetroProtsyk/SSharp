using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Runtime.Promotion;
using System;

namespace Scripting.SSharp.UnitTests
{
  [TestClass]
  public class MethodOverloads
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
    public void ConsoleTest()
    {
      RuntimeHost.AddType("Console", typeof(ConsoleMock));

      string expected = "Value of x is 10 and y is 20";
      string actual = Script.RunCode("return Console.WriteLine(\"Value of x is {0} and y is {1}\", 10, 20);").ToString();

      Assert.AreEqual<string>(expected, actual);
    }

    [TestMethod]
    public void StringFormattingTest()
    {
      string expected = "1 + 1 = 2";
      string actual = Script.RunCode("return string.Format(\"{0} + {1} = {2}\", 1, 1, 2);").ToString();

      Assert.AreEqual<string>(expected, actual);
    }
  }

  public class ConsoleMock
  {
    public static void WriteLine(char[] buffer, int index, int count)
    {
      Console.WriteLine(buffer, index, count);
    }

    public static string WriteLine(string format, object arg0, object arg1)
    {
      Console.WriteLine(format, arg0, arg1);
      return string.Format(format, arg0, arg1);
    }
  }
}
