using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Xunit;

using Scripting.SSharp.Runtime;
using Scripting.SSharp.Runtime.Promotion;
using Scripting.SSharp;
using Microsoft.CSharp.RuntimeBinder;

namespace UnitTests
{
  /// <summary>
  /// UnitTest operators,  OperatorsDispatching
  /// </summary>
  public class OperatorsDispatching : IDisposable
  {
    public OperatorsDispatching()
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
    public void BaseOperators_Plus()
    {
      object rez = null;

      rez = Script.RunCode("return 1+1;");
      Assert.Equal(2, rez);

      rez = Script.RunCode("return 1.2+1;");
      Assert.Equal(2.2, rez);

      rez = Script.RunCode("return '1'+1;");
      Assert.Equal("11", rez);

      rez = Script.RunCode("return 'Hello '+1+' Text';");
      Assert.Equal("Hello 1 Text", rez);
    }

    [Fact]
    public void BaseOperators_Minus()
    {
      object rez = null;

      rez = Script.RunCode("return 10-1;");
      Assert.Equal(9, rez);

      rez = Script.RunCode("return 1.2-1;");
      Assert.Equal((double)1.2 - 1, rez);
    }

    [Fact]
    public void BaseOperators_Minus1()
    {
      Assert.Throws<RuntimeBinderException>( () => Script.RunCode("return '1'-1;"));
    }

    [Fact]
    public void BaseOperators_Mul()
    {
      object rez = null;

      rez = Script.RunCode("return 10*12;");
      Assert.Equal(10*12, rez);

      rez = Script.RunCode("return 3.2*3;");
      Assert.Equal((double)3.2*3, rez);

      rez = Script.RunCode("return 3.5*21.5;");
      Assert.Equal((double)3.5 * 21.5, rez);
    }

    [Fact]
    public void BaseOperators_Div()
    {
      object rez = null;

      rez = Script.RunCode("return 6/2;");
      Assert.Equal(6 / 2, rez);

      rez = Script.RunCode("return 10/12;");
      Assert.Equal(10 / 12, rez);

      rez = Script.RunCode("return 45.43/12.3;");
      Assert.Equal((double)45.43 / 12.3, rez);

      rez = Script.RunCode("return 3.5/21;");
      Assert.Equal((double)3.5 / 21, rez);

      rez = Script.RunCode("return 3/21.2;");
      Assert.Equal(3 / (double)21.2, rez);
    }

    [Fact]
    public void BaseOperators_Mod()
    {
      object rez = null;

      rez = Script.RunCode("return 6%2;");
      Assert.Equal(6 % 2, rez);

      rez = Script.RunCode("return 10%12;");
      Assert.Equal(10 % 12, rez);

      rez = Script.RunCode("return 45.43%12.3;");
      Assert.Equal((double)45.43 % 12.3, rez);
    }

    [Fact]
    public void BaseOperators_Pow()
    {
      object rez = null;

      rez = Script.RunCode("return 6^2;");
      Assert.Equal(Math.Pow(6, 2), rez);

      rez = Script.RunCode("return 10^12;");
      Assert.Equal(Math.Pow(10, 12), rez);
    }
  }
}
