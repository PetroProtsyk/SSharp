using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Runtime.Promotion;
using Scripting.SSharp;
using Scripting.SSharp.Runtime.Operators;
using System.Threading.Tasks;

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
      EventBroker.ClearAllSubscriptions();
    }

    [TestCleanup]
    public void TearDown()
    {
      RuntimeHost.CleanUp();
      EventBroker.ClearAllSubscriptions();
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

      Assert.IsTrue((bool)eq.Evaluate(null, null));

      //Assert.IsTrue((bool)eq.Evaluate("C", 'C'));

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

    [TestMethod]
    public void Function_OverloadsWithConflictingTypes()
    {
      RuntimeHost.AddType("TestTypeConversion", typeof(TestTypeConversion));

      object rez = Script.RunCode(@"
         x=true; y =1; return TestTypeConversion.WriteLine('Value of x {0} and y {1}',x,y);
      ");

      Assert.AreEqual("Value of x True and y 1", rez);
    }

    [TestMethod]
    public void ContextSwitching()
    {
      Script s = Script.Compile("return A;");

      ScriptContext sc = new ScriptContext();
      sc.SetItem("A", 1);
      s.Context = sc;      

      Assert.AreEqual(1, s.Execute());

      sc = new ScriptContext();
      sc.SetItem("A", 10);
      s.Context = sc;

      Assert.AreEqual(10, s.Execute());
    }

    [TestMethod]
    public void MultiDimArrays() {
        object rez = Script.RunCode(@"
           a = new int[3,3];
           c = 1;
           for (i=0; i<3; i++)
            for (j=0; j<3; j++){
             a[i,j] = c; c++;
            }

           s = '';
           for (i=0; i<3; i++)
            for (j=0; j<3; j++)
              s += a[i,j];

           return s;
         ");
        Assert.AreEqual("123456789",rez);
    }

    [TestMethod]
    public void ProblemWithGlobalScope()
    {
        Script s = Script.Compile(@"
            abc = '123';
            function OnGetCustomersCompleted(s, e)
            {
                s.v = abc;
            }

            s = new CustomerFacade();
            s.GetCustomersCompleted += OnGetCustomersCompleted;
            s.GetCustomersAsync();
            
            return s;
         ");

        CustomerFacade rez = (CustomerFacade)s.Execute();

        while (rez.busy) System.Threading.Thread.Sleep(100);

        Assert.AreEqual("123", rez.v);
    }

    [TestMethod]
    public void ProblemWithGlobalScopeWithAsyncEvents()
    {
        CustomerFacade rez = (CustomerFacade)Script.RunCode(@"
            abc = '123';
            function OnGetCustomersCompleted(s, e)
            {
                s.v = abc;
            }

            s = new CustomerFacade();
            s.GetCustomersCompleted += OnGetCustomersCompleted;
            s.GetCustomersAsync();
            
            return s;
         ");

        //Now scope is clear, while event is still subscribed and will be invoked
        while (rez.busy) System.Threading.Thread.Sleep(100);
        Assert.IsTrue(rez.v.Contains("given handler is not associated with any context"));
    }
    
    [TestMethod]
    public void ContextReusageProblem()
    {
        var context = new ScriptContext();

        string code1 = "'test';";
        string code2 = "3*5; return 1+1;";

        var result1 = Script.RunCode(code1, context, true);

        var script = Script.Compile(code2);
        script.Context = context;

        script.Execute();

        Assert.AreEqual(2, context.Result);
    }

    [TestMethod]
    public void ContextReusageProblemMetaOperator()
    {
      string code1 = "a=<! return 1+2; !>; a(); 3*5; return 1+1;";
      var s = Script.Compile(code1);
      var c = s.Code(s.Ast);

      Assert.AreEqual(2, s.Execute());
    }

    [TestMethod]
    public void ImplicitConversionFailure()
    {
      string code1 = "a=(Decimal)19.2; s=Decimal.Parse(a); b=(double)a;";
      var s = Script.Compile(code1);
      var c = s.Execute();
      
      Assert.AreEqual(19.2d, c);

      var s1 = Script.Compile("a=(Decimal)19.2; s=a+2; b=a*a; d=2.3*(a+1);");
      var c1 = s1.Execute();

      Assert.AreEqual(new Decimal(19.2d), s1.Context.GetItem("a",true));
      Assert.AreEqual(new Decimal(19.2d)+2, s1.Context.GetItem("s", true));
      Assert.AreEqual(new Decimal(19.2d) * new Decimal(19.2d), s1.Context.GetItem("b", true));
      Assert.AreEqual(2.3*(double)((new Decimal(19.2d))+1), s1.Context.GetItem("d", true));

    }

    [TestMethod]
    public void ExpressionTypeConversion_Bug() {
        string code1 = "a='19'; b='3'; s=(Decimal)a+(Decimal)b;";
        var s = Script.Compile(code1);
        var c = s.Execute();
        string tt = s.SyntaxTree;
        Assert.IsInstanceOfType(c, typeof(decimal));
        Assert.AreEqual(new Decimal(22), c);
    }

    [TestMethod]
    public void NullValueBinding() {
        var s = Script.Compile(@"shipName = null;
                    if( shipName == null ) shipName = '';
        ");
        var c = s.Execute();
        Assert.AreEqual("", c);
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
  public class CustomerFacade {
      
      public event EventHandler<EventArgs> GetCustomersCompleted;

      public string v { get; set; }

      public bool busy { get; set; }

      public void GetCustomersAsync()
      {
          busy = true;
          
          Task t = Task.Factory.StartNew(
              () => {
                  System.Threading.Thread.Sleep(500);

                  try {
                      if (GetCustomersCompleted != null) {
                          GetCustomersCompleted.Invoke(this, EventArgs.Empty);
                      }
                  }
                  catch(Exception e)
                  {
                      v = e.ToString();
                  }
                  finally {
                      busy = false;
                  }
              });
      }
  }

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

  class TestTypeConversion
  {
    public static string WriteLine(char[] format, object arg1, object arg2)
    {
      throw new NotSupportedException();
    }

    public static string WriteLine(char[] format, int arg1, int arg2)
    {
      throw new NotSupportedException();
    }

    public static string WriteLine(string format, params object[] args)
    {
      return string.Format(format, args);
    }

    public static string WriteLine(string format,object arg1, object arg2)
    {
      return string.Format(format, arg1, arg2);
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
