using System;
using System.Diagnostics;
using System.IO;
using Scripting.SSharp;
using Scripting.SSharp.Runtime;

namespace Debug.Net
{
  public class Person
  {
    [Promote]
    public void TestMe()
    {
    }

    private string _Name;
    public string Name
    {
      get { return _Name; }
      set { _Name = value; }
    }

    public string this[string name]
    {
      get { return name + "______"; }
      set { this.Name = name; }
    }
  }

  class Program
  {
    static void Main(string[] args)
    {
      RuntimeHost.Initialize();
      RuntimeHost.AddType<Person>("Person");

      object c = Script.RunCode(@"
        namespace A{
          b = 2;
          b = b + 1;

          function c(){
            return A_b;
          }
        }

        return A_c();");

      object o = Script.RunCode(@"
 
N = 100;
l = new List<|int|>(N);
for (i = 0; i < N; i++) l.Add(i);

function calc(x){
  return 1;
}

for (j = 0; j < 5; j++){

 sw = Stopwatch.StartNew();
// s2 = l.AsParallel<|int|>().Sum<|int|>(calc);
// Console.WriteLine('Time for parallel:' +sw.ElapsedMilliseconds+' result:'+s2);

 sw = Stopwatch.StartNew();
 //s1 = l.Sum<|int|>(calc);
 //Console.WriteLine('Time for 1 thread:' + sw.ElapsedMilliseconds+' result:'+s1);

}

al = ArrayList.Synchronized(new ArrayList());
l.AsParallel<|int|>().ForAll<|int|>(function(l1){ al.Add( l1*l1 ); });

");

      //string test_01 = "for (i = 0; i <= 100000; i++) { }";
      //string test_02 = "for (i = 0; i <= 100000; i = i + 1) { }";
      string test_03 = "for (i = 0; i <= 100000; i++) {  i.ToString(); }";      
      //string test_04 = "a = [1,2,3]; x = 0; for (i = 0; i < 100000; i++) { x = a[1]; }";      
      //string test_05 = "a = [1,2,3]; for (i = 0; i < 100000; i++) { a[0] = i; }";
      //string test_06 = "x = null; p = new Person(); for (i = 0; i < 100000; i++) { x = p[\"testme\"]; }";
      //string test_07 = "x = null; p = new Person(); for (i = 0; i < 100000; i++) { p[\"name\"] = \"test\"; }";
      //string test_08 = "for (i = 0; i <= 100000; i++) { i.ToString(null); }";
      //string test_09 = "p = new Person(); for (i = 0; i <= 100000; i++) { p.Name = \"Denis\"; }";
      //string test_10 = "p = new CustomObject(); for (i = 0; i <= 100000; i++) { p.Name = \"Denis\"; }";
      TestCode("Performance_40_03__.txt", test_03, 50);

      //string code = "p = new dynamic(); p.Name = \"Denis Vuyka\"; p.Age = 28; Console.WriteLine(p.Name); Console.WriteLine(p.Age);";
      //string code = "Console.WriteLine(1 == 1);";
      //Script.RunCode(code);
            
      Console.WriteLine("Press a key to exit");
      Console.ReadKey();
    }

    private static void TestCode(string outputFile, string code, int numberOfTimes)
    {
      if (File.Exists(outputFile)) File.Delete(outputFile);
      File.AppendAllText(outputFile, code + Environment.NewLine);
      Stopwatch watch = new Stopwatch();
      for (int i = 0; i < numberOfTimes; i++)
      {
        watch.Start(); ;
        Script.RunCode(code);
        watch.Stop();
        Console.WriteLine((i + 1) + ": Elapsed (ms): " + watch.ElapsedMilliseconds);
        File.AppendAllText(outputFile, watch.ElapsedMilliseconds + Environment.NewLine);
        watch.Reset();
      }
    }
  }
}
