
namespace Scripting.SSharp.Runtime
{
  /// <summary>
  /// Represents activator for event scope
  /// </summary>
  public class LocalScopeActivator : IScopeActivator
  {
    #region IScopeActivator Members

    public IScriptScope Create(IScriptScope parent, params object[] args)
    {
      return new LocalScope(parent);
    }

    #endregion
  }
}
