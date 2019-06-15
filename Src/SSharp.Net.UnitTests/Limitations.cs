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
  /// Summary description for Limitations
  /// </summary>
  public class LimitationsTests : IDisposable
  {
    public LimitationsTests()
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
    public void ParamsKeywordIsNotSupported()
    {
      Assert.Throws<ScriptMethodNotFoundException>(() =>
      {
          RuntimeHost.AddType("ParamsLimitation", typeof(ParamsLimitation));
          Assert.False(
            (bool)Script.RunCode("ParamsLimitation.Test(1,2,3,4);"));
      });
    }
  }

  internal class ParamsLimitation
  {

    public static bool Test(params int[] args)
    {
      return true;
    }

  }
}
