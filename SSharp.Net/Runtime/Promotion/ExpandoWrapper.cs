
namespace Scripting.SSharp.Runtime.Promotion
{
  /// <summary>
  /// Wraps object, allows to create new fields and access members form inner object
  /// Note: ExpandoWrapper does not support generic methods
  /// </summary>
  public class ExpandoWrapper : Expando
  {
    private readonly object _instance;

    public ExpandoWrapper(object instance)
    {
      _instance = instance;
    }

    [Promote(false)]
    public override object Instance
    {
      get
      {
        return _instance;
      }
    }

    [Promote(false)]
    public override IMemberBinding GetMember(string name, params object[] arguments)
    {
      IMemberBinding bind = RuntimeHost.Binder.BindToMember(_instance, name, true);
      if (bind != null)
        return bind;

      return base.GetMember(name, arguments);
    }

    [Promote(false)]
    public override IBinding GetMethod(string name, params object[] arguments)
    {
      IBinding bind = RuntimeHost.Binder.BindToMethod(_instance, name, null, arguments);
      if (bind != null)
        return bind;

      return base.GetMethod(name, arguments);
    }
  }
}
