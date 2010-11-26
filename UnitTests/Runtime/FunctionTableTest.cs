using System;
using System.Collections;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.UnitTests.Runtime
{
  [TestClass]
  public class FunctionTableTest
  {
    private class TestFunction : IInvokable
    {
      #region IInvokable Members

      public bool CanInvoke()
      {
        return true;
      }

      public object Invoke(IScriptContext context, object[] args)
      {
        return null;
      }

      #endregion
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void AddFunctionWithGenericsRequiresName()
    {
      new FunctionTable().AddFunction<TestFunction>(null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void AddFunctionWithTypeRequiresName()
    {
      new FunctionTable().AddFunction(null, typeof(TestFunction));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void AddFunctionWithTypeRequiresType()
    {
      new FunctionTable().AddFunction("test", null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ContainsRequiresName()
    {
      new FunctionTable().Contains(string.Empty);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void AddFunctionWithTypeExpectsProperType()
    {
      new FunctionTable().AddFunction("test", typeof(string));
    }

    [TestMethod]
    public void ShouldAddFunctionWithGenerics()
    {
      FunctionTable table = new FunctionTable();
      table.AddFunction<TestFunction>("test");

      Assert.AreEqual<int>(1, table.Count());
      Assert.IsTrue(table.Contains("test"));
      Assert.AreEqual<string>("test", table.First().Key);
      Assert.AreEqual<Type>(typeof(TestFunction), table.First().Value);
    }

    [TestMethod]
    public void ShouldAddFunctionWithType()
    {
      FunctionTable table = new FunctionTable();
      table.AddFunction("test", typeof(TestFunction));

      Assert.AreEqual<int>(1, table.Count());
      Assert.IsTrue(table.Contains("test"));
      Assert.AreEqual<string>("test", table.First().Key);
      Assert.AreEqual<Type>(typeof(TestFunction), table.First().Value);
    }

    [TestMethod]
    public void ShouldReturnEnumerator()
    {
      FunctionTable table = new FunctionTable();
      table.AddFunction<TestFunction>("test");

      int count = 0;
      
      IEnumerator enumerator = ((IEnumerable)table).GetEnumerator();
      while (enumerator.MoveNext()) count++;

      Assert.AreEqual<int>(1, count);
    }
  }
}
