namespace Scripting.SSharp.Runtime.Operators
{
  /// <summary>
  /// Base interface for all Operators
  /// </summary>
  public interface IOperator
  {
    /// <summary>
    /// Operator symbol: +,-,/,++, etc.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Indicates unarity of the operator
    /// </summary>
    bool Unary { get; }

    /// <summary>
    /// should be used by unary operator
    /// </summary>
    /// <param name="value"></param>
    /// <returns>result or throws exception in case Unary=false</returns>
    object Evaluate(object value);

    /// <summary>
    /// should be used by unary operator
    /// </summary>
    /// <param name="value"></param>
    /// <returns>result or throws exception in case Unary=true</returns>
    object Evaluate(object left, object right);
  }
}
