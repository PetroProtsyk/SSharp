using System;

namespace Scripting.SSharp.Runtime.Operators
{
  /// <summary>
  /// Implementation of multiplication operator
  /// </summary>
  public class MulOperator : IOperator
  {
    public string Name
    {
      get { return OperatorCodes.Mul; }
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
      return DynamicMath.Multiply(left, right);
    }
  }
}
