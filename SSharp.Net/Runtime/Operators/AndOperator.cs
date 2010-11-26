using System;

namespace Scripting.SSharp.Runtime.Operators
{
  /// <summary>
  /// Implementation of "and" operator.
  /// </summary>
  public sealed class AndOperator : IOperator
  {    
    public string Name
    {
      get { return "&"; }
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
      return DynamicMath.And(left, right);
    }
  }
}