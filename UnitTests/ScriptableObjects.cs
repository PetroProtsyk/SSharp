using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Runtime.Promotion;
using Scripting.SSharp;

namespace UnitTests
{
  /// <summary>
  /// Tests MutantFramework
  /// </summary>
  [TestClass]
  public class ScriptableObjects
  {
    public ScriptableObjects()
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
    public void WrapperHasAccessToObjectProperties()
    {
      Script script = Script.Compile(@"
           a = [1,2,3];
           a.Length;

           b = new ExpandoWrapper(a);
           b.Length;
       ");

      object rez = script.Execute();

      Assert.AreEqual(3, rez);
    }

    [TestMethod]
    public void WrapperHasAccessToObjectPropertiesIndexer()
    {
      Script script = Script.Compile(@"
           a = [1,2,3];
           a.Length;

           b = new ExpandoWrapper(a);
           b[1];
       ");

      object rez = script.Execute();

      Assert.AreEqual(2, rez);
    }

    [TestMethod]
    public void ISupportAssignTest()
    {
      RuntimeHost.AddType("Point", typeof(TestPoint));
      Script script = Script.Compile(@"
        mObj = [ Text -> 'Hello' , X -> 10, Y -> 150];
        point = new Point(20,300);  
        point:= mObj;
        point.X;");

      object rez = script.Execute();

      Assert.AreEqual(10, rez);
    }

    [TestMethod]
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
      Assert.AreEqual(2, resultVal);
    }

    [TestMethod]
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

      Assert.AreEqual(2, resultVal);
    }

    [TestMethod]
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
      Assert.AreEqual(2, resultVal);
    }

    [TestMethod]
    [ExpectedException(typeof(ScriptVerificationException))]
    public void ExpandoClassAndAnonimousFunctionContractException()
    {
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
         );
    }

    [TestMethod]
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
      Assert.AreEqual(3, resultVal);
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
