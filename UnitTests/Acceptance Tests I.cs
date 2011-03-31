using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Runtime.Promotion;
using Scripting.SSharp;
using System.Collections;
using Scripting.SSharp.Parser;
using Scripting.SSharp.Debug;

namespace UnitTests
{
  [TestClass]
  public class AcceptanceTests_FirstPart
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
    public void AccessingEnum()
    {
      object resultVal =
         Script.RunCode(
         @"
            return A_Enum.Two;
          "
         );

      Assert.AreEqual(A_Enum.Two, resultVal);
    }

    [TestMethod]
    public void GetEnumeratorTest()
    {
      string result = "";
      foreach (string s in new A_TestList())
      {
        result += s;
      }

      object resultVal =
         Script.RunCode(
         @"
           result = '';
           foreach (s in new A_TestList())
           {
            result += s;
           }
          "
         );

      Assert.AreEqual(result, resultVal);
    }

    [TestMethod]
    public void CreatesArrayWith1000Objects()
    {
      int N = 1000;
      StringBuilder codeBuilder = new StringBuilder();
      codeBuilder.Append('[');
      codeBuilder.Append('1');
      for (int i = 0; i < N; i++) 
      {
         codeBuilder.Append(',');
         codeBuilder.Append(i);
      }
      codeBuilder.Append("];");

      object[] resultVal = (object[]) Script.RunCode( codeBuilder.ToString() );

      Assert.AreEqual(N + 1, resultVal.Length);
    }

    [TestMethod]
    public void CreatesExpandoWith2000Fields()
    {
      int N = 2000;
      StringBuilder codeBuilder = new StringBuilder();
      codeBuilder.Append("[b->'Hello'");
      for (int i = 0; i < N; i++)
      {
        codeBuilder.AppendFormat(", a{0}->{1}", i, i);
      }
      codeBuilder.Append("];");

      Expando resultVal = (Expando)Script.RunCode(codeBuilder.ToString());

      Assert.AreEqual(N + 1, resultVal.Fields.Count());
    }

    [TestMethod]
    public void GlobalKeywordModifier()
    {
      object resultVal =
         Script.RunCode(
         @"
            a = 2;

            function GlobalAcess(){
              global:a = a + 2;
            }
            GlobalAcess();

            return a;
          "
         );

      Assert.AreEqual(4, resultVal);
    }

    [TestMethod]
    public void GlobalKeywordModifierWithLocalId()
    {
      object resultVal =
         Script.RunCode(
         @"
            a = 2;

            function GlobalAcess(){
              a = 3;
              global:a = global:a + 2;
            }
            GlobalAcess();

            return a;
          "
         );

      Assert.AreEqual(4, resultVal);
    }

    //NOTE: ref, out
    [TestMethod]
    public void ParameterModifiers()
    {
      object resultVal =
         Script.RunCode(
         @"
            t = new TestModif();
            y = 2; z = 3;
            t.Test(5, out y, ref z);

            return y+z;
          "
         );

      Assert.AreEqual(11L, resultVal);

      resultVal =
         Script.RunCode(
         @"
            t = new TestModif();
            y = null; z = 3;
            t.TestOut(5, out y, ref z);

            return y;
          "
         );

      Assert.IsInstanceOfType(resultVal, typeof(TestModif));
    }

    [TestMethod]
    public void FunctionLocalNameClashes()
    {
      object resultVal =
       Script.RunCode(
       @"
            function f1(a){
              return a;
            }

            a = [1,2,3];
            f1(a);          

            return f1(4);
          "
       );

      Assert.AreEqual(4, resultVal);
    }

