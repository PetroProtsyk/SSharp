
namespace Scripting.SSharp.Runtime.Promotion
{
  /// <summary>
  /// Scriptable object that can be used to rename properties from the given instance
  /// </summary>
  public class MemberRename : IScriptable
  {
    private readonly IMemberBinding _oldMember = null;
    private string _original;
    private readonly string _newName;

    public MemberRename(object instance, string original, string newName)
    {
      Instance = instance;
      _oldMember = RuntimeHost.Binder.BindToMember(instance, original, true);
      if (_oldMember == null)
        throw new ScriptIdNotFoundException(original);

      _newName = newName;
      _original = original;
    }

    #region IScriptable
    [Promote(false)]
    public object Instance
    {
      get;
      private set;
    }

    [Promote(false)]
    public IMemberBinding GetMember(string name, params object[] arguments)
    {
      return name == _newName ? _oldMember : null;
    }

    [Promote(false)]
    public IBinding GetMethod(string name, params object[] arguments)
    {
      return null;
    }

    #endregion
  }
}
