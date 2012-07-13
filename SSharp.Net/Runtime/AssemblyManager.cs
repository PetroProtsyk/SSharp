using System;
using Scripting.SSharp.Runtime.Configuration;

namespace Scripting.SSharp.Runtime
{
  public class AssemblyManager : BaseAssemblyManager, IAssemblyManager
  {
    #region Initialization
    [Promote(false)]
    public override void Initialize(ScriptConfiguration configuration)
    {
      base.Initialize(configuration);
    
      AppDomain.CurrentDomain.AssemblyLoad += CurrentDomainAssemblyLoad;
    }
    #endregion

    #region Overrides
    protected override void LoadAssemblies()
    {
      base.LoadAssemblies();

      WorkingAssemblies.Clear();
      WorkingAssemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies());
    }

    private void CurrentDomainAssemblyLoad(object sender, AssemblyLoadEventArgs args)
    {
      AddAssembly(args.LoadedAssembly);
    }
    #endregion

    #region IDisposable Members
    [Promote(false)]
    public override void Dispose()
    {
      AppDomain.CurrentDomain.AssemblyLoad -= CurrentDomainAssemblyLoad;
      base.Dispose();
    }

    #endregion
  }
}
