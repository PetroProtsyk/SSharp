using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Runtime.Promotion;
using Scripting.SSharp;

namespace UnitTests
{
  /// <summary>
  /// Summary description for BaseAlgorithms
  /// </summary>
  [TestClass]
  public class BaseAlgorithms
  {
    public BaseAlgorithms()
    {
    }

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
    public void IndexerDefinedByExplicitInterface()
    {
      Script script = Script.Compile(@"
          s = new SQLiteDataReader();
          return s['Test Value'];");
      RuntimeHost.AddType("SQLiteDataReader", typeof(SQLiteDataReader));

      Assert.AreEqual(10, script.Execute());
    }


    [TestMethod]
    public void BubbleSort()
    {
      object resultVal =
         Script.RunCode(
         @"
         a=[17,-2, 0,-3, 5, 3,1, 2, 55];

         for (i=0; i < a.Length; i=i+1)
          for (j=i+1; j <  a.Length; j=j+1)
           if (a[i] > a[j] )
           {
             temp = a[i]; 
             a[i] = a[j];
             a[j] = temp;
           }

          s = 'Results:';
          for (i=0; i < a.Length; i++)
           if (i!=0)
             s = s + ',' + a[i];
           else
             s += a[i];"
         );

      Assert.AreEqual("Results:-3,-2,0,1,2,3,5,17,55", resultVal);
    }

    [TestMethod]
    public void QuickSort()
    {
      object result =
         Script.RunCode(@"         
         function swap(array, a, b)
         {
           tmp=array[a];
           array[a]=array[b];
           array[b]=tmp;
         }
 
         function partition(array, begin, end, pivot)
         {
           piv=array[pivot];
           swap(array, pivot, end-1);	
           store=begin;
           for(ix=begin; ix < end-1; ix++) {
            if(array[ix]<=piv) {
              swap(array, store, ix);
              store++;
             }
           }
           swap(array, end-1, store);
           return store;
         }

         function qsort(array, begin, end)
         {
           if(end-1>begin) {
             pivot=begin+(end-begin) / 2;
             pivot=partition(array, begin, end, pivot);
             qsort(array, begin, pivot);
             qsort(array, pivot+1, end);
           }
         }

         a = [1,2,10,0,12,34,-9,5,3,3,4,1,23,4];
         qsort(a, 0, a.Length);
         s='';
         for (i=0; i < a.Length; i++)
           s = s+' '+a[i];
         "
         );

      Assert.AreEqual(" -9 0 1 1 2 3 3 4 4 5 10 12 23 34", result);
    }

    [TestMethod]
    public void SimplePrecondition()
    {
      object resultVal =
         Script.RunCode(
         @"function d(a,b,c)
          [
            pre(a>0);
            post();
            invariant();
          ]
          {
            return 1;
          }
          d(1,2,3);"
         );

      Assert.AreEqual((int)1, resultVal);
    }

    [TestMethod]
    [ExpectedException(typeof(ScriptVerificationException))]
    public void SimplePreconditionWithException()
    {
      object resultVal =
         Script.RunCode(
         @"function d(a,b,c)
          [
            pre(a>0);
            post();
            invariant();
          ]
          {
            return 1;
          }
          d(-1,2,3);"
         );
    }

    [TestMethod]
    public void UsingStatement()
    {
      object resultVal =
         Script.RunCode(@"using (Math) { return Pow(2,10); }");

      Assert.AreEqual((double)1024, resultVal);
    }

    [TestMethod]
    public void GenericsArray()
    {
      object resultVal =
        Script.RunCode(@"v = new int[10];
            for(i=0; i<v.Length; i++)
              v[i] = i;
            s = ''; 
            foreach(i in v)
              s = s + i + ' ';
            a = new List<|string|>[10];
            a[0] = new List<|string|>();
            a[0].Add('Hello');
            b = a[0].ToArray();
            c = b[0];");

      Assert.AreEqual("Hello", resultVal);
    }

    [TestMethod]
    public void OpertatorPresidence()
    {
      object resultVal = Script.RunCode(@"a=3; a-1>2;");
      Assert.AreEqual(false, resultVal);

      resultVal = Script.RunCode(@"a=3; a==1 | true;");
      Assert.AreEqual(true, resultVal);

    }

    [TestMethod]
    public void SimpleNameSpaces()
    {
      object resultVal = Script.RunCode(@"a = System.DateTime.Now;");

      Assert.IsInstanceOfType(resultVal, typeof(DateTime));
    }

    [TestMethod]
    public void NameSpaces()
    {
      object rez = Script.RunCode(@"a = System.Text.RegularExpressions.Regex.CacheSize;");

      Assert.AreEqual(System.Text.RegularExpressions.Regex.CacheSize, rez);
    }

    [TestMethod]
    public void ForBreak()
    {
      object resultVal = Script.RunCode(@"
          s = '';   
          for (i=0;i<3;i++)
          {
             for (j=0;j<5;j++)
             {
               if (j==3) break;
               s+='j';
             }
             s+='i';
          }
          ");

      Assert.AreEqual("jjjijjjijjji", resultVal);
    }

    [TestMethod]
    public void WhileBreak()
    {
      object resultVal = Script.RunCode(@"
          s = '';   
          for (i=0;i<3;i++)
          {
             j = 0;
             while(j<5)
             {
               if (j==3) break;
               s+='j';
               j++;
             }
             s+='i';
          }
          ");

      Assert.AreEqual("jjjijjjijjji", resultVal);
    }

    [TestMethod]
    public void ForEachBreak()
    {
      object resultVal = Script.RunCode(@"
          a=[1,2,4,3,4,5];
          s = '';   
          for (i=0;i<3;i++)
          {
             foreach(j in a)
             {
               if (j==3) break;
               s+=j.ToString();
             }
             s+='i';
          }
          ");

      Assert.AreEqual("124i124i124i", resultVal);
    }

    [TestMethod]
    public void ProgReturn()
    {
      object resultVal = Script.RunCode(@"
          s = '';    
          for (i=0;i<3;i++)
          {
             for (j=0;j<5;j++)
             {
               if (j==2) break;
               s+='j';
             }
             s+='i';
          }
         return s;
         a = [17,-2, 0,-3, 5, 3,1, 2, 55];

         for (i=0; i < a.Length; i=i+1)
          for (j=i+1; j <  a.Length; j=j+1)
           if (a[i] > a[j] )
           {
             temp = a[i]; 
             a[i] = a[j];
             a[j] = temp;
           }

          s = 'Results:';
          for (i=0; i < a.Length; i++)
           if (i!=0)
             s = s + ',' + a[i];
           else
             s += a[i];");

      Assert.AreEqual("jjijjijji", resultVal);
    }

    [TestMethod]
    public void ForContinue()
    {
      object resultVal = Script.RunCode(@"
          s = '';    
          for (i=0;i<3;i++)
          {
             for (j=0;j<5;j++)
             {
               if (j==2) continue;
               s+=j.ToString();
             }
             s+='i';
          }
         return s;");

      Assert.AreEqual("0134i0134i0134i", resultVal);
    }

    [TestMethod]
    public void FuncReturn()
    {
      object resultVal = Script.RunCode(@"
          s = '';    
          function Test(){
          for (i=0;i<3;i++)
           {
             s+='i';
             for (j=0;j<5;j++)
             {
               if (j==2) return s;
               s+=j.ToString();
             }
           }
          }
         return Test();");

      Assert.AreEqual("i01", resultVal);
    }

    [TestMethod]
    public void TryCatch()
    {
      Script result = Script.Compile(@"
          try
          {
            a = c/0;
          }
          catch(e)
          {
            msg = e.Message;
          }
          finally
          {
            return 'Message:'+msg;
          }");
      result.Context.SetItem("c", 10);

      object resultVal = result.Execute();
      Assert.IsInstanceOfType(resultVal, typeof(string));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ThrowException()
    {
      Script.RunCode(@"
           throw new ArgumentNullException('me');
          ");
    }

    [TestMethod]
    public void ExplicitTypeConvert()
    {
      object resultVal = Script.RunCode(@"
          a = 1.0;
          return (string)a;
      ");

      Assert.IsInstanceOfType(resultVal, typeof(string));
    }

    [TestMethod]
    public void ComparingWithNull()
    {
      object resultVal = Script.RunCode(@"
          a = null;
          if (a!=null)
          {
           throw new Exception('This should not happen'); 
          }
          else
           return 'OK';
      ");

      Assert.AreEqual("OK", resultVal);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ScriptCompilerCompilesWrongProgramm()
    {
      Script.RunCode(@"
          astNode = Compiler.Parse('wrong programm');
          if (astNode != null)
             astNode.Execute(new ScriptContext()).Value;
          else        
           throw new ArgumentException('Syntax Error'); 
      ");
    }

    [TestMethod]
    public void SwitchStatement()
    {
      object resultVal = Script.RunCode(@"
            a = 5;
            switch(a)
            {
              default:
                return 2;
            }
            ");

      Assert.AreEqual(2, resultVal);

      resultVal = Script.RunCode(@"
            a = 5;
            switch(a)
            {
              case 1:
                b = 3;
              case 5:
                b = 'Hello';
              default:
                b = 2;
            }
            ");

      Assert.AreEqual("Hello", resultVal);

      resultVal = Script.RunCode(@"
            a = 5;
            switch(a)
            {
              case 5:
                b = 'Hello';
            }
            ");

      Assert.AreEqual("Hello", resultVal);
    }

    [TestMethod]
    public void BaseOperatorsUnary()
    {
      ScriptContext context = new ScriptContext();
      Script.RunCode(@"
        a = 4; b = 2;
        a++;
        b--;
        ", context);

      Assert.AreEqual(5, context.GetItem("a", true));
      Assert.AreEqual(1, context.GetItem("b", true));
    }

    [TestMethod]
    public void BaseOperatorIs()
    {
      ScriptContext context = new ScriptContext();

      Script.RunCode(@"
        a = 4;
        result =  a is int;
        ", context);

      Assert.AreEqual(true, context.GetItem("result", true));
    }

    [TestMethod]
    public void EventInvokation()
    {
      Script script2 = Script.Compile(@"function f(sender){ sender.test = 1;}
       c = new EventHelperInvocation();
       c.test = 0;
       c.eva+=f;
       c.OnEva();
       c.eva-=f;
       return c;
       ");
      EventHelperInvocation rez = (EventHelperInvocation)script2.Execute();
      EventBroker.ClearAllSubscriptions();
      Assert.AreEqual(1, rez.test);
      Assert.IsTrue(rez.IsEventNull());
      Assert.IsFalse(EventBroker.HasSubscriptions);
    }

    [TestMethod]
    public void DuplicateEventSubscriptionSupported()
    {
      Script script2 = Script.Compile(@"function f(sender){ sender.test = 1;}
       c = new EventHelperInvocation();
       c.test = 0;
       c.eva+=f;
       c.eva+=f;
       ");
      script2.Execute();
    }


    [TestMethod]
    [ExpectedException(typeof(ScriptVerificationException))]
    public void ContractScopeThrowsException()
    {
      object rez = Script.RunCode(@"
         function F1(x)
         [
          pre(x is int); 
          post(x is int);
          invariant(x is int);
         ]
         {
           x = 10;
           x = 'heelo!';
         }

         F1(2);
      
       ");
    }

    [TestMethod]
    public void ContractScope()
    {
      object rez = Script.RunCode(@"
         function F1(x)
         [
          pre(x is int); 
          post(x is int);
          invariant(x is int);
         ]
         {
           x = 10;
         }

         F1(2);
      
       ");

      Assert.AreEqual(10, rez);
    }

    [TestMethod]
    public void PlusEqualOperator()
    {
      Script rez = Script.Compile(@"
         a = new SumOp();
         a.a += 2;   

         return a.a;  
       ");

      RuntimeHost.AddType("SumOp", typeof(SumOp));

      Assert.AreEqual(2, rez.Execute());
    }


    [TestMethod]
    [ExpectedException(typeof(ScriptIdNotFoundException))]
    public void MissingNamespaceException()
    {
      Script.RunCode(@"
          MyMissingSpace.NullValue;
       ");
    }

    [TestMethod]
    public void PostprocessingsSetsDeclaredFunctions()
    {
      object rez = Script.RunCode(@"
          return f();

          function f() {return 1;}
       ");

      Assert.AreEqual(1, rez);
    }

    private class SumOp
    {
      public int a = 0;
    }

    [TestMethod]
    public void ScriptCreatesChar()
    {
      object rez = Script.RunCode(@"
         return (Char)char(';');
      ");

      Assert.AreEqual(';', rez);
    }

    [TestMethod]
    public void BinaryOperatorHelpers()
    {
      object rez = Script.RunCode(@"
         a = null;
         return false && a.b;
      ");

      Assert.AreEqual(false, rez);

      rez = Script.RunCode(@"
         a = null;
         return true || a.b;
      ");

      Assert.AreEqual(true, rez);

      rez = Script.RunCode(@"
         return true && false;
      ");

      Assert.AreEqual(false, rez);
    }

    [TestMethod]
    public void ComparingCharProperties()
    {
      object rez = Script.RunCode(@"
         c = new CharValueObject();
         return c.CharValue == char('C');
      ");

      Assert.AreEqual(true, rez);
    }

    [TestMethod]
    public void DecimalValueTests()
    {
      object rez = Script.RunCode(@"
         d = new DecimalValueObject();
         return d.V1 == d.V2;
      ");

      Assert.AreEqual(false, rez);

      rez = Script.RunCode(@"
         d = new DecimalValueObject();
         return d.V1 == d.V1;
      ");

      Assert.AreEqual(true, rez);

      rez = Script.RunCode(@"
         d = new DecimalValueObject();
         return d.V1 + d.V2;
      ");

      Assert.AreEqual(45m+5m, rez);

      rez = Script.RunCode(@"
         d = new DecimalValueObject();
         return d.V1 > d.V2;
      ");

      Assert.AreEqual(true, rez);

      rez = Script.RunCode(@"
         d = new DecimalValueObject();
         return d.V1 < d.V2;
      ");

      Assert.AreEqual(false, rez);

      rez = Script.RunCode(@"
         d = new DecimalValueObject();
         return d.V1 == 45m;
      ");
      Assert.AreEqual(true, rez);

      rez = Script.RunCode(@"
         d = new DecimalValueObject();
         return d.V1 == 45;
      ");
      Assert.AreEqual(true, rez);

    }

    [TestMethod]
    public void DecimalDivTests()
    {
      System.Int16 i16 = 10;
      System.Int32 i32 = 10;
      System.Int64 i64 = 10;
      System.Double d = 10;
      System.Single f = 10;
      System.Decimal dc = 2;
      object rez = null;

      IScriptContext context = new ScriptContext();
      context.SetItem("i16", i16);
      context.SetItem("i32", i32);
      context.SetItem("i64", i64);
      context.SetItem("d", d);
      context.SetItem("f", f);
      context.SetItem("dc", dc);

      rez = Script.RunCode(@"
            return i16 / dc;
      ", context);
      Assert.AreEqual(i16/dc, rez);

      rez = Script.RunCode(@"
            return i32 / dc;
      ", context);
      Assert.AreEqual(i32 / dc, rez);

      rez = Script.RunCode(@"
            return i64 / dc;
      ", context);
      Assert.AreEqual(i64 / dc, rez);

      rez = Script.RunCode(@"
            return new decimal(f) / dc;
      ", context);
      Assert.AreEqual(new decimal(f) / dc, rez);

      rez = Script.RunCode(@"
            return new decimal(d) / dc;
      ", context);
      Assert.AreEqual(new decimal(d) / dc, rez);

      // Reverse

      rez = Script.RunCode(@"
            return  dc / i16;
      ", context);
      Assert.AreEqual(dc / i16, rez);

      rez = Script.RunCode(@"
            return dc / i32;
      ", context);
      Assert.AreEqual(dc / i32, rez);

      rez = Script.RunCode(@"
            return dc / i64;
      ", context);
      Assert.AreEqual(dc / i64, rez);

      rez = Script.RunCode(@"
            return dc / new decimal(f);
      ", context);
      Assert.AreEqual(dc / new decimal(f), rez);

      rez = Script.RunCode(@"
            return dc / new decimal(d);
      ", context);
      Assert.AreEqual(dc / new decimal(d), rez);
    }
    
    [TestMethod]
    public void DecimalPlusTests()
    {
      System.Int16 i16 = 10;
      System.Int32 i32 = 10;
      System.Int64 i64 = 10;
      System.Double d = 10;
      System.Single f = 10;
      System.Decimal dc = 2;
      object rez = null;

      IScriptContext context = new ScriptContext();
      context.SetItem("i16", i16);
      context.SetItem("i32", i32);
      context.SetItem("i64", i64);
      context.SetItem("d", d);
      context.SetItem("f", f);
      context.SetItem("dc", dc);

      rez = Script.RunCode(@"
            return i16 + dc;
      ", context);
      Assert.AreEqual(i16 + dc, rez);

      rez = Script.RunCode(@"
            return i32 + dc;
      ", context);
      Assert.AreEqual(i32 + dc, rez);

      rez = Script.RunCode(@"
            return i64 + dc;
      ", context);
      Assert.AreEqual(i64 + dc, rez);

      rez = Script.RunCode(@"
            return new decimal(f) + dc;
      ", context);
      Assert.AreEqual(new decimal(f) + dc, rez);

      rez = Script.RunCode(@"
            return new decimal(d) + dc;
      ", context);
      Assert.AreEqual(new decimal(d) + dc, rez);

      // Reverse

      rez = Script.RunCode(@"
            return  dc + i16;
      ", context);
      Assert.AreEqual(dc + i16, rez);

      rez = Script.RunCode(@"
            return dc + i32;
      ", context);
      Assert.AreEqual(dc + i32, rez);

      rez = Script.RunCode(@"
            return dc + i64;
      ", context);
      Assert.AreEqual(dc + i64, rez);

      rez = Script.RunCode(@"
            return dc + new decimal(f);
      ", context);
      Assert.AreEqual(dc + new decimal(f), rez);

      rez = Script.RunCode(@"
            return dc + new decimal(d);
      ", context);
      Assert.AreEqual(dc + new decimal(d), rez);
    }

    [TestMethod]
    public void DecimalMinusTests()
    {
      System.Int16 i16 = 10;
      System.Int32 i32 = 10;
      System.Int64 i64 = 10;
      System.Double d = 10;
      System.Single f = 10;
      System.Decimal dc = 2;
      object rez = null;

      IScriptContext context = new ScriptContext();
      context.SetItem("i16", i16);
      context.SetItem("i32", i32);
      context.SetItem("i64", i64);
      context.SetItem("d", d);
      context.SetItem("f", f);
      context.SetItem("dc", dc);

      rez = Script.RunCode(@"
            return i16 - dc;
      ", context);
      Assert.AreEqual(i16 - dc, rez);

      rez = Script.RunCode(@"
            return i32 - dc;
      ", context);
      Assert.AreEqual(i32 - dc, rez);

      rez = Script.RunCode(@"
            return i64 - dc;
      ", context);
      Assert.AreEqual(i64 - dc, rez);

      rez = Script.RunCode(@"
            return new decimal(f) - dc;
      ", context);
      Assert.AreEqual(new decimal(f) - dc, rez);

      rez = Script.RunCode(@"
            return new decimal(d) - dc;
      ", context);
      Assert.AreEqual(new decimal(d) - dc, rez);

      // Reverse

      rez = Script.RunCode(@"
            return  dc - i16;
      ", context);
      Assert.AreEqual(dc - i16, rez);

      rez = Script.RunCode(@"
            return dc - i32;
      ", context);
      Assert.AreEqual(dc - i32, rez);

      rez = Script.RunCode(@"
            return dc - i64;
      ", context);
      Assert.AreEqual(dc - i64, rez);

      rez = Script.RunCode(@"
            return dc - new decimal(f);
      ", context);
      Assert.AreEqual(dc - new decimal(f), rez);

      rez = Script.RunCode(@"
            return dc - new decimal(d);
      ", context);
      Assert.AreEqual(dc - new decimal(d), rez);
    }
    
    [TestMethod]
    public void DecimalMulTests()
    {
      System.Int16 i16 = 10;
      System.Int32 i32 = 10;
      System.Int64 i64 = 10;
      System.Double d = 10;
      System.Single f = 10;
      System.Decimal dc = 2;
      object rez = null;

      IScriptContext context = new ScriptContext();
      context.SetItem("i16", i16);
      context.SetItem("i32", i32);
      context.SetItem("i64", i64);
      context.SetItem("d", d);
      context.SetItem("f", f);
      context.SetItem("dc", dc);

      rez = Script.RunCode(@"
            return i16 * dc;
      ", context);
      Assert.AreEqual(i16 * dc, rez);

      rez = Script.RunCode(@"
            return i32 * dc;
      ", context);
      Assert.AreEqual(i32 * dc, rez);

      rez = Script.RunCode(@"
            return i64 * dc;
      ", context);
      Assert.AreEqual(i64 * dc, rez);

      rez = Script.RunCode(@"
            return new decimal(f) * dc;
      ", context);
      Assert.AreEqual(new decimal(f) * dc, rez);

      rez = Script.RunCode(@"
            return new decimal(d) * dc;
      ", context);
      Assert.AreEqual(new decimal(d) * dc, rez);

      // Reverse

      rez = Script.RunCode(@"
            return  dc * i16;
      ", context);
      Assert.AreEqual(dc * i16, rez);

      rez = Script.RunCode(@"
            return dc * i32;
      ", context);
      Assert.AreEqual(dc * i32, rez);

      rez = Script.RunCode(@"
            return dc * i64;
      ", context);
      Assert.AreEqual(dc * i64, rez);

      rez = Script.RunCode(@"
            return dc * new decimal(f);
      ", context);
      Assert.AreEqual(dc * new decimal(f), rez);

      rez = Script.RunCode(@"
            return dc * new decimal(d);
      ", context);
      Assert.AreEqual(dc * new decimal(d), rez);
    }

    [TestMethod]
    public void DecimalModTests()
    {
      System.Int16 i16 = 10;
      System.Int32 i32 = 10;
      System.Int64 i64 = 10;
      System.Double d = 10;
      System.Single f = 10;
      System.Decimal dc = 2;
      object rez = null;

      IScriptContext context = new ScriptContext();
      context.SetItem("i16", i16);
      context.SetItem("i32", i32);
      context.SetItem("i64", i64);
      context.SetItem("d", d);
      context.SetItem("f", f);
      context.SetItem("dc", dc);

      rez = Script.RunCode(@"
            return i16 % dc;
      ", context);
      Assert.AreEqual(i16 % dc, rez);

      rez = Script.RunCode(@"
            return i32 % dc;
      ", context);
      Assert.AreEqual(i32 % dc, rez);

      rez = Script.RunCode(@"
            return i64 % dc;
      ", context);
      Assert.AreEqual(i64 % dc, rez);

      rez = Script.RunCode(@"
            return new decimal(f) % dc;
      ", context);
      Assert.AreEqual(new decimal(f) % dc, rez);

      rez = Script.RunCode(@"
            return new decimal(d) % dc;
      ", context);
      Assert.AreEqual(new decimal(d) % dc, rez);

      // Reverse

      rez = Script.RunCode(@"
            return  dc % i16;
      ", context);
      Assert.AreEqual(dc % i16, rez);

      rez = Script.RunCode(@"
            return dc % i32;
      ", context);
      Assert.AreEqual(dc % i32, rez);

      rez = Script.RunCode(@"
            return dc % i64;
      ", context);
      Assert.AreEqual(dc % i64, rez);

      rez = Script.RunCode(@"
            return dc % new decimal(f);
      ", context);
      Assert.AreEqual(dc % new decimal(f), rez);

      rez = Script.RunCode(@"
            return dc % new decimal(d);
      ", context);
      Assert.AreEqual(dc % new decimal(d), rez);
    }
  }

  #region Helpers
  public interface IS
  {
    int this[string index] { get; }
  }

  public class SQLiteDataReader : IS
  {
    #region IS Members

    int IS.this[string index]
    {
      get { return index.Length; }
    }

    #endregion
  }

  public class EventHelperInvocation
  {
    public int test;
    public event System.EventHandler eva;

    public bool IsEventNull()
    {
      return eva == null;
    }

    public void OnEva()
    {
      eva.Invoke(this, EventArgs.Empty);
    }
  }

  public class CharValueObject
  {
    public char CharValue
    {
      get
      {
        return 'C';
      }
    }
  }

  public class DecimalValueObject
  {
    public decimal V1
    {
      get
      {
        return 45m;
      }
    }

    public decimal V2
    {
      get
      {
        return 5m;
      }
    }
  }
  #endregion
}
