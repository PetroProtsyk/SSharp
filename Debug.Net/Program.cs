using Scripting.SSharp;
using Scripting.SSharp.Runtime;

namespace Debug.Net
{
  class Program
  {
    static void Main(string[] args)
    {
      RuntimeHost.Initialize();
      var result = Script.RunCode(@"
         Console.WriteLine('Hello World! This is S#!');
      ");
    }
  }
}
