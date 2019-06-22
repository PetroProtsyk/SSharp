using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Scripting.SSharp.Runtime;
using Scripting.SSharp;
using System.Diagnostics;

namespace Debug.TestExecute
{
  class Program
  {
    static int Main(string[] args)
    {
      Console.WriteLine("S# Test Execute");
      if (args == null || args.Length ==0 || !Directory.Exists(args[0]))
      {
        TypeLine("Please specify valid path containing test files", ConsoleColor.Red);
        return 1;
      }

      TypeLine("Initializing script engine", ConsoleColor.Yellow);

      Stopwatch totalTime = Stopwatch.StartNew();


      Stopwatch sw = new Stopwatch();
      sw.Reset(); sw.Start();

      RuntimeHost.Initialize();

      TypeLine("Elapsed time:" + sw.Elapsed, ConsoleColor.White);
      TypeLine("Pass 1: Parsing files", ConsoleColor.Yellow);

      DirectoryInfo dir = new DirectoryInfo(args[0]);

      int parsingErr = 0, execErr = 0, total = 0;

      foreach (var file in dir.GetFiles())
      {
        sw.Reset(); sw.Start();
        try
        {
          Script s = Script.Compile(File.ReadAllText(file.FullName));
          TypeLine("Parsing:" + file.Name + " success elapsed time:" + sw.ElapsedMilliseconds, ConsoleColor.Gray);
        }
        catch (ScriptSyntaxErrorException)
        {
          parsingErr++;
          TypeLine("Parsing:" + file.Name + " failed elapsed time:" + sw.ElapsedMilliseconds, ConsoleColor.Red);
        }        
      }

      TypeLine("Pass 2: Executing", ConsoleColor.Yellow);

      foreach (var file in dir.GetFiles())
      {
        sw.Reset(); sw.Start();
        total++;
        try
        {
          Script s = Script.Compile(File.ReadAllText(file.FullName));
          s.Context.SetItem("Console", typeof(Console));
          s.Context.SetItem("Test", new Test());
          object rez = s.Execute();
          //TypeLine("Execute:" + file.Name + " success, result [" + (rez == null ? "Null" : rez.ToString()) + "] elapsed time:" + sw.ElapsedMilliseconds, ConsoleColor.Gray);
          TypeLine(file.Name + ";success;" + sw.Elapsed, ConsoleColor.Gray);
        }
        catch (Exception e)
        {
          execErr++;
          //TypeLine("Execute:" + file.Name + " failed, exception [" + e.Message + "] elapsed time:" + sw.ElapsedMilliseconds, ConsoleColor.Red);
          TypeLine(file.Name + ";fail;" + e.Message, ConsoleColor.DarkRed);
        }
      }

      if (execErr + parsingErr == 0)
      {
        TypeLine("Done, all " + total + " tests passed, time " + totalTime.Elapsed, ConsoleColor.Green);
        return 0;
      }
      else
      {
        TypeLine(string.Format("Done tests {2} in {3} ms with errors: parsing {0}, execution {1}", parsingErr, execErr, total, totalTime.ElapsedMilliseconds), ConsoleColor.Red);
        return 1;
      }
    }

    public static void Type(string text, ConsoleColor c)
    {
      ConsoleColor old = Console.ForegroundColor;
      Console.ForegroundColor = c;
      Console.Write(text);
      Console.ForegroundColor = old;
    }

    public static void TypeLine(string text, ConsoleColor c)
    {
      ConsoleColor old = Console.ForegroundColor;
      Console.ForegroundColor = c;
      Console.WriteLine(text);
      Console.ForegroundColor = old;
    }

  }
}
