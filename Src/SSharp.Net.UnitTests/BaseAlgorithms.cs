using System;
using Xunit;
using Scripting.SSharp.Runtime;
using Scripting.SSharp;

namespace UnitTests
{
  /// <summary>
  /// Summary description for BaseAlgorithms
  /// </summary>
  public class BaseAlgorithms : IDisposable
  {
    public BaseAlgorithms()
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
    public void IndexerDefinedByExplicitInterface()
    {
      Script script = Script.Compile(@"
          s = new SQLiteDataReader();
          return s['Test Value'];");
      RuntimeHost.AddType("SQLiteDataReader", typeof(SQLiteDataReader));

      Assert.Equal(10, script.Execute());
    }


    [Fact]
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

      Assert.Equal("Results:-3,-2,0,1,2,3,5,17,55", resultVal);
    }

    [Fact]
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

      Assert.Equal(" -9 0 1 1 2 3 3 4 4 5 10 12 23 34", result);
    }

    [Fact]
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

      Assert.Equal((int)1, resultVal);
    }

    [Fact]
    public void SimplePreconditionWithException()
    {
      Assert.Throws<ScriptVerificationException>( () =>
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
         ));
    }

    [Fact]
    public void UsingStatement()
    {
      object resultVal =
         Script.RunCode(@"using (Math) { return Pow(2,10); }");

      Assert.Equal((double)1024, resultVal);
    }

    [Fact]
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

      Assert.Equal("Hello", resultVal);
    }

    [Fact]
    public void OpertatorPresidence()
    {
      object resultVal = Script.RunCode(@"a=3; a-1>2;");
      Assert.False((bool)resultVal);

      resultVal = Script.RunCode(@"a=3; a==1 | true;");
      Assert.True((bool)resultVal);

    }

    [Fact]
    public void SimpleNameSpaces()
    {
      object resultVal = Script.RunCode(@"a = System.DateTime.Now;");

      Assert.IsType<DateTime>(resultVal);
    }

    [Fact]
    public void NameSpaces()
    {
      object rez = Script.RunCode(@"a = System.Text.RegularExpressions.Regex.CacheSize;");

      Assert.Equal(System.Text.RegularExpressions.Regex.CacheSize, rez);

      Assert.Equal(Scripting.SSharp.Runtime.RuntimeHost.Activator, Script.RunCode("return Scripting.SSharp.Runtime.RuntimeHost.Activator;"));
    }

    [Fact]
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

      Assert.Equal("jjjijjjijjji", resultVal);
    }

    [Fact]
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

      Assert.Equal("jjjijjjijjji", resultVal);
    }

    [Fact]
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

      Assert.Equal("124i124i124i", resultVal);
    }

    [Fact]
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

      Assert.Equal("jjijjijji", resultVal);
    }

    [Fact]
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

      Assert.Equal("0134i0134i0134i", resultVal);
    }

    [Fact]
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

      Assert.Equal("i01", resultVal);
    }

    [Fact]
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
      Assert.IsType<string>(resultVal);
    }

    [Fact]
    public void ThrowException()
    {
      Assert.Throws<ArgumentNullException>( () =>
        Script.RunCode(@"
           throw new ArgumentNullException('me');
          "));
    }

    [Fact]
    public void ExplicitTypeConvert()
    {
      object resultVal = Script.RunCode(@"
          a = 1.0;
          return (string)a;
      ");

      Assert.IsType<string>(resultVal);
    }

    [Fact]
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

      Assert.Equal("OK", resultVal);
    }

    [Fact]
    public void ScriptCompilerCompilesWrongProgramm()
    {
      Assert.Throws<ArgumentException>( () =>
        Script.RunCode(@"
          astNode = Compiler.Parse('wrong programm');
          if (astNode != null)
             astNode.Execute(new ScriptContext()).Value;
          else
           throw new ArgumentException('Syntax Error'); 
      "));
    }

    [Fact]
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

      Assert.Equal(2, resultVal);

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

      Assert.Equal("Hello", resultVal);

      resultVal = Script.RunCode(@"
            a = 5;
            switch(a)
            {
              case 5:
                b = 'Hello';
            }
            ");

      Assert.Equal("Hello", resultVal);
    }

    [Fact]
    public void BaseOperatorsUnary()
    {
      ScriptContext context = new ScriptContext();
      Script.RunCode(@"
        a = 4; b = 2;
        a++;
        b--;
        ", context);

      Assert.Equal(5, context.GetItem("a", true));
      Assert.Equal(1, context.GetItem("b", true));
    }

    [Fact]
    public void BaseOperatorIs()
    {
      ScriptContext context = new ScriptContext();

      Script.RunCode(@"
        a = 4;
        result =  a is int;
        ", context);

      Assert.True((bool)context.GetItem("result", true));
    }

    [Fact]
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
      Assert.Equal(1, rez.test);
      Assert.True(rez.IsEventNull());
      Assert.False(EventBroker.HasSubscriptions);
    }

    [Fact]
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


    [Fact]
    public void ContractScopeThrowsException()
    {
      Assert.Throws<ScriptVerificationException>( () =>
       Script.RunCode(@"
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
      
       "));
    }

    [Fact]
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

      Assert.Equal(10, rez);
    }

    [Fact]
    public void PlusEqualOperator()
    {
      Script rez = Script.Compile(@"
         a = new SumOp();
         a.a += 2;   

         return a.a;  
       ");

      RuntimeHost.AddType("SumOp", typeof(SumOp));

      Assert.Equal(2, rez.Execute());
    }


    [Fact]
    public void MissingNamespaceException()
    {
      Assert.Throws<ScriptIdNotFoundException>( () =>
       Script.RunCode(@"
          MyMissingSpace.NullValue;
       "));
    }

    [Fact]
    public void PostprocessingsSetsDeclaredFunctions()
    {
      object rez = Script.RunCode(@"
          return f();

          function f() {return 1;}
       ");

      Assert.Equal(1, rez);
    }

    private class SumOp
    {
      public int a = 0;
    }

    [Fact]
    public void ScriptCreatesChar()
    {
      object rez = Script.RunCode(@"
         return (Char)char(';');
      ");

      Assert.Equal(';', rez);
    }

    [Fact]
    public void BinaryOperatorHelpers()
    {
      object rez = Script.RunCode(@"
         a = null;
         return false && a.b;
      ");

      Assert.False((bool)rez);

      rez = Script.RunCode(@"
         a = null;
         return true || a.b;
      ");

      Assert.True((bool)rez);

      rez = Script.RunCode(@"
         return true && false;
      ");

      Assert.False((bool)rez);
    }

    [Fact]
    public void BitOperators() {
        object rez = Script.RunCode(@"
         a = 1;
         b = 2;
         return a|b;
      ");

        Assert.Equal(3, rez);

        rez = Script.RunCode(@"
         a = 1;
         b = 2;
         return a&b;
      ");

        Assert.Equal(0, rez);
    }

    [Fact]
    public void ComparingCharProperties()
    {
      object rez = Script.RunCode(@"
         c = new CharValueObject();
         return c.CharValue == char('C');
      ");

      Assert.True((bool)rez);
    }

    [Fact]
    public void DecimalValueTests()
    {
      object rez = Script.RunCode(@"
         d = new DecimalValueObject();
         return d.V1 == d.V2;
      ");

      Assert.False((bool)rez);

      rez = Script.RunCode(@"
         d = new DecimalValueObject();
         return d.V1 == d.V1;
      ");

      Assert.True((bool)rez);

      rez = Script.RunCode(@"
         d = new DecimalValueObject();
         return d.V1 + d.V2;
      ");

      Assert.Equal(45m+5m, rez);

      rez = Script.RunCode(@"
         d = new DecimalValueObject();
         return d.V1 > d.V2;
      ");

      Assert.True((bool)rez);

      rez = Script.RunCode(@"
         d = new DecimalValueObject();
         return d.V1 < d.V2;
      ");

      Assert.False((bool)rez);

      rez = Script.RunCode(@"
         d = new DecimalValueObject();
         return d.V1 == 45m;
      ");
      Assert.True((bool)rez);

      rez = Script.RunCode(@"
         d = new DecimalValueObject();
         return d.V1 == 45;
      ");
      Assert.True((bool)rez);

    }

    [Fact]
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
      Assert.Equal(i16/dc, rez);

      rez = Script.RunCode(@"
            return i32 / dc;
      ", context);
      Assert.Equal(i32 / dc, rez);

      rez = Script.RunCode(@"
            return i64 / dc;
      ", context);
      Assert.Equal(i64 / dc, rez);

      rez = Script.RunCode(@"
            return new decimal(f) / dc;
      ", context);
      Assert.Equal(new decimal(f) / dc, rez);

      rez = Script.RunCode(@"
            return new decimal(d) / dc;
      ", context);
      Assert.Equal(new decimal(d) / dc, rez);

      // Reverse

      rez = Script.RunCode(@"
            return  dc / i16;
      ", context);
      Assert.Equal(dc / i16, rez);

      rez = Script.RunCode(@"
            return dc / i32;
      ", context);
      Assert.Equal(dc / i32, rez);

      rez = Script.RunCode(@"
            return dc / i64;
      ", context);
      Assert.Equal(dc / i64, rez);

      rez = Script.RunCode(@"
            return dc / new decimal(f);
      ", context);
      Assert.Equal(dc / new decimal(f), rez);

      rez = Script.RunCode(@"
            return dc / new decimal(d);
      ", context);
      Assert.Equal(dc / new decimal(d), rez);
    }
    
    [Fact]
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
      Assert.Equal(i16 + dc, rez);

      rez = Script.RunCode(@"
            return i32 + dc;
      ", context);
      Assert.Equal(i32 + dc, rez);

      rez = Script.RunCode(@"
            return i64 + dc;
      ", context);
      Assert.Equal(i64 + dc, rez);

      rez = Script.RunCode(@"
            return new decimal(f) + dc;
      ", context);
      Assert.Equal(new decimal(f) + dc, rez);

      rez = Script.RunCode(@"
            return new decimal(d) + dc;
      ", context);
      Assert.Equal(new decimal(d) + dc, rez);

      // Reverse

      rez = Script.RunCode(@"
            return  dc + i16;
      ", context);
      Assert.Equal(dc + i16, rez);

      rez = Script.RunCode(@"
            return dc + i32;
      ", context);
      Assert.Equal(dc + i32, rez);

      rez = Script.RunCode(@"
            return dc + i64;
      ", context);
      Assert.Equal(dc + i64, rez);

      rez = Script.RunCode(@"
            return dc + new decimal(f);
      ", context);
      Assert.Equal(dc + new decimal(f), rez);

      rez = Script.RunCode(@"
            return dc + new decimal(d);
      ", context);
      Assert.Equal(dc + new decimal(d), rez);
    }

    [Fact]
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
      Assert.Equal(i16 - dc, rez);

      rez = Script.RunCode(@"
            return i32 - dc;
      ", context);
      Assert.Equal(i32 - dc, rez);

      rez = Script.RunCode(@"
            return i64 - dc;
      ", context);
      Assert.Equal(i64 - dc, rez);

      rez = Script.RunCode(@"
            return new decimal(f) - dc;
      ", context);
      Assert.Equal(new decimal(f) - dc, rez);

      rez = Script.RunCode(@"
            return new decimal(d) - dc;
      ", context);
      Assert.Equal(new decimal(d) - dc, rez);

      // Reverse

      rez = Script.RunCode(@"
            return  dc - i16;
      ", context);
      Assert.Equal(dc - i16, rez);

      rez = Script.RunCode(@"
            return dc - i32;
      ", context);
      Assert.Equal(dc - i32, rez);

      rez = Script.RunCode(@"
            return dc - i64;
      ", context);
      Assert.Equal(dc - i64, rez);

      rez = Script.RunCode(@"
            return dc - new decimal(f);
      ", context);
      Assert.Equal(dc - new decimal(f), rez);

      rez = Script.RunCode(@"
            return dc - new decimal(d);
      ", context);
      Assert.Equal(dc - new decimal(d), rez);
    }
    
    [Fact]
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
      Assert.Equal(i16 * dc, rez);

      rez = Script.RunCode(@"
            return i32 * dc;
      ", context);
      Assert.Equal(i32 * dc, rez);

      rez = Script.RunCode(@"
            return i64 * dc;
      ", context);
      Assert.Equal(i64 * dc, rez);

      rez = Script.RunCode(@"
            return new decimal(f) * dc;
      ", context);
      Assert.Equal(new decimal(f) * dc, rez);

      rez = Script.RunCode(@"
            return new decimal(d) * dc;
      ", context);
      Assert.Equal(new decimal(d) * dc, rez);

      // Reverse

      rez = Script.RunCode(@"
            return  dc * i16;
      ", context);
      Assert.Equal(dc * i16, rez);

      rez = Script.RunCode(@"
            return dc * i32;
      ", context);
      Assert.Equal(dc * i32, rez);

      rez = Script.RunCode(@"
            return dc * i64;
      ", context);
      Assert.Equal(dc * i64, rez);

      rez = Script.RunCode(@"
            return dc * new decimal(f);
      ", context);
      Assert.Equal(dc * new decimal(f), rez);

      rez = Script.RunCode(@"
            return dc * new decimal(d);
      ", context);
      Assert.Equal(dc * new decimal(d), rez);
    }

    [Fact]
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
      Assert.Equal(i16 % dc, rez);

      rez = Script.RunCode(@"
            return i32 % dc;
      ", context);
      Assert.Equal(i32 % dc, rez);

      rez = Script.RunCode(@"
            return i64 % dc;
      ", context);
      Assert.Equal(i64 % dc, rez);

      rez = Script.RunCode(@"
            return new decimal(f) % dc;
      ", context);
      Assert.Equal(new decimal(f) % dc, rez);

      rez = Script.RunCode(@"
            return new decimal(d) % dc;
      ", context);
      Assert.Equal(new decimal(d) % dc, rez);

      // Reverse

      rez = Script.RunCode(@"
            return  dc % i16;
      ", context);
      Assert.Equal(dc % i16, rez);

      rez = Script.RunCode(@"
            return dc % i32;
      ", context);
      Assert.Equal(dc % i32, rez);

      rez = Script.RunCode(@"
            return dc % i64;
      ", context);
      Assert.Equal(dc % i64, rez);

      rez = Script.RunCode(@"
            return dc % new decimal(f);
      ", context);
      Assert.Equal(dc % new decimal(f), rez);

      rez = Script.RunCode(@"
            return dc % new decimal(d);
      ", context);
      Assert.Equal(dc % new decimal(d), rez);
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
