using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Runtime.Promotion;
using Scripting.SSharp;

namespace UnitTests
{
  public class Arithmetics : IDisposable
  {
    public Arithmetics()
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
    public void ArithmeticExpressions()
    {
      IScriptContext context = new ScriptContext();
      object result =
         Script.RunCode(
         @"
              a=1.0;
              b = 2.0; 
              c = 3.0; 
              d = 2.0;
              e = 18.0;
              f = 6.0;

              p = 2.0; u = 3.0; v = 1.0; r = 2.0; s = 5.0; t = 12.0;

              // r1 = 9
              r1 = a + b + c*d;

              // r2 = -2.5
              r2 = a*(b - c/d )- e/f;

              //r3 = -4.5
              r3 = a*b*((c - d )*a - p*(u - v)*(r + s))/t;

              //r4 = 65536
              r4 = 2 * d^(c*5);

              //r5 = 2
              r5 = 5 % 3;
           
              v1 = -3;
        ",
         context);

      Assert.Equal(9.0, context.GetItem("r1", true));
      Assert.Equal(-2.5, context.GetItem("r2", true));
      Assert.Equal(-4.5, context.GetItem("r3", true));
      Assert.Equal((double)65536, context.GetItem("r4", true));
      Assert.Equal((Int32)2, context.GetItem("r5", true));
      Assert.Equal(-3, context.GetItem("v1", true));
    }

    [Fact]
    public void NumberInputFormats()
    {
      IScriptContext context = new ScriptContext();
      object result =
         Script.RunCode(
         @"
           h = 0xAAFFAA;
           u = 3u;
           l = 31231231278l;
           ul = 23423234548ul;
           d = 3.2312d;
           f = 3424.123f; 
           m = 23123.25434543m;

           n1 = 4e+3;
           n2 = 6.32e-3;
         ",
         context);

      Assert.Equal(0xAAFFAA, context.GetItem("h", true));
      Assert.Equal(3u, context.GetItem("u", true));
      Assert.Equal(31231231278L, context.GetItem("l", true));
      Assert.Equal(23423234548ul, context.GetItem("ul", true));
      Assert.Equal(3424.123f, context.GetItem("f", true));
      Assert.Equal(3.2312d, context.GetItem("d", true));
      Assert.Equal(23123.25434543m, context.GetItem("m", true));

      Assert.Equal(4e+3, context.GetItem("n1", true));
      Assert.Equal(6.32e-3, context.GetItem("n2", true));
    }
  }
}
