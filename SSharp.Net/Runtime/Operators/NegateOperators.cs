using System;

namespace Scripting.SSharp.Runtime.Operators
{
  /// <summary>
  /// Implementation of negate operator
  /// </summary>
  public class NegateOperator1 : UnaryOperator
  {
    public NegateOperator1()
    {
      Name = "-";
    }

    public override object Evaluate(object value)
    {
      if (value == null) throw new NullReferenceException("Cannot negate null value");
      Type type = value.GetType();

      if (type == typeof(Boolean))
        return !(Boolean)value;
      if (type == typeof(Int16))
        return -(Int16)value;
      if (type == typeof(Int32))
        return -(Int32)value;
      if (type == typeof(Int64))
        return -(Int64)value;
      if (type == typeof(Double))
        return -(Double)value;
      if (type == typeof(float))
        return -(float)value;

      throw new Exception("Cannot negate value of type: " + type.Name);
    }
  }

  /// <summary>
  /// Implementation of negate operator
  /// </summary>
  public class NegateOperator2 : NegateOperator1
  {
    public NegateOperator2()
    {
      Name = "~";
    }
  }

  /// <summary>
  /// Implementation of negate operator
  /// </summary>
  public class NegateOperator3 : NegateOperator1
  {
    public NegateOperator3()
    {
      Name = "!";
    }
  }
}
