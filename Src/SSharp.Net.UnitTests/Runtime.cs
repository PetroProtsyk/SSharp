using System;
using System.IO;
using System.Reflection;
using Xunit;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Runtime.Configuration;
using Scripting.SSharp.Runtime.Promotion;
using Scripting.SSharp;

namespace UnitTests
{
  /// <summary>
  /// Summary description for Runtime
  /// </summary>
  public class Runtime : IDisposable
  {
    public void Dispose()
    {
      RuntimeHost.CleanUp();
      EventBroker.ClearAllSubscriptions();
    }

    public static Stream TestConfig
    {
      get
      {
        Stream configStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SSharp.Net.UnitTests.TestConfig.xml");
        configStream.Seek(0, SeekOrigin.Begin);
        return configStream;
      }
    }

    // Ensures runtime host does not crash if Initialize is called multiple times
    [Fact]
    public void ShouldAllowMultipleInitializes()
    {
      RuntimeHost.Initialize();
      RuntimeHost.Initialize();
      RuntimeHost.Initialize(TestConfig);
      RuntimeHost.Initialize(new ScriptConfiguration());
    }

    [Fact]
    public void ShouldChangeInitializationState()
    {
      Assert.False(RuntimeHost.IsInitialized);
      RuntimeHost.Initialize();
      Assert.True(RuntimeHost.IsInitialized);
      RuntimeHost.CleanUp();
      Assert.False(RuntimeHost.IsInitialized);
    }

    [Fact]
    public void DefaultSettings()
    {
      RuntimeHost.Initialize();
      Assert.False( RuntimeHost.GetSettingsItem<bool>("UnsubscribeAllEvents"));
      RuntimeHost.CleanUp();
    }

    [Fact]
    public void BaseAssemblyLoaderOnlyScansAssembliesFromConfigFile()
    {
      RuntimeHost.AssemblyManager = new BaseAssemblyManager();
      RuntimeHost.AssemblyManager.BeforeAddAssembly +=
        (s, e) => {
          if (e.Assembly.FullName != "System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")
            throw new Exception();
        };
      RuntimeHost.Initialize(TestConfig);

      RuntimeHost.CleanUp();
    }

    [Fact]
    public void BaseAssemblyAddingRemovingAssembly()
    {
      RuntimeHost.AssemblyManager = new BaseAssemblyManager();
      RuntimeHost.Initialize(TestConfig);

      Assert.False(RuntimeHost.AssemblyManager.HasNamespace("UnitTests"));
      RuntimeHost.AssemblyManager.AddAssembly(typeof(Runtime).Assembly);
      Assert.True(RuntimeHost.AssemblyManager.HasNamespace("UnitTests"));
      RuntimeHost.AssemblyManager.RemoveAssembly(typeof(Runtime).Assembly);
      Assert.False(RuntimeHost.AssemblyManager.HasNamespace("UnitTests"));

      RuntimeHost.CleanUp();
    }

    [Fact]
    public void AssemblyManagerFiltersTypes()
    {
      RuntimeHost.InitializingTypes += RuntimeHostInitializingTypes;
      RuntimeHost.Initialize(TestConfig);

      Assert.True(RuntimeHost.AssemblyManager.HasNamespace("UnitTests"));
      Assert.False(RuntimeHost.AssemblyManager.HasNamespace("System.Text"));
              
      RuntimeHost.CleanUp();
    }

    void RuntimeHostInitializingTypes(object sender, EventArgs e)
    {
      RuntimeHost.AssemblyManager.BeforeAddAssembly += AssemblyManagerBeforeAddAssembly;
    }

    void AssemblyManagerBeforeAddAssembly(object sender, AssemblyHandlerEventArgs e)
    {
      e.Cancel = e.Assembly != typeof(Runtime).Assembly;
    }

    [Fact]
    public void DisableBindingToStringMethod()
    {
      RuntimeHost.Binder = new MyBinder();
      RuntimeHost.Initialize(TestConfig);

      Script script = Script.Compile(@"
            b = 'a';
            b.ToString();
       ");
      Assert.Throws<ScriptMethodNotFoundException>(() => script.Execute());
    }

    private class MyBinder : ObjectBinding
    {
      public override bool CanBind(MemberInfo member)
      {
        if (member.Name == "ToString") return false;

        return base.CanBind(member);
      }
    }

    [Fact]
    public void CustomOperatorHandling()
    {
      RuntimeHost.Initialize(TestConfig);
      RuntimeHost.RegisterOperatorHandler("$", new DollarHandler());

      Script script = Script.Compile(@"
              return $'1234';
       ");
      object rez = script.Execute();

      Assert.Equal(1234, rez);
    }

    private class DollarHandler : IOperatorHandler
    {
      #region IOperatorHandler Members

      public object Process(HandleOperatorArgs args)
      {
        if (args.Arguments != null && args.Arguments.Length == 1)
        {
          string value = (string)args.Arguments[0];
          args.Cancel = true;
          return int.Parse(value);
        }

        throw new NotSupportedException();
      }

      #endregion
    }
  }
}
