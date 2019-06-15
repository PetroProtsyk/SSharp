using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Runtime.Promotion;
using Scripting.SSharp;
using System.Collections;
using Scripting.SSharp.Parser;

namespace UnitTests
{
  public class AcceptanceTests_FirstPart : IDisposable
  {
    public AcceptanceTests_FirstPart()
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
    public void AccessingEnum()
    {
      object resultVal =
         Script.RunCode(
         @"
            return A_Enum.Two;
          "
         );

      Assert.Equal(A_Enum.Two, resultVal);
    }

    [Fact]
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

      Assert.Equal(result, resultVal);
    }

    [Fact]
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

      Assert.Equal(N + 1, resultVal.Length);
    }

    [Fact]
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

      Assert.Equal(N + 1, resultVal.Fields.Count());
    }

    [Fact]
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

      Assert.Equal(4, resultVal);
    }

    [Fact]
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

      Assert.Equal(4, resultVal);
    }

    //NOTE: ref, out
    [Fact]
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

      Assert.Equal(11L, resultVal);

      resultVal =
         Script.RunCode(
         @"
            t = new TestModif();
            y = null; z = 3;
            t.TestOut(5, out y, ref z);

            return y;
          "
         );

      Assert.IsType<TestModif>(resultVal);
    }

    [Fact]
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

      Assert.Equal(4, resultVal);
    }

    [Fact]
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

      Assert.Equal(1, resultVal);
    }

    [Fact]
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

      Assert.Equal(1, resultVal);
    }
    
    [Fact]
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

      Assert.Equal(0, resultVal);
    }

    [Fact]
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

      Assert.Equal(0, resultVal);
    }

    [Fact]
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
      
      Assert.True((bool)resultVal);
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

    [Fact]
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

      Assert.Equal(2, resultVal);

      resultVal =
           Script.RunCode(
           @"
           x = 0;
           
           function f() { x = 3; g(); return x; }
           function g() global(x) { x = 2;}

           return f();
          ");

      Assert.Equal(2, resultVal);

      resultVal =
     Script.RunCode(
     @"
           x = 0;
           
           function f() { x = 3; g(); return x; }
           function g() global(x) { x = 2;}

           return x;
          ");

      Assert.Equal(0, resultVal);
    }

    [Fact]
    public void ScopeTraversalExample()
    {
      Script s = Script.Compile(
       @"
           x = 0;           
           function g() { x = 1; return gScope.GetItem('x',false); }

           return g();
          ");      
      s.Context.SetItem("gScope", s.Context.Scope);


      Assert.Equal(0, s.Execute());
    }

    [Fact]
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
      
      Assert.Equal("aaabbbccc", context.GetItem("a", true));
      Assert.Equal("'aaa", context.GetItem("b", true));
      Assert.Equal("cc\\aaa", context.GetItem("c", true));
      Assert.Equal("hello world \\\\", context.GetItem("d", true));
    }

    [Fact]
    public void StringParsingError()
    {
      Assert.Throws<ScriptSyntaxErrorException>(() =>
       Script.Compile(
       @"
          a = 'aaabbbccc;
          b = '\'aaa';
          c = 'cc\\aaa';

          d = @'hello world \\';
        "));
    }

    [Fact]
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

      Assert.Equal(45, context.GetItem("sum", true));
      Assert.Equal(RuntimeHost.NoVariable, context.GetItem("x", false));
    }

    [Fact]
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

      Assert.Equal(2, context.GetItem("a", true));
      Assert.Equal(4, context.GetItem("b", true));
    }

    [Fact]
    public void VarCreatesNullVariable()
    {
      IScriptContext context = new ScriptContext();

      object resultVal =
       Script.RunCode(
       @"
           var sum;
          ", context
       );

      Assert.Null(context.GetItem("sum", true));

      resultVal =
         Script.RunCode(
         @"
                 var sum = 2;
                ", context
         );

      Assert.Equal(2, context.GetItem("sum", true));
    }

    [Fact]
    public void NotEqualsOperator()
    {
        object resultVal =
         Script.RunCode(
         @"
            return ((byte)0) != ((int)0);
        ");

        Assert.False((bool)resultVal);
    }

    [Fact]
    public void CreatingGlobalVariableDifferentCases()
    {
      IScriptContext context = new ScriptContext();

      object resultVal =
       Script.RunCode(
       @"
           a = 2;
          ", context
       );

      Assert.Equal(2, context.GetItem("a", true));

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

      Assert.Equal(4, context.GetItem("a", true));

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

      Assert.Equal(RuntimeHost.NoVariable, context.GetItem("a", false));

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

      Assert.Equal(4, context.GetItem("b", false));
    }

    [Fact]
    public void DuplicateEvents() {
        Script s = Script.Compile(
           @"
            invoked_count = 0;
 
            function handler(s,e) global(invoked_count) {
             invoked_count++;
            }

            function handler2(s,e) global(invoked_count) {
             invoked_count+=2;
            }

            ce = new ContextEvent();

            ce.NameChanged += handler;
            ce.NameChanged += handler;
            ce.NameChanged += handler2;

            ce.Name = 'TestName';
            ce.Name = 'TestName0';

            // Remove events in different order
            ce.NameChanged -= handler2;
            ce.NameChanged -= handler;
            ce.NameChanged -= handler;

            // Change again
            ce.Name = 'TestName1';

            "
           );

        s.Execute();
        Assert.Equal(8, (int)s.Context.GetItem("invoked_count", false));
        Assert.True(((ContextEvent)s.Context.GetItem("ce", false)).IsNullEvent);
    }

    [Fact]
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
      Assert.False((bool)s.Context.GetItem("invoked", false));

      resultVal.Name = "TestName";

      Assert.True((bool)s.Context.GetItem("invoked", false));
    }

    [Fact]
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

      Assert.Equal(1, c.GetItem("invoked", false));

      s.Context = c1;
      resultVal.Name = "TestName2";

      Assert.Equal(11, c1.GetItem("invoked", false));

      s.Dispose();
      //TODO: Event Broker should be refactored
      try
      {
        resultVal.Name = "TestName 4";
      }
      catch (ScriptEventException e)
      {
        Assert.Equal(Strings.ContextNotFoundExceptionMessage, e.Message);
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

    public bool IsNullEvent {
        get {
            return NameChanged == null;
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
