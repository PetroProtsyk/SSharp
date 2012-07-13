
namespace Scripting.SSharp.Runtime.Promotion
{
  /// <summary>
  /// Scriptable object that can be used to rename properties from the given instance
  /// </summary>
  public class MemberRename : IScriptable
  {
    IMemberBinding oldMember = null;
    string original;
    string newName;

    public MemberRename(object instance, string original, string newName)
    {
      Instance = instance;
      oldMember = RuntimeHost.Binder.BindToMember(instance, original, true);
      if (oldMember == null)
        throw new ScriptIdNotFoundException(original);

      this.newName = newName;
      this.original = original;
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
      if (name == newName) return oldMember;
      return null;
    }

    [Promote(false)]
    public IBinding GetMethod(string name, params object[] arguments)
    {
      return null;
    }

    #endregion
  }
}
