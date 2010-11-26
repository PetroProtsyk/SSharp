using Scripting.SSharp.Runtime.Promotion;

namespace Scripting.SSharp.Runtime
{
  /// <summary>
  /// Expose dynamic members of an Instance to the script.
  /// This require using of DefaultObjectBinder class as default object binder.
  /// </summary>
  public interface IScriptable
  {
    /// <summary>
    /// Should return object wrapped by IScriptable or this
    /// </summary>
    [Promote(false)]
    object Instance { get; }

    /// <summary>
    /// Gets a binding to an instance's member (field, property)
    /// </summary>
    [Promote(false)]
    IMemberBinding GetMember(string name, params object[] arguments);

    /// <summary>
    /// Gets a binding to an instance's method
    /// </summary>
    [Promote(false)]
    IBinding GetMethod(string name, params object[] arguments);
  }
}
