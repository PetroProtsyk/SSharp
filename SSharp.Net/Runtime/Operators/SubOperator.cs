using System;

namespace Scripting.SSharp.Runtime.Operators
{
  /// <summary>
  /// Implementation of "sub" operator
  /// </summary>
  public sealed class SubOperator : IOperator
  {
    public string Name
    {
      get { return OperatorCodes.Sub; }
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
      return DynamicMath.Subtract(left, right);
    }
  }  
}