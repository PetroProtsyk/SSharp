using System;
using Scripting.SSharp.Runtime.Promotion;

namespace Scripting.SSharp.Runtime.Operators
{
  /// <summary>
  /// Implementation of and operator
  /// </summary>
  public class AndOperator : BinaryOperator
  {
    public AndOperator() :
      base("&")
    {
      RegisterEvaluator<Boolean, Boolean>((x, y) => x & y);
    }
  }

  /// <summary>
  /// Implementation of exclusive and operator
  /// </summary>
  public class And2Operator : BinaryOperator
  {
    public And2Operator() :
      base("&&")
    {
      RegisterEvaluator<Boolean, Boolean>((x, y) => x && y);
    }
  }

  /// <summary>
  /// Implementation of or operator
  /// </summary>
  public class OrOperator : BinaryOperator
  {
    public OrOperator() :
      base("|")
    {
      RegisterEvaluator<Boolean, Boolean>((x, y) => x | y);
    }
  }

  /// <summary>
  /// Implementation of exclusive or operator
  /// </summary>
  public class Or2Operator : BinaryOperator
  {
    public Or2Operator() :
      base("||")
    {
      RegisterEvaluator<Boolean, Boolean>((x, y) => x || y);
    }
  }

  /// <summary>
  /// Implementation of equals operator
  /// </summary>
  public class EqualsOperator : IOperator
  {
    public EqualsOperator()
    {
    }

    #region IOperator Members

    public string Name
    {
      get { return "=="; }
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
      if (left == null || right == null)
        return object.Equals(left, right);

      IBinding equalityMethod = null;

      equalityMethod = RuntimeHost.Binder.BindToMethod(left, "op_Equality", null, new object[] { left, right });
      if (equalityMethod != null)
        return equalityMethod.Invoke(null, null);

      equalityMethod = RuntimeHost.Binder.BindToMethod(right, "op_Equality", null, new object[] { right, left });
      if (equalityMethod != null)
        return equalityMethod.Invoke(null, null);

      equalityMethod = RuntimeHost.Binder.BindToMethod(left, "Equals", null, new object[] { right });
      if (equalityMethod != null)
        return equalityMethod.Invoke(null, null);
      
      return object.Equals(left, right);
    }

    #endregion
  }

  /// <summary>
  /// Implementation of not equals operator
  /// </summary>
  public class NotEqualsOperator : IOperator
  {
    EqualsOperator equalsOperator = new EqualsOperator();

    public NotEqualsOperator()
    {
    }

    #region IOperator Members

    public string Name
    {
      get { return "!="; }
    }

    public bool Unary
    {
      get { return false; }
    }

    public object Evaluate(object value)
    {
      return equalsOperator.Evaluate(value);
    }

    public object Evaluate(object left, object right)
    {
      return !(bool)equalsOperator.Evaluate(left, right);
    }

    #endregion
  }
}
