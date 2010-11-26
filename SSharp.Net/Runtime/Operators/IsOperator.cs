using System;

namespace Scripting.SSharp.Runtime.Operators
{
  /// <summary>
  /// Implementation of is operator
  /// </summary>
  public sealed class IsOperator : IOperator
  {
    #region IOperator Members

    public string Name
    {
      get { return "is"; }
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
      if (!(right is Type)) throw new ArgumentException("must be Type");

      var t1 = left.GetType();
      var t2 = (Type)right;

      return t1.IsSubclassOf(t2) || t1 == t2;
    }
    #endregion
  }
}
