using System;
using System.Reflection;

namespace Scripting.SSharp.Runtime.Promotion
{
  /// <summary>
  /// Is a result of binding to Property, Field or Event of an object
  /// </summary>
  public interface IMemberBinding : IBinding
  {
    /// <summary>
    /// Object to which this member belongs
    /// </summary>
    object Target { get; }

    Type TargetType { get; }

    MemberInfo Member { get; }

    /// <summary>
    /// Sets value to the member
    /// </summary>
    /// <param name="value"></param>
    void SetValue(object value);

    /// <summary>
    /// Returns the value of a member
    /// </summary>
    /// <returns></returns>
    object GetValue();

    void AddHandler(object value);

    void RemoveHandler(object value);
  }
}
