
namespace Scripting.SSharp.Runtime
{
  /// <summary>
  /// Creates instances of Scopes
  /// </summary>
  public interface IScopeFactory
  {
    void RegisterType(int id, IScopeActivator instance);

    void RegisterType(ScopeTypes id, IScopeActivator instance);

    IScriptScope Create();

    IScriptScope Create(ScopeTypes id);

    IScriptScope Create(ScopeTypes id, IScriptScope parent);

    IScriptScope Create(ScopeTypes id, params object[] args);

    IScriptScope Create(ScopeTypes id, IScriptScope parent, params object[] args);

    IScriptScope Create(int id, params object[] args);

    IScriptScope Create(int id, IScriptScope parent, params object[] args);

  }


  public enum ScopeTypes
  {
    Default = 0,

    Function = 1,

    Using = 2,

    Event = 3,

    Reserved = 4,

    Local = 5
  }
}