    [TestMethod]
    public void ScopingExample()
    {
      object resultVal =
       Script.RunCode(
       @"
           x = 0;
           
           function f() { return x; }
           function g() { x = 1; return f(); }

           return g();
          ");

      Assert.AreEqual(1, resultVal);
    }

    [TestMethod]
    public void ScopingExample_GlobalRefersToParentScope()
    {
      object resultVal =
       Script.RunCode(
       @"
           x = 0;
           
           function f() global(x) { return x; }
           function g() { x = 1; return f(); }

           return g();
          ");

      Assert.AreEqual(1, resultVal);
    }
    
    [TestMethod]
    public void ScopingExample_GlobalRefersToParentScope1()
    {
      object resultVal =
       Script.RunCode(
       @"
           x = 0;
           
           function f() global(x) { return x; }
           function g() { x = 1; return global:x; }

           return g();
          ");

      Assert.AreEqual(0, resultVal);
    }

    [TestMethod]
    public void ScopingExample_LocalFunctionVariable()
    {
      object resultVal =
       Script.RunCode(
       @"
           x = 0;
           
           function g() { x = 1;}

           g();
           return x; 
          ");

      Assert.AreEqual(0, resultVal);
    }

    [TestMethod]
    public void ScopingExample_TestingVariable()
    {
      IScriptContext context = new ScriptContext();
      context.SetFunction<DefinedFunction>("Defined");

      object resultVal =
       Script.RunCode(
       @"
           x = 0;
           
           function g() { Defined('x'); }
           
           return g();
          ", context);
      
      Assert.IsTrue((bool)resultVal);
    }

    private class DefinedFunction : IInvokable
    {
      #region IInvokable Members

      public bool CanInvoke()
      {
        return true;
      }

      public object Invoke(IScriptContext context, object[] args)
      {
        string id = (string)args[0];
        IScriptScope scope = context.Scope;
        while (scope != null)
        {
          if (scope.HasVariable(id)) return true;
          scope = scope.Parent;
        }
        return false;
      }

      #endregion
    }

    [TestMethod]
    public void ScopingExample_ModifyingGlobalVariable()
    {
      object resultVal =
       Script.RunCode(
       @"
           x = 0;
           
           function g() global(x) { x = 2;}

           g();
           return x; 
          ");

      Assert.AreEqual(2, resultVal);

      resultVal =
           Script.RunCode(
           @"
           x = 0;
           
           function f() { x = 3; g(); return x; }
           function g() global(x) { x = 2;}

           return f();
          ");

      Assert.AreEqual(2, resultVal);

      resultVal =
     Script.RunCode(
     @"
           x = 0;
           
           function f() { x = 3; g(); return x; }
           function g() global(x) { x = 2;}

           return x;
          ");

      Assert.AreEqual(0, resultVal);
    }

    [TestMethod]
    public void ScopeTraversalExample()
    {
      Script s = Script.Compile(
       @"
           x = 0;           
           function g() { x = 1; return gScope.GetItem('x',false); }

           return g();
          ");      
      s.Context.SetItem("gScope", s.Context.Scope);


      Assert.AreEqual(0, s.Execute());
    }

    [TestMethod]
    public void StringParsing()
    {
      IScriptContext context = new ScriptContext();

      Script s = Script.Compile(
       @"
          a = 'aaabbbccc';
          b = '\'aaa';
          c = 'cc\\aaa';

          d = @'hello world \\';
        ");
      s.Context = context;
      s.Execute();
      
      Assert.AreEqual("aaabbbccc", context.GetItem("a", true));
      Assert.AreEqual("'aaa", context.GetItem("b", true));
      Assert.AreEqual("cc\\aaa", context.GetItem("c", true));
      Assert.AreEqual("hello world \\\\", context.GetItem("d", true));
    }
    
    [TestMethod]
    public void DebugEvent() {
        try {
            int break_points = 0;

            DebugManager.SetDefaultDebugger(false);
            DebugManager.BreakPoint += (s, e) => {
                break_points++;
            };

            using (Script s = Script.CompileForDebug(@"
                s = (decimal)19.2;

                1+1; 2+2; b=true; 
                for (i=1; i<3; i++) { 
                   if (i==2)
                    s = 'hello debugger!';
                  else
                    b=false; 
                }
                a=1;
                while (a<4)
                {
                    s=['a','b','c'];
                    foreach(c in s)
                        e=(a+c);
                    a++;
                }

                ")) {
                s.Execute();

                Assert.AreEqual(37, break_points);
            }
        }
        finally {
            DebugManager.BreakPoint = null;
        }
    }

    [TestMethod]
    [ExpectedException(typeof(ScriptSyntaxErrorException))]
    public void StringParsingError()
    {
      IScriptContext context = new ScriptContext();

      Script s = Script.Compile(
       @"
          a = 'aaabbbccc;
          b = '\'aaa';
          c = 'cc\\aaa';

          d = @'hello world \\';
        ");
      s.Context = context;
      s.Execute();

      Assert.AreEqual("aaabbbccc", context.GetItem("a", true));
      Assert.AreEqual("'aaa", context.GetItem("b", true));
      Assert.AreEqual("cc\\aaa", context.GetItem("c", true));
      Assert.AreEqual("hello world \\\\", context.GetItem("d", true));
    }

    [TestMethod]
    public void VarLocalScope()
    {
      IScriptContext context = new ScriptContext();

      object resultVal =
       Script.RunCode(
       @"
           var sum = 0;

           for (var x=0; x<10; x++){ 
             var temp = x;            
             sum += x;
           }

           return sum;
          ", context
       );

      Assert.AreEqual(45, context.GetItem("sum", true));
      Assert.AreEqual(RuntimeHost.NoVariable, context.GetItem("x", false));
    }

    [TestMethod]
    public void VarLocalNestedScopes()
    {
      IScriptContext context = new ScriptContext();

      object resultVal =
       Script.RunCode(
       @"
         var b;

         {
           a = 2;
           b = 4;

           {
              var b = 3;

              {
                var a;
                a = 2;
              }

           }
         }

          ", context
       );

      Assert.AreEqual(2, context.GetItem("a", true));
      Assert.AreEqual(4, context.GetItem("b", true));
    }

    [TestMethod]
    public void VarCreatesNullVariable()
    {
      IScriptContext context = new ScriptContext();

      object resultVal =
       Script.RunCode(
       @"
           var sum;
          ", context
       );

      Assert.IsNull(context.GetItem("sum", true));

      resultVal =
         Script.RunCode(
         @"
                 var sum = 2;
                ", context
         );

      Assert.AreEqual(2, context.GetItem("sum", true));
    }

    [TestMethod]
    public void CreatingGlobalVariableDifferentCases()
    {
      IScriptContext context = new ScriptContext();

      object resultVal =
       Script.RunCode(
       @"
           a = 2;
          ", context
       );

      Assert.AreEqual(2, context.GetItem("a", true));

      context = new ScriptContext();
      resultVal =
         Script.RunCode(
         @"           
           //Global scope

           { //Local scope 1
             
             {//Local scope 2
             
               //Create global variable from local scope
               a = 4;

             }
           }
           // a = 4 will be evaluated in global scope
           return a;
                ", context
         );

      Assert.AreEqual(4, context.GetItem("a", true));

      context = new ScriptContext();
      resultVal =
         Script.RunCode(
         @"           
           //Global scope

           { //Local scope 1
             var a; //Create empty variable in local scope 1

             {//Local scope 2
             
               //This will set variable to top-most scope which contains 
               //definition for variable, which is Local scope 1
               a = 4;
             }
           }

           //Global scope still empty
                ", context
         );

      Assert.AreEqual(RuntimeHost.NoVariable, context.GetItem("a", false));

      context = new ScriptContext();
      resultVal =
         Script.RunCode(
         @"           
           //Global scope

           { //Local scope 1
             var a; //Create empty variable in local scope 1

             {//Local scope 2

               //This will set variable to top-most scope which contains 
               //definition for variable, which is Local scope 1              
               a = 5;
                
               { //Local scope 3
                 
                 var a; //Create empty variable in local scope 3
             
                 //This will set variable to top-most scope which contains 
                 //definition for variable, which is Local scope 1
                 global:a = 4;
                 
                 a = 3; // Set local variable
               }
             }
             
             //Create variable in global scope
             b = a;
           }

           //b = 4;
           return b;
                ", context
         );

      Assert.AreEqual(4, context.GetItem("b", false));
    }

    [TestMethod]
    public void ContextDependantEvents()
    {      
      Script s = Script.Compile(
         @"
            invoked = false;
 
            function handler(s,e) global(invoked) {
             invoked = true;
            }

            ce = new ContextEvent();
            ce.NameChanged += handler;

            return ce;
          "
         );


      ContextEvent resultVal =(ContextEvent)s.Execute();
      Assert.IsFalse((bool)s.Context.GetItem("invoked", false));

      resultVal.Name = "TestName";

      Assert.IsTrue((bool)s.Context.GetItem("invoked", false));
    }

    [TestMethod]
    public void ContextDependantEventsAndContextSwitching()
    {
      ScriptContext c = new ScriptContext();
      c.SetItem("invoked", 0);

      ScriptContext c1 = new ScriptContext();
      c1.SetItem("invoked", 10);


      Script s = Script.Compile(
         @"
            function handler(s,e) global(invoked) {
             invoked++;
            }

            ce = new ContextEvent();
            ce.NameChanged += handler;

            return ce;
          "
         );
      s.Context = c;

      ContextEvent resultVal = (ContextEvent)s.Execute();
      resultVal.Name = "TestName";

      Assert.AreEqual(1, c.GetItem("invoked", false));

      s.Context = c1;
      resultVal.Name = "TestName2";

      Assert.AreEqual(11, c1.GetItem("invoked", false));

      s.Dispose();
      //TODO: Event Broker should be refactored
      try
      {
        resultVal.Name = "TestName 4";
      }
      catch (ScriptEventException e)
      {
        Assert.AreEqual(Strings.ContextNotFoundExceptionMessage, e.Message);
      }
    }
}

  public class ContextEvent
  {
    string name;

    public string Name
    {
      get
      {
        return name;
      }
      set
      {
        name = value;
        if (NameChanged != null)
          NameChanged.Invoke(this, EventArgs.Empty);
      }
    }

    public event EventHandler<EventArgs> NameChanged;
  }

  public class TestModif
  {
    public void Test(int x, out int y, ref long z)
    {
      y = x;
      z = x + 1;
    }

    public void TestOut(int x, out TestModif y, ref long z)
    {
      y = this;
      z = x + 1;
    }
  }

  public class A_TestList
  {
    public IEnumerator GetEnumerator()
    {
      return (new object[] { "Hello", " ", "World" }).GetEnumerator();
    }
  }

  public enum A_Enum
  {
    One,
    Two,
    Three
  }
}
