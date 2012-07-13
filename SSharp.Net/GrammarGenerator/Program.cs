using System;
using System.Diagnostics;
using Scripting.SSharp;

namespace Debug
{  
  class Program
  {
    static void Main(string[] args)
    {
      if (args.Length == 0)
      {
        Console.WriteLine("S# parser data generator");
        Console.WriteLine("Please provide file name....");
        return;
      }

      System.IO.File.WriteAllText(args[0], Scripting.SSharp.Parser.FastParserGenerator.Build());
    }
  }
}
