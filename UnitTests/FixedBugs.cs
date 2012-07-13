using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Runtime.Promotion;
using Scripting.SSharp;
using Scripting.SSharp.Runtime.Operators;

namespace UnitTests
{
  /// <summary>
  /// Summary description for Bugs_June
  /// </summary>
  [TestClass]
  public class BugsJune
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

    [TestMethod]
    public void ExpandoFields()
    {
      RuntimeHost.AddType("Array", typeof(List<int>));

      object resultVal =
         Script.RunCode(
         @"
            return [                
                      storage -> new Array(),
                      storage1 -> new Array(),
                      storage2 -> new Array()
                    ];
          "
         );

      Expando data = (Expando)resultVal;

      Assert.IsInstanceOfType(data["storage"], typeof(List<int>));
      Assert.IsInstanceOfType(data["storage1"], typeof(List<int>));
      Assert.IsInstanceOfType(data["storage2"], typeof(List<int>));
    }
    
    [TestMethod]
    public void TestTypeExpr()
    {
      RuntimeHost.AddType("Int", typeof(int));
      ScriptContext context = new ScriptContext();

      object rez = Script.RunCode("a = 2; return a is Int;", context);
      Assert.IsTrue((bool)rez);
    }

    [TestMethod]
    public void TestTypeExprGenerics()
    {
      RuntimeHost.AddType("Int", typeof(int));
      ScriptContext context = new ScriptContext();

      object rez = Script.RunCode("a = new List<|Int|>(); return a is List<|Int|>;", context);
      Assert.IsTrue((bool)rez);
    }

    [TestMethod]
    public void TestTypeExprBug()
    {
      object rez = Script.RunCode("a = 2; return a is int;");
      Assert.IsTrue((bool)rez);
    }

    [TestMethod]
    public void MethodInvocationBug()
    {
      RuntimeHost.AddType("TestOverloading", typeof(TestOverloading));
      object rez = Script.RunCode("b = new TestOverloading(); return b.GetString(2);");
      Assert.AreEqual("Ok", rez);
    }

    [TestMethod]
    public void TestTypeExprGenerics1()
    {
      object rez = Script.RunCode("b = new System.Collections.Generic.Stack<|string|>();");
      Assert.IsInstanceOfType(rez, typeof(Stack<string>));
    }

    [TestMethod]
    public void InternalClassTest()
    {
      object rez = Script.RunCode("Class123.Class234.Value;");
      Assert.AreEqual(Class123.Class234.Value, rez);
    }
    
    [TestMethod]
    public void TypeConversionConflictsWithArithmeticOperations()
    {
      object rez = Script.RunCode("(1+2+3)-6;");
      Assert.AreEqual(0, rez);
    }

