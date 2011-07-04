using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Runtime.Promotion;
using Scripting.SSharp;
using System.Data;

namespace UnitTests
{
  /// <summary>
  /// Summary description for ToDo
  /// </summary>
  [TestClass]
  public class AdvancedTests
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
    public void WorkingWithDataSets()
    {
      DataSet data = new DataSet();

      DataTable table1 = new DataTable("table1");
      table1.Columns.Add("id", typeof(int));
      table1.Columns.Add("Name", typeof(string));
      data.Tables.Add(table1);

      data.Tables["table1"].Rows.Add(1, "Petro");
      data.Tables["table1"].Rows.Add(2, "Denis");
      
      ScriptContext context = new ScriptContext();
      context.SetItem("data", data);

      object result = Script.RunCode(@"
            return data.Tables['table1'].Rows[0][1];
           ", context);

      Assert.AreEqual(data.Tables["table1"].Rows[0][1], result);

      result = Script.RunCode(@"
            data.Tables['table1'].Rows[0][1] = 'Maria';
           ", context);

      Assert.AreEqual("Maria", data.Tables["table1"].Rows[0][1]);

    }

    [TestMethod]
    public void FunctionScoping()
    {
      ScriptContext context = new ScriptContext();
      Script script = Script.Compile(@"
          y = 100;

          function f()
          { 
            y = y - 1;
          }
          f();          

          return y;
      ");

      object result = script.Execute();
      Assert.AreEqual(100, result);
    }


    [TestMethod]
    public void FunctionGlobalScoping()
    {
      ScriptContext context = new ScriptContext();
      Script script = Script.Compile(@"
          y = 100;

          function f() global(y)
          { 
            y = y - 1;
          }
          f();          

          return y;
      ");

      object result = script.Execute();
      Assert.AreEqual(99, result);
    }

    [TestMethod]
    public void FunctionExpressionGlobalScoping()
    {
      ScriptContext context = new ScriptContext();
      Script script = Script.Compile(@"
          y = 100;

          f = function() global(y)
          { 
            y = y - 1;
          };

          f();          

          return y;
      ");

      object result = script.Execute();
      Assert.AreEqual(99, result);
    }

    [TestMethod]
    public void FunctionRecursionWithLocalAssignment()
    {
      ScriptContext context = new ScriptContext();
      Script script = Script.Compile(@"
        function Draw(level)
        {           
          if (level>0)
          {
            level = level - 1;
            return Draw(level)+Draw(level);
          }

          return 1;
        }

        Draw(n);
      ");
      int n = 10;
      script.Context.SetItem("n", n);

      object result = script.Execute();
      Assert.AreEqual((int)Math.Pow(2,n), (int)result);
    }

    [TestMethod]
    public void SimpleSequentialCalls()
    {
      ScriptContext context = new ScriptContext();
      Script script = Script.Compile(@" 
        function f() {return [1,2,3];}
  
        return f()[1];
      ");

      object result = script.Execute();
      Assert.AreEqual(2, result);
    }

    [TestMethod]
    public void SequentialCalls()
    {
      ScriptContext context = new ScriptContext();
      Script script = Script.Compile(@" 
        function f() {return [1,2,3];}
        
        function q() {return [1,f,0];} 
 
        return q()[1]()[2];
      ");

      object result = script.Execute();
      Assert.AreEqual(3, result);
    }

    [TestMethod]
    public void ExternalFunctionCall()
    {
      ScriptContext context = new ScriptContext();
      Script script = Script.Compile(@"
        return Test();
      ");
      script.Context.SetItem("Test", new TestFunction());

      object result = script.Execute();
      Assert.AreEqual(10, result);
    }

    [TestMethod]
    public void Classes()
    {
      ScriptContext context = new ScriptContext();
      Script script = Script.Compile(@"
          school =
          [
            classes ->
              [
                A->['Alexey', 'Ivan','John'],
                B->['Petr','Valya','Masha','Joe']
              ],

             GetClasses->function ()
              {
                result = new List<|object|>(); 
                foreach(class in body.classes.Fields)
                   result.Add(class);
                
                return result;
              },

             GetClass->function (name)
              {
                result = new List<|object|>(); 
                foreach(student in body.classes[name] )
                     result.Add(class);
                
                return result;
              }

          ];

          school.GetClasses();
      ");

      List<object> result = (List<object>) script.Execute();
      Assert.AreEqual(2, result.Count);
    }
     
    [TestMethod]
    public void Graphs()
    {
      ScriptContext context = new ScriptContext();
      Script script = Script.Compile(@"
          //1
          //  2
          //   3
          //  4
          //  5

          u = [ 1, 2 , 3 , 4 , 5];
          v = [ [1,2], [2,3], [1,4], [1,5] ];

          g = [ 
                vertexes -> u,
                edges    -> v
              ];

          function IsTree(g)
          {
           vert = new ArrayList ();
           
           foreach (edge in g.edges)
           {
             if (!vert.Contains(edge[1]))
                  vert.Add(edge[1]);
             else
                  return false;
           } 

           if  ( vert.Count == g.vertexes.Length-1 )
           {
             foreach (root in g.vertexes)
              if ( ! vert.Contains(root))
              {
                 g.root =  root;
                 break;
               }

             return true;
           }
           else
              return false;
          }

          IsTree(g);

          root = g.root;

          s = '';
          foreach (x in v)
           s = s + x[0]+'->' + x[1] + ', ';

      ");

      string result = (string)script.Execute();
      Assert.AreEqual("1->2, 2->3, 1->4, 1->5, ", result);
    }

    //TODO: Review this approach
    [TestMethod]
    public void Threading()
    {
      ScriptContext context = new ScriptContext();
      //context.AddType("Thread", typeof(Thread));
      //context.AddType("ThreadStart", typeof(ThreadStart));
      //context.AddType("ParameterizedThreadStart", typeof(ParameterizedThreadStart));

      Script script = Script.Compile(@"
          function ThreadTest()
          { 
            //MessageBox.Show('Test Thread');
            //x = 'Test';
            return true;
          }

          th = new Thread(new ParameterizedThreadStart(ThreadTest, ThreadTest.ThreadInvoke));
          th.Start(Context);

          Thread.Sleep(1000);
      ");

      object result = script.Execute();
    }

    [TestMethod]
    public void MethodHandling()
    {
      object result = Script.RunCode(@"
             sin = Math.Sin;

             return sin(0.75);
      ");

      Assert.AreEqual(Math.Sin(0.75), result);
    }

    [TestMethod]
    public void MethodThroughInterface()
    {
      object result = Script.RunCode(@"
             a = new TestInterface();

             return a.Get();
      ");

      Assert.AreEqual(15, result);
    }

    [TestMethod]
    public void MethodThroughInterfaceExplicit()
    {
      object result = Script.RunCode(@"
             a = new TestInterface();
             i = new ExplicitInterface(a, ITest);
             
             return i.Get();
      ");

      Assert.AreEqual(2, result);
    }

    [TestMethod]
    public void MethodThroughInterfaceExplicit1()
    {
      object result = Script.RunCode(@"
             a = new TestInterfaceSingle();
             
             return a.Get();
      ");

      Assert.AreEqual(2, result);
    }

    [TestMethod]
    public void ConflictingLexicalTokens()
    {
      //Is a conflict between meta expression token <[   ]>
      //and array resolution with greater operator
      object result = Script.RunCode(@"
           a=[2,5,10];
           if (a[2]>a[1])
             return 2;
      ");
      Assert.AreEqual(2, result);

      //Is a conflict between meta expression token <!   !>
      //and array resolution with greater operator
      result = Script.RunCode(@"
           a=true;
           if (a<!a)
             return 5;
           else return 3;
      ");

      Assert.AreEqual(2, result);
    }

    [TestMethod]
    public void ComparingStrings()
    {
      object result = Script.RunCode(@"
           a = 'a';
           b = 'b';

           a>b;
           b<a;
           a>=b; 
           a<=b;
           a==b;
      ");

      Assert.AreEqual(false, result);
    }

    [TestMethod]
    public void CallToGenericMethod()
    {
      object result = Script.RunCode(@"
             a = new TestInterface();

             return a.GenericGet<|string|>('Hello World');
      ");

      Assert.AreEqual(new TestInterface().GenericGet<string>("Hello World"), result);
    }

    [TestMethod]
    public void UsingGenericArguments()
    {
      object result = Script.RunCode(@"
             a = new DoubleGeneric<|TestInterface, ITest1|>();

             return a.Get(new TestInterface());
      ");

      Assert.IsInstanceOfType(result, typeof(TestInterface));
    }

    [TestMethod]
    public void EvaluatingNetMethodsToIInvokable()
    {
      object result = Script.RunCode(@"
             a = DateTime.Now;
             b = a.ToString;
          
             return b();
      ");

      Assert.IsInstanceOfType(result, typeof(string));
    }

    [TestMethod]
    public void AccessingShadowedMembers()
    {
      object result = Script.RunCode(@"
              a = new TestShadowBase();
              b = new TestShadow();

              return a.Name + b.Name + a.Get() + b.Get();
      ");

      Assert.AreEqual("BaseShadowBaseGetShadowGet", result);
    }

    [TestMethod]
    public void AccessingExtensionMethods() {
      IEnumerable<int> input = new int[] { 1, 2, 3, 4, 5 };
      IScriptContext ctx = new ScriptContext();
      ctx.SetItem("input", input);

      object result = Script.RunCode(@"
              return input.Sum();
      ", ctx);

      Assert.AreEqual(input.Sum(), result);
    }

    [TestMethod]
    public void AccessingExtensionMethods1() {
        IEnumerable<object> input = new object[] { 1, "V1", 2, 3, 4, 5 };
        IScriptContext ctx = new ScriptContext();
        ctx.SetItem("input", input);

        object result = Script.RunCode(@"
              return input.OfType<|int|>().Count<|int|>();
        ", ctx);

        Assert.AreEqual(input.OfType<int>().Count(), result);
    }

    [TestMethod]
    public void AccessingExtensionMethods2_ImplictGenericsNotSupported() {
        IEnumerable<object> input = new object[] { 1, "V1", 2, 3, 4, 5 };
        IScriptContext ctx = new ScriptContext();
        ctx.SetItem("input", input);

        object result = Script.RunCode(@"
              return input.OfType<|int|>().Count();
        ", ctx);

        Assert.AreEqual(input.OfType<int>().Count(), result);
    }

    [TestMethod]
    public void AccessingExtensionMethodsWithLambdaExpressions() {
        IEnumerable<object> input = new object[] { 1, "V1", 2, 3, 4, 5 };

        Script result = Script.Compile(@"
              f = function(i) { return i>3; };
              return f;
        ");
        result.Context.SetItem("input", input);

        var f = result.Execute() as Scripting.SSharp.Parser.Ast.ScriptFunctionDefinition;
        var d = f.AsDelegate(typeof(Func<int, bool>));
        d.ActiveContext = result.Context;
        
        var rez = (bool)d.Method.DynamicInvoke(5);
        Assert.IsTrue(rez);

        rez = (bool)d.Method.DynamicInvoke(2);
        Assert.IsFalse(rez);

        var r = input.OfType<int>().Where<int>((Func<int,bool>)d.Method).ToArray();
        Assert.AreEqual(2, r.Length);


        result = Script.Compile(@"
              f = function(i) { return i>3; };
              return input.OfType<|int|>().Select<|int, bool|>(f).Count<|bool|>();
        ");
        result.Context.SetItem("input", input);
        result.Execute();

        Assert.AreEqual(input.OfType<int>().Select(i => i > 3).Count(), result.Context.Result);

        result = Script.Compile(@"
              f = function(i) { return i>3; };
              r = input.OfType<|int|>();
              c = r.Where<|int|>(f);
              return c.ToArray<|int|>().Length;
        ");
        result.Context.SetItem("input", input);
        result.Execute();

        Assert.AreEqual(input.OfType<int>().Where<int>(i => i > 3).ToArray().Length, result.Context.Result);
    }

    [TestMethod]
    public void AccessingExtensionMethodsWithLambdaExpressionsEx() {
        IEnumerable<object> input = new object[] { 1, 2, 3, 4, 5 };

        Script result = Script.Compile(@"
          return input.OfType<|int|>().Sum<|int|>(function (x) { return x*x; });
        ");
        result.Context.SetItem("input", input);
        result.Execute();

        Assert.AreEqual(input.OfType<int>().Sum(x=>x*x), result.Context.Result);
    }

    [TestMethod]
    public void TestForConstEvaluations() {
        Script result = Script.Compile(@"
          loop = 10;
          while (loop > 0){
            loop--;
            counter = (1+2)*3+15;
          }
        ");
        result.Execute();
    }

    [TestMethod]
    public void Modules() {
        RuntimeHost.ModuleManager.RegisterModule("sqrt", "function sqrt(x) { return Math.Sqrt(x); }");

        Script s = Script.Compile(@"
            #include<sqrt>
            return sqrt(4);
        ");

        Assert.AreEqual(2.0, s.Execute());
    }

    [TestMethod]
    public void ModulesDoesNotWorkWithString() {
        RuntimeHost.ModuleManager.RegisterModule("sqrt", "function sqrt(x) { return Math.Sqrt(x); }");

        Script s = Script.Compile(@"s=@'
            #include<sqrt>
            ';
            return s;
        ");

        Assert.AreEqual(@"
            #include<sqrt>
            ", s.Execute());
    }

  }

  public class TestFunction : IInvokable
  {
    #region IInvokable Members

    public bool CanInvoke()
    {
      return true;
    }

    public object Invoke(IScriptContext context, object[] args)
    {
      return 10;
    }

    #endregion
  }

  public interface ITest
  {
    int Get();
  }

  public interface ITest1
  {
    int Get();
  }

  public class TestInterface : ITest, ITest1
  {
    #region ITest Members

    int ITest.Get()
    {
      return 2;
    }

    #endregion

    #region ITest1 Members

    public int Get()
    {
      return 15;
    }

    #endregion

    public string GenericGet<T>(T input)
    {
      return input.ToString();
    }
  }

  public class TestInterfaceSingle : ITest
  {
    int ITest.Get()
    {
      return 2;
    }
  }

  public class DoubleGeneric<T, W> where T : W
  {
    public W Get(T input)
    {
      return (W)input;
    }
  }

  public class TestShadowBase
  {
    public string Name { get { return "Base"; } }

    public virtual string Get(){ return "BaseGet"; }
  }

  public class TestShadow : TestShadowBase
  {
    public new string Name { get { return "Shadow"; } }

    public new string Get() { return "ShadowGet"; }
  }
}
