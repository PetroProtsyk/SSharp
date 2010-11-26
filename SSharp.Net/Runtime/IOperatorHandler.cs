using System;

namespace Scripting.SSharp.Runtime
{
  /// <summary>
  /// Handle operator execution. Objects implementing this 
  /// interface may provide custom implementation for standard 
  /// operators.
  /// 
  /// Operator handlers may be registered through RuntimeHost
  /// </summary>
  public interface IOperatorHandler
  {
    /// <summary>
    /// Process HandleOperatorArgs and yields the result of executing operator
    /// </summary>
    /// <param name="args">an instance of HandleOperatorArgs</param>
    /// <returns>result of handling operator</returns>
    object Process(HandleOperatorArgs args);
  }

  /// <summary>
  /// Is a class containing information for operator handlers
  /// </summary>
  public class HandleOperatorArgs : EventArgs
  {
    /// <summary>
    /// Operator symbol
    /// </summary>
    public string Symbol { get; private set; }
    /// <summary>
    /// Opertor Arguments
    /// </summary>
    public object[] Arguments { get; private set; }
    /// <summary>
    /// Result of operator execution
    /// </summary>
    public object Result { get; set; }
    /// <summary>
    /// Current execution context
    /// </summary>
    public IScriptContext Context { get; private set; }
    /// <summary>
    /// Flag which is used to cancel default behavior
    /// </summary>
    public bool Cancel { get; set; }

    public HandleOperatorArgs(IScriptContext context, string symbol, object[] arguments)
    {
      Symbol = symbol;
      Arguments = arguments;
      Context = context;
      Cancel = false;
    }

  }

}
