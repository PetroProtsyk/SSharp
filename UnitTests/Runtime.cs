using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Runtime.Promotion;
using Scripting.SSharp;

namespace UnitTests
{
  /// <summary>
  /// Summary description for Runtime
  /// </summary>
  [TestClass]
  public class Runtime
  {
    [TestCleanup]
    public void TearDown()
    {
      RuntimeHost.CleanUp();
      EventBroker.ClearAllEvents();
    }

    public static Stream TestConfig
    {
      get
      {
        Stream configStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Scripting.SSharp.UnitTests.TestConfig.xml");
        configStream.Seek(0, SeekOrigin.Begin);
        return configStream;
      }
    }

    [TestMethod]
    public void DefaultSettings()
    {
      RuntimeHost.Initialize();
      Assert.IsFalse( RuntimeHost.GetSettingsItem<bool>("UnsubscribeAllEvents"));
      RuntimeHost.CleanUp();
    }

    [TestMethod]
    public void BaseAssemblyLoaderOnlyScansAssembliesFromConfigFile()
    {
      RuntimeHost.AssemblyManager = new BaseAssemblyManager();
      RuntimeHost.AssemblyManager.BeforeAddAssembly +=
        (s, e) => {
          if (!e.Assembly.FullName.StartsWith("System.Data,"))
            throw new Exception();
        };
      RuntimeHost.Initialize(TestConfig);

      RuntimeHost.CleanUp();
    }

    [TestMethod]
    public void BaseAssemblyAddingRemovingAssembly()
    {
      RuntimeHost.AssemblyManager = new BaseAssemblyManager();
      RuntimeHost.Initialize(TestConfig);

      Assert.IsFalse(RuntimeHost.AssemblyManager.HasNamespace("UnitTests"));
      RuntimeHost.AssemblyManager.AddAssembly(typeof(Runtime).Assembly);
      Assert.IsTrue(RuntimeHost.AssemblyManager.HasNamespace("UnitTests"));
      RuntimeHost.AssemblyManager.RemoveAssembly(typeof(Runtime).Assembly);
      Assert.IsFalse(RuntimeHost.AssemblyManager.HasNamespace("UnitTests"));

      RuntimeHost.CleanUp();
    }

    [TestMethod]
    public void AssemblyManagerFiltersTypes()
    {
      RuntimeHost.InitializingTypes += RuntimeHostInitializingTypes;
      RuntimeHost.Initialize(TestConfig);

      Assert.IsTrue(RuntimeHost.AssemblyManager.HasNamespace("UnitTests"));
      Assert.IsFalse(RuntimeHost.AssemblyManager.HasNamespace("System.Text"));
              
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

    [TestMethod]
    [ExpectedException(typeof(ScriptMethodNotFoundException))]
    public void DisableBindingToStringMethod()
    {
      RuntimeHost.Binder = new MyBinder();
      RuntimeHost.Initialize(TestConfig);

      Script script = Script.Compile(@"
            b = 'a';
            b.ToString();
       ");
      script.Execute();
    }

    private class MyBinder : ObjectBinding
    {
      public override bool CanBind(MemberInfo member)
      {
        if (member.Name == "ToString") return false;

        return base.CanBind(member);
      }
    }

    [TestMethod]
    public void CustomOperatorHandling()
    {
      RuntimeHost.Initialize(TestConfig);
      RuntimeHost.RegisterOperatorHandler("$", new DollarHandler());

      Script script = Script.Compile(@"
              return $'1234';
       ");
      object rez = script.Execute();

      Assert.AreEqual(1234, rez);
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
