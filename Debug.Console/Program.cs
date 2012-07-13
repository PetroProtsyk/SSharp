using System;
using System.Diagnostics;
using Scripting.SSharp;
using Scripting.SSharp.Execution;
using Scripting.SSharp.Execution.Compilers.Dom;
using Scripting.SSharp.Execution.VM;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Runtime.Promotion;

namespace Debug
{  
  class Program
  {
    static void Main(string[] args)
    {
      RuntimeHost.Initialize();
                  
      string code = @"
          g_variable = 100; 
          g_string = ""hello"";
          MyFunction(); 
          Console.WriteLine(""global variable: "" + g_variable); 
          function MyFunction() 
          { 
              g_variable = 200;               
global:g_string += "" world!"";

              Console.WriteLine(""global variable within function: "" + global:g_variable); 
              Console.WriteLine(""local variable: "" + g_variable); 
 Console.WriteLine(g_string);
          }; ";      
      Script.RunCode(code);


      Console.WriteLine("Press a key to exit");
      Console.ReadKey();
    }
  }
}
