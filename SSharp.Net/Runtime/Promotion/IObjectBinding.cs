using System;
using System.Reflection;

namespace Scripting.SSharp.Runtime.Promotion
{
  /// <summary>
  /// Public interface for Object Bind. It is used to bind arguments to:
  /// * indexers
  /// * constructors
  /// * methods
  /// * interfaces
  /// </summary>
  public interface IObjectBinding
  {
    /// <summary>
    /// Binds to constructor
    /// </summary>
    /// <param name="target"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    IBinding BindToConstructor(Type target, object[] arguments);

    /// <summary>
    /// Binds to method
    /// </summary>
    /// <param name="target"></param>
    /// <param name="methodName"></param>
    /// <param name="genericParameters"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    IBinding BindToMethod(object target, string methodName, Type[] genericParameters, object[] arguments);

    /// <summary>
    /// Binds to method
    /// </summary>
    /// <param name="target"></param>
    /// <param name="method"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    IBinding BindToMethod(object target, MethodInfo method, object[] arguments);

    /// <summary>
    /// Binds to Field, Property of Event of a given object
    /// </summary>
    /// <param name="target"></param>
    /// <param name="memberName"></param>
    /// <returns></returns>
    IMemberBinding BindToMember(object target, string memberName, bool throwNotFound);

    /// <summary>
    /// Binds to the indexer property
    /// </summary>
    /// <param name="target">target object</param>
    /// <param name="arguments">parameters</param>
    /// <param name="setter">if true binds to setter, elsewise binds to getter</param>
    /// <returns>IObjectBind or null</returns>
    IBinding BindToIndex(object target, object[] arguments, bool setter);

    //object Get(string name, object instance, bool throwNotFound, params object[] arguments);

    //object Set(string name, object instance, object value, bool throwNotFound, params object[] arguments);

    /// <summary>
    /// Converts value to target type
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <returns></returns>
    object ConvertTo(object value, Type targetType);

    /// <summary>
    /// Evaluates if object binder could run binding procedure for the given member.
    /// <b>Default</b> object binders uses BindableAttribute.
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    bool CanBind(MemberInfo member);
  }
}