    [TestMethod]
    public void InterfaceLimitations()
    {
      object rez = Script.RunCode(@"
         a = new AT1();

         it = a.Method();

         a.SetMethod(it);
         a.Property = it;
         
         it.Value = 2;
         return it.Value;
      ");

      Assert.AreEqual(2, rez);
    }

    [TestMethod]
    public void EqualityOperator()
    {
      object rez = Script.RunCode(@"
          a = new Class1_1();
          if (a.Short == 1 && a.C == 'C')
            return 1;
          else 
            return 2;
      ");

      Assert.AreEqual(1, rez);
    }

    [TestMethod]
    public void EventsCrashed()
    {
      RuntimeHost.SetSettingItem("UnsubscribeAllEvents", false);
      RuntimeHost.SetSettingItem("ContextEnabledEvents", true);

      EventsCrashedTest rez = Script.RunCode(@"

        a = new EventsCrashedTest();
        a.Opened += myhandler;
  
        a.State = false;
        a.Open();

        function myhandler(sender,e){ sender.State = true; }

        return a;
      ") as EventsCrashedTest;

      Assert.IsTrue(rez.State);
    }

    [TestMethod]
    public void EqualityOperatorFullTest()
    {
      EqualsOperator eq = new EqualsOperator();

      Assert.IsTrue((bool)eq.Evaluate(Byte.Parse("1"), Byte.Parse("1")));
      Assert.IsTrue((bool)eq.Evaluate(Byte.Parse("1"), Int16.Parse("1")));
      Assert.IsTrue((bool)eq.Evaluate(Byte.Parse("1"), Int32.Parse("1")));
      Assert.IsTrue((bool)eq.Evaluate(Byte.Parse("1"), Int64.Parse("1")));
      Assert.IsTrue((bool)eq.Evaluate(Byte.Parse("1"), Double.Parse("1")));
      Assert.IsTrue((bool)eq.Evaluate(Byte.Parse("1"), Single.Parse("1")));

      Assert.IsTrue((bool)eq.Evaluate(Int16.Parse("1"), Byte.Parse("1"))); 
      Assert.IsTrue((bool)eq.Evaluate(Int16.Parse("1"), Int16.Parse("1")));
      Assert.IsTrue((bool)eq.Evaluate(Int16.Parse("1"), Int32.Parse("1")));
      Assert.IsTrue((bool)eq.Evaluate(Int16.Parse("1"), Int64.Parse("1")));
      Assert.IsTrue((bool)eq.Evaluate(Int16.Parse("1"), Double.Parse("1")));
      Assert.IsTrue((bool)eq.Evaluate(Int16.Parse("1"), Single.Parse("1")));

      Assert.IsTrue((bool)eq.Evaluate(Int32.Parse("1"), Byte.Parse("1")));
      Assert.IsTrue((bool)eq.Evaluate(Int32.Parse("1"), Int16.Parse("1")));
      Assert.IsTrue((bool)eq.Evaluate(Int32.Parse("1"), Int32.Parse("1")));
      Assert.IsTrue((bool)eq.Evaluate(Int32.Parse("1"), Int64.Parse("1")));
      Assert.IsTrue((bool)eq.Evaluate(Int32.Parse("1"), Double.Parse("1")));
      Assert.IsTrue((bool)eq.Evaluate(Int32.Parse("1"), Single.Parse("1")));

      Assert.IsTrue((bool)eq.Evaluate(Int64.Parse("1"), Byte.Parse("1")));
      Assert.IsTrue((bool)eq.Evaluate(Int64.Parse("1"), Int16.Parse("1")));
      Assert.IsTrue((bool)eq.Evaluate(Int64.Parse("1"), Int32.Parse("1")));
      Assert.IsTrue((bool)eq.Evaluate(Int64.Parse("1"), Int64.Parse("1")));
      Assert.IsTrue((bool)eq.Evaluate(Int64.Parse("1"), Double.Parse("1")));
      Assert.IsTrue((bool)eq.Evaluate(Int64.Parse("1"), Single.Parse("1")));

      Assert.IsTrue((bool)eq.Evaluate(Double.Parse("1"), Byte.Parse("1")));
      Assert.IsTrue((bool)eq.Evaluate(Double.Parse("1"), Int16.Parse("1")));
      Assert.IsTrue((bool)eq.Evaluate(Double.Parse("1"), Int32.Parse("1")));
      Assert.IsTrue((bool)eq.Evaluate(Double.Parse("1"), Int64.Parse("1")));
      Assert.IsTrue((bool)eq.Evaluate(Double.Parse("1"), Double.Parse("1")));
      Assert.IsTrue((bool)eq.Evaluate(Double.Parse("1"), Single.Parse("1")));

      Assert.IsTrue((bool)eq.Evaluate(Single.Parse("1"), Byte.Parse("1")));
      Assert.IsTrue((bool)eq.Evaluate(Single.Parse("1"), Int16.Parse("1")));
      Assert.IsTrue((bool)eq.Evaluate(Single.Parse("1"), Int32.Parse("1")));
      Assert.IsTrue((bool)eq.Evaluate(Single.Parse("1"), Int64.Parse("1")));
      Assert.IsTrue((bool)eq.Evaluate(Single.Parse("1"), Double.Parse("1")));
      Assert.IsTrue((bool)eq.Evaluate(Single.Parse("1"), Single.Parse("1")));

      Assert.IsTrue((bool)eq.Evaluate("C", 'C'));

    }

    [TestMethod]
    public void OperatorOverloaderTest()
    {
      object rez = Script.RunCode(@"
          a = new OperatorOverloader(3);
          b = new OperatorOverloader(4);
          c = 3.2;

          return a+b;
      ");

      Assert.AreEqual((double)7.0, ((OperatorOverloader)rez).Value);
      
      rez = Script.RunCode(@"
          a = new OperatorOverloader(3);
          b = new OperatorOverloader(4);
          c = 3.2;

          return a+c;
      ");

      Assert.AreEqual((double)6.2, rez);
      
      rez = Script.RunCode(@"
          a = new OperatorOverloader(3);
          b = new OperatorOverloader(4);
          c = 3.2;

          return c+b;
      ");

      Assert.AreEqual((double)7.2, rez);

    }

    [TestMethod]
    public void OperatorOverloaderTest_Full()
    {
      object rez = Script.RunCode(@"
          a = new OperatorOverloader(3);
          b = new OperatorOverloader(4);
          c = 3.2;

          a+b;
          a+c;
          c+a;
          a+b+c;
          a+c+b;
          b+c+a;

          a-b;
          a-c;
          c-a;
          a-b-c;
          a-c-b;
          b-c-a;

          a*b;
          a*c;
          c*a;
          a*b*c;
          a*c*b;
          b*c*a;

          a/b;
          a/c;
          c/a;
          a/b/c;
          a/c/b;
          b/c/a;

          a%b;
          a%c;
          c%a;
          a%b%c;
          a%c%b;
          b%c%a;

          a+b*c;
          a+c/b;
      ");
    }

    [TestMethod]
    public void OperatorOverloader_List()
    {
      object rez = Script.RunCode(@"
          a = [new OperatorOverloader(3),
               new OperatorOverloader(4),
               new OperatorOverloader(5),
               1.0];
          s = 0;
          
          for(i = 0; i < a.Length; i++)
          {
            s = s + a[i]*a[i];
          }

          return s;
      ");

      Assert.AreEqual((double)(3*3+4*4+5*5+1*1), rez);
    }

  }

  #region Interfaces
  public interface IT1
  {
    int Value { get; set; }
  }

  public class AT1 : IT1
  {
    #region IT1 Members

    public int Value
    {
      get;
      set;
    }

    #endregion

    public IT1 Method()
    {
      return this;
    }

    public void SetMethod(IT1 v)
    {
    }

    public IT1 Property
    {
      get;
      set;
    }
  }

  #endregion

  #region Test Class
  public class Class1_1
  {
    public short Short { get { return 1; } }

    public char C { get { return 'C'; } }
  }
  
  public class Class123
  {
    public class Class234
    {
      public static string Value = "Test";

      public string Instance = "Test1";
    }
  }
  #endregion

  #region Helpers
  public class OperatorOverloader
  {
    public double Value { get; set; }

    public OperatorOverloader(double value)
    {
      Value = value;
    }

    public static double operator +(OperatorOverloader oper, double k)
    {
      return k + oper.Value;
    }
    public static double operator +(double k, OperatorOverloader oper)
    {
      return k + oper.Value;
    }
    public static OperatorOverloader operator +(OperatorOverloader a, OperatorOverloader b)
    {
      return new OperatorOverloader( a.Value + b.Value );
    }


    public static double operator -(OperatorOverloader oper, double k)
    {
      return k - oper.Value;
    }
    public static double operator -(double k, OperatorOverloader oper)
    {
      return k - oper.Value;
    }
    public static OperatorOverloader operator -(OperatorOverloader a, OperatorOverloader b)
    {
      return new OperatorOverloader(a.Value - b.Value);
    }

    public static double operator *(OperatorOverloader oper, double k)
    {
      return k * oper.Value;
    }
    public static double operator *(double k, OperatorOverloader oper)
    {
      return k * oper.Value;
    }
    public static OperatorOverloader operator *(OperatorOverloader a, OperatorOverloader b)
    {
      return new OperatorOverloader(a.Value * b.Value);
    }

    public static double operator /(OperatorOverloader oper, double k)
    {
      return k / oper.Value;
    }
    public static double operator /(double k, OperatorOverloader oper)
    {
      return k / oper.Value;
    }
    public static OperatorOverloader operator /(OperatorOverloader a, OperatorOverloader b)
    {
      return new OperatorOverloader(a.Value / b.Value);
    }

    public static double operator %(OperatorOverloader oper, double k)
    {
      return k % oper.Value;
    }
    public static double operator %(double k, OperatorOverloader oper)
    {
      return k % oper.Value;
    }
    public static OperatorOverloader operator %(OperatorOverloader a, OperatorOverloader b)
    {
      return new OperatorOverloader(a.Value + b.Value);
    }
  }
  
  class TestOverloading
  {
    public string GetString(bool value)
    {
      return "Error";
    }

    public string GetString(int value)
    {
      return "Ok";
    }
  }

  public class EventsCrashedTest
  {
    public event EventHandler<EventArgs> Opened;

    protected virtual void OnOpened()
    {
      if (Opened != null)
        Opened.Invoke(this, EventArgs.Empty);
    }

    public void Open()
    {
      OnOpened();
    }

    public bool State { get; set; }
  }
  #endregion
}
