using System;
using System.Collections.Generic;
using Scripting.SSharp.Runtime.Promotion;

namespace Scripting.SSharp.Runtime.Operators
{
  public abstract class BinaryOperator : IOperator
  {
    /// <summary>
    /// Generic evaluator
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    private delegate object EvaluatorGeneric(object left, object right);

    /// <summary>
    /// Strongly typed evaluator
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public delegate object Evaluator<T1, T2>(T1 left, T2 right);

    private Dictionary<Type, Dictionary<Type, EvaluatorGeneric>> operators = new Dictionary<Type, Dictionary<Type, EvaluatorGeneric>>();

    public BinaryOperator(string name)
    {
      Name = name;
    }

    #region IOperator Members

    public virtual string Name
    {
      get;
      private set;
    }

    protected static Dictionary<string, string> NameOperatorMapping = new Dictionary<string, string>()
    {
      {"+", "op_Addition"},
      {"-", "op_Subtraction"},
      {"*", "op_Multiply"},
      {"/", "op_Division"},
      {"%", "op_Modulus"}
    };

    public virtual object Evaluate(object left, object right)
    {
      EvaluatorGeneric function;
      Dictionary<Type, EvaluatorGeneric> rezCache;

      Type leftType = left.GetType();
      Type rightType = right.GetType();

      if (!(operators.TryGetValue(leftType, out rezCache) &&
            rezCache.TryGetValue(rightType, out function)))
      {
        string opName = NameOperatorMapping[Name];

        IBinding bind = RuntimeHost.Binder.BindToMethod(left, opName, null, new object[] { left, right });
        if (bind == null)
          bind = RuntimeHost.Binder.BindToMethod(right, opName, null, new object[] { left, right });

        if (bind == null)
          throw new NotSupportedException("Operator does not support given arguments");

        EvaluatorGeneric evaluatorGeneric = (x, y) =>
        {
          return bind.Invoke(null, new object[] { x, y });
        };
        
        RegisterEvaluatorGeneric(leftType, rightType, evaluatorGeneric);

        function = evaluatorGeneric;
      }

      //try
      //{
      //  function = operators[left.GetType()][right.GetType()];
      //}
      //catch (KeyNotFoundException)
      //{
      //  throw new NotSupportedException("Operator does not support given arguments");
      //}

      return function(left, right);
    }

    private void RegisterEvaluatorGeneric(Type left, Type right, EvaluatorGeneric eval)
    {
      if (operators.ContainsKey(left))
      {
        operators[left].Add(right, eval);
      }
      else
      {
        Dictionary<Type, EvaluatorGeneric> op = new Dictionary<Type, EvaluatorGeneric>();
        op.Add(right, eval);

        operators.Add(left, op);
      }
    }

    /// <summary>
    /// Register the evaluater strongly typed version.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="eval"></param>
    protected void RegisterEvaluator<T1, T2>(Evaluator<T1, T2> evaluator)
    {
      EvaluatorGeneric evaluatorGeneric = (x, y) =>
      {
        return evaluator((T1)x, (T2)y);
      };

      RegisterEvaluatorGeneric(typeof(T1), typeof(T2), evaluatorGeneric);
    }

    #endregion

    #region Unary functions
    public bool Unary
    {
      get { return false; }
    }

    public object Evaluate(object value)
    {
      throw new NotImplementedException();
    }
    #endregion
  }
}
