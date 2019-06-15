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
  /// Tests MutantFramework
  /// </summary>
  public class ScriptableObjects : IDisposable
  {
    public ScriptableObjects()
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
    public void WrapperHasAccessToObjectProperties()
    {
      Script script = Script.Compile(@"
           a = [1,2,3];
           a.Length;

           b = new ExpandoWrapper(a);
           b.Length;
       ");

      object rez = script.Execute();

      Assert.Equal(3, rez);
    }

    [Fact]
    public void WrapperHasAccessToObjectPropertiesIndexer()
    {
      Script script = Script.Compile(@"
           a = [1,2,3];
           a.Length;

           b = new ExpandoWrapper(a);
           b[1];
       ");

      object rez = script.Execute();

      Assert.Equal(2, rez);
    }

    [Fact]
    public void ISupportAssignTest()
    {
      RuntimeHost.AddType("Point", typeof(TestPoint));
      Script script = Script.Compile(@"
        mObj = [ Text -> 'Hello' , X -> 10, Y -> 150];
        point = new Point(20,300);  
        point:= mObj;
        point.X;");

      object rez = script.Execute();

      Assert.Equal(10, rez);
    }

    [Fact]
    public void ExpandoClassAndAnonimousFunction()
    {
      Script script = Script.Compile(
         @"
            stack = [
                
                 storage -> new Array(),

                 Push -> function (item)
                         {
                           body.storage.Add(item);
                         }
                 ];

            stack.Push(2);

            return stack.storage[0];
          "
         );

      RuntimeHost.AddType("Array", typeof(List<int>));

      object resultVal = script.Execute();
      Assert.Equal(2, resultVal);
    }

    [Fact]
    public void ExpandoClassAndAnonimousFunctionContract()
    {
      object resultVal = Script.RunCode(
         @"
            stack = [
                
                 storage -> new Array(),

                 Push -> function (item)
                         [
                          pre(item > 0);
                          post();
                          invariant();
                         ]
                         {
                           body.storage.Add(item);
                         }
                 ];

            stack.Push(2);

            return stack.storage[0];
          "
         );

      Assert.Equal(2, resultVal);
    }

    [Fact]
    public void ExpandoClassAndAnonimousFunctionGlobal()
    {
      Script result =
         Script.Compile(
         @"
            y = 0;
            stack = [
                
                 storage -> new Array(),

                 Test -> function (item)
                           global(y)
                         {
                           y = item;
                         }
                 ];

            stack.Test(2);
            return y;
          "
         );

      object resultVal = result.Execute();
      Assert.Equal(2, resultVal);
    }

    [Fact]
    public void ExpandoClassAndAnonimousFunctionContractException()
    {
      Assert.Throws<ScriptVerificationException>(() => 
         Script.RunCode(
         @"
            stack = [
                
                 storage -> new Array(),

                 Push -> function (item)
                         [
                          pre(item > 0);
                          post();
                          invariant();
                         ]
                         {
                           body.storage.Add(item);
                         }
                 ];

            stack.Push(-2);

            return stack.storage[0];
          "
         ));
    }

    [Fact]
    public void MemberRename()
    {
      RuntimeHost.AddType("TestPoint", typeof(TestPoint));

      Script result =
         Script.Compile(
         @"
            a = new TestPoint(3,2);
            b = new MemberRename(a, 'X','Z');
            b.Z;
          "
         );
      

      object resultVal = result.Execute();
      Assert.Equal(3, resultVal);
    }

    internal class TestPoint
    {
      public int X { get; set; }
      public int Y { get; set; }

      public TestPoint(int x, int y)
      {
        X = x;
        Y = y;
      }
    }
  }
}
