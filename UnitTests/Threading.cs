using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Runtime.Promotion;
using Scripting.SSharp;

namespace UnitTests
{
  /// <summary>
  /// Summary description for Threading
  /// </summary>
  [TestClass]
  public class ThreadingTests
  {
    static string code = @"i=1; while(i<3) {eval('1+1'); i=i+1;}; i=1; while(i<255) {i=i+1;}; return i;";

    static void execute(object code)
    {     
      string codestring = (string)code;
      object rez = Script.RunCode(codestring);

      Assert.AreEqual(255, rez);
      
      ThreadCounter.Dec();
    }

    static Thread StartThread()
    {      

      Thread th = new Thread(execute);
      th.Name = "Thread #" + ThreadCounter.Add();
      th.Start(code);

      return th;
    }

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
    public void TestThreading()
    {
      for (int i = 0; i < 15; i++)
      {
        StartThread();
      }

      ThreadCounter.Wait();
    }
  }

  public static class ThreadCounter
  {
    static object locker = new object();

    static volatile int counter = 0;

    public static int Add()
    {
      lock (locker)
      {
        counter++;
        return counter;
      }
    }

    public static void Dec()
    {
      lock(locker)
      {
        counter--;
      }
    }

    public static void Wait()
    {
      while (counter > 0)
      {
        Thread.Sleep(100);
      }
    }
  }
}
