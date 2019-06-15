using System;
using System.Collections;
using System.Linq;
using Xunit;
using Scripting.SSharp.Runtime;

namespace UnitTests
{
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

    [Fact]
    public void AddFunctionWithGenericsRequiresName()
    {
      Assert.Throws<ArgumentNullException>(() => new FunctionTable().AddFunction<TestFunction>(null));
    }

    [Fact]
    public void AddFunctionWithTypeRequiresName()
    {
      Assert.Throws<ArgumentNullException>(() => new FunctionTable().AddFunction(null, typeof(TestFunction)));
    }

    [Fact]
    public void AddFunctionWithTypeRequiresType()
    {
      Assert.Throws<ArgumentNullException>(() => new FunctionTable().AddFunction("test", null));
    }

    [Fact]
    public void ContainsRequiresName()
    {
      Assert.Throws<ArgumentException>(() => new FunctionTable().Contains(string.Empty));
    }

    [Fact]
    public void AddFunctionWithTypeExpectsProperType()
    {
      Assert.Throws<ArgumentException>(() => new FunctionTable().AddFunction("test", typeof(string)));
    }

    [Fact]
    public void ShouldAddFunctionWithGenerics()
    {
      FunctionTable table = new FunctionTable();
      table.AddFunction<TestFunction>("test");

      Assert.Single(table);
      Assert.True(table.Contains("test"));
      Assert.Equal("test", table.First().Key);
      Assert.Equal(typeof(TestFunction), table.First().Value);
    }

    [Fact]
    public void ShouldAddFunctionWithType()
    {
      FunctionTable table = new FunctionTable();
      table.AddFunction("test", typeof(TestFunction));

      Assert.Single(table);
      Assert.True(table.Contains("test"));
      Assert.Equal("test", table.First().Key);
      Assert.Equal(typeof(TestFunction), table.First().Value);
    }

    [Fact]
    public void ShouldReturnEnumerator()
    {
      FunctionTable table = new FunctionTable();
      table.AddFunction<TestFunction>("test");

      int count = 0;
      
      IEnumerator enumerator = ((IEnumerable)table).GetEnumerator();
      while (enumerator.MoveNext()) count++;

      Assert.Equal(1, count);
    }
  }
}
