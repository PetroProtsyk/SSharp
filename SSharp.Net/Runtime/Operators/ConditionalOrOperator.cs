using System;

namespace Scripting.SSharp.Runtime.Operators
{
  /// <summary>
  /// Implementation of "conditional or" operator
  /// </summary>
  public sealed class ConditionalOrOperator : IOperator
  {
    public string Name
    {
      get { return "||"; }
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
      return DynamicMath.ConditionalOr(left, right);
    }
  }
}