namespace Scripting.SSharp.Runtime
{
  /// <summary>
  /// Objects implementing this interface may be called from the script
  /// just like usual functions.
  /// </summary>
  public interface IInvokable
  {
    /// <summary>
    /// Indicates wether Invoke could be called
    /// </summary>
    /// <returns>boolean value</returns>
    bool CanInvoke();

    /// <summary>
    /// Executes call to the object.
    /// </summary>
    /// <param name="context">Current execution context</param>
    /// <param name="args">Arguments or empty list. Prefer passing empty array instead of null.</param>
    /// <returns>execution result</returns>
    object Invoke(IScriptContext context, object[] args);
  }
}
