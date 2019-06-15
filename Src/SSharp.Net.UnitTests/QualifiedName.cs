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
  /// Summary description for QualifiedName
  /// </summary>
  public class QualifiedName : IDisposable
  {
    public QualifiedName()
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
    public void GenericTypes()
    {
      RuntimeHost.AddType("Generic", typeof(Generic<>));
      ScriptContext context = new ScriptContext();
      Script script = Script.Compile(@"
          return Generic<|int|>.TypeName;
      ");

      object result = script.Ast.Execute(context);

      Assert.Equal(Generic<int>.TypeName, result);
    }

    [Fact]
    public void BaseAssignment()
    {
      RuntimeHost.AddType("TestT", typeof(TestT));
      ScriptContext context = new ScriptContext();

      TestT rez = (TestT)Script.RunCode(@"
         a = new TestT();
         a.value = 'test';
         a.intVal = 20;

         return a;
      ", context);
      
      Assert.Equal(20, rez.intVal);
      Assert.Equal("test", rez.value);
    }

    [Fact]
    public void AssignmentToEvaluatedObject()
    {
      ScriptContext context = new ScriptContext();
      //context.AddType("TestT", typeof(TestT));

      TestT rez = (TestT)Script.RunCode(@"
         a = new TestT();
         a.GetThis().value = 'test';
         a.GetThis().intVal = 20;

         return a;
      ", context);

      Assert.Equal(20, rez.intVal);
      Assert.Equal("test", rez.value);
    }

    [Fact]
    public void AssignmentToArrayObject()
    {
      TestT obj = new TestT();
      List<int> list = new List<int>();
      list.AddRange(new int[]{1,2,3,4,5,6,7,8,9});
      obj.testObject = list;

      ScriptContext context = new ScriptContext();
      //context.AddType("TestT", typeof(TestT));
      context.SetItem("a", obj);

      TestT rez = (TestT)Script.RunCode(@"
         a.testObject[2] = 15;
         a.GetThis().testObject[3] = 16;
         return a;
      ", context);

      Assert.Equal(15, list[2]);
      Assert.Equal(16, list[3]);
    }

    [Fact]
    public void AssignmentToArrayObjectReturnedByFunctionCall()
    {
      TestT obj = new TestT();
      List<int> list = new List<int>();
      list.AddRange(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      obj.testObject = list;

      ScriptContext context = new ScriptContext();
      //context.AddType("TestT", typeof(TestT));
      context.SetItem("a", obj);

      TestT rez = (TestT)Script.RunCode(@"
         a.GetTestObject()[3] = 16;
         return a;
      ", context);

      Assert.Equal(16, list[3]);
    }

    [Fact]
    public void LongTypeExpression()
    {     
      object rez = Script.RunCode(@"
         a = System.Data.Common.CatalogLocation.End;
         return a;
      ");

      Assert.Equal(System.Data.Common.CatalogLocation.End, rez);
    }

    [Fact]
    public void UsingObjectThroughGenericInterface()
    {
      object rez = Script.RunCode(@"
          list = new List<|int|>();
     
          a = (ICollection<|int|>)list;
          a.Add(3);
          return a[0];
      ");

      Assert.Equal(3, rez);
    }
  }

  public class TestT
  {
    public string value;

    public int intVal
    {
      get;
      set;
    }

    public object testObject;

    public object GetTestObject()
    {
      return testObject;
    }

    public TestT GetThis()
    {
      return this;
    }
  }

  public class Generic<T>
  {
    public Generic()
    {
    }

    public static string TypeName = typeof(T).Name;
  }
}
