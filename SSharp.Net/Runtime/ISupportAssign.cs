namespace Scripting.SSharp.Runtime
{
  /// <summary>
  /// Objects implementing this interface will participate in strong assignment (:=) operator.
  /// </summary>
  public interface ISupportAssign
  {
    void AssignTo(object target);
  }
}
