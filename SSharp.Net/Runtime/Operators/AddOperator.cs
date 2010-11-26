using System;

namespace Scripting.SSharp.Runtime.Operators
{
  /// <summary>
  /// Implementation of "add" operator.
  /// </summary>
  public sealed class AddOperator : IOperator
  {
    public string Name
    {
      get { return OperatorCodes.Add; }
    }

    public bool Unary
    {
      get { return false; }
    }

    public object Evaluate(object value)
    {
      throw new NotImplementedException();
    }

    public object Evaluate(object left, object right)
    {
      return DynamicMath.Add(left, right);
    }
  }
}
