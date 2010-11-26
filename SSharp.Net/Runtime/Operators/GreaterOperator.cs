using System;

namespace Scripting.SSharp.Runtime.Operators
{
  /// <summary>
  /// Implementation of "greater" operator
  /// </summary>
  public sealed class GreaterOperator : IOperator
  {    
    public string Name
    {
      get { return OperatorCodes.Gr; }
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
      return DynamicMath.Greater(left, right);
    }
  }
}
