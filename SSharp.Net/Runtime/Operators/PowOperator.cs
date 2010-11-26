using System;

namespace Scripting.SSharp.Runtime.Operators
{
  /// <summary>
  /// Implementation of power operator
  /// </summary>
  public sealed class PowOperator : IOperator
  {
    public string Name
    {
      get { return "^"; }
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
      return Math.Pow(Convert.ToDouble(left), Convert.ToDouble(right));
    }
  }
}