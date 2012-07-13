using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Scripting.SSharp.Runtime.Promotion
{
  /// <summary>
  /// Base implementation of object binder
  /// </summary>
  public partial class ObjectBinding : IObjectBinding
  {
    #region Fields
    /// <summary>
    /// Strategies for converting parameters during method binding
    /// </summary>
    protected readonly List<ComposeParameterStrategy> parameterStrategies = new List<ComposeParameterStrategy>();
    /// <summary>
    /// Chain of getters
    /// </summary>
    protected readonly List<IGetter> Getters = new List<IGetter>();
    /// <summary>
    /// Chain of setters
    /// </summary>
    protected readonly List<ISetter> Setters = new List<ISetter>();

    /// <summary>
    /// Empty types constant (empty array of types).
    /// </summary>
    protected static readonly Type[] EmptyTypes = new Type[0];

    /// <summary>
    /// BindingFlags for constructor binding 
    /// </summary>
    protected internal static BindingFlags ConstructorFilter = BindingFlags.Public | BindingFlags.Instance;
    /// <summary>
    /// BindingFlags for method binding 
    /// </summary>
    protected internal static BindingFlags MethodFilter = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
    /// <summary>
    /// BindingFlags for property binding 
    /// </summary>
    protected internal static BindingFlags PropertyFilter = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
    /// <summary>
    /// BindingFlags for field binding 
    /// </summary>
    protected internal static BindingFlags FieldFilter = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
    /// <summary>
    /// BindingFlags for nested type binding 
    /// </summary>
    protected internal static BindingFlags NestedTypeFilter = BindingFlags.Public;

    /// <summary>
    /// Signals that no result is calculated
    /// </summary>
    protected static readonly object NoResult = new object();
    #endregion

    #region Constructor
    [Promote(false)]
    public ObjectBinding()
    {
      parameterStrategies.Add(new ComposeParameterStrategy(ComposeParametersExactPredicate, ComposeParametersExactConverter));
      parameterStrategies.Add(new ComposeParameterStrategy(ComposeParametersStrictPredicate, ComposeParametersStrictConverter));
      parameterStrategies.Add(new ComposeParameterStrategy(ComposeParametersWeekPredicate, ComposeParametersWeekConverter));

      IHandler property = new PropertyHandler(this);
      IHandler field = new FieldHandler(this);
      IHandler @event = new EventHandler(this);
      //IHandler mutant = new MutantHandler();
      IHandler sriptable = new ScriptableHandler(this);

      Getters.Add(property);
      Getters.Add(field);
      Getters.Add(@event);
      Getters.Add(new MethodGetter(this));
      Getters.Add(sriptable);
      //Getters.Add(mutant);
      Getters.Add(new NestedTypeGetter(this));
      //Getters.Add(new NameSpaceGetter());

      Setters.Add(property);
      Setters.Add(field);
      Setters.Add(@event);
      Setters.Add(sriptable);
      //Setters.Add(mutant);
    }
    #endregion

    #region IObjectBinder Members
    /// <summary>
    /// Binds to constructor
    /// </summary>
    /// <param name="targetType">Type</param>
    /// <param name="arguments">Arguments for constructor</param>
    /// <returns>ConstructorBind or null</returns>
    public IBinding BindToConstructor(Type targetType, object[] arguments)
    {
      //Use Default Constructor
      if (arguments == null || arguments.Length == 0)
      {
        ConstructorInfo defaultConstructor = targetType.GetConstructor(ConstructorFilter, null, EmptyTypes, null);

        if (defaultConstructor == null || !CanBind(defaultConstructor))
          return null;
        //return new ConstructorBind(target, null, null);

        return new ConstructorBinding(defaultConstructor, null);
      }

      IEnumerable<ConstructorInfo> constructors = targetType
        .GetConstructors(ConstructorFilter)
        .Where(c => CanBind(c));

      for (int i = 0; i < parameterStrategies.Count; i++)
      {
        ComposeParameterStrategy strategy = parameterStrategies[i];

        foreach (ConstructorInfo constructor in constructors)
        {
          ParameterInfo[] parameterInfos = constructor.GetParameters();
          object[] convertedArguments = ComposeParameters(arguments, parameterInfos, strategy.Predicate, strategy.Converter);

          if (convertedArguments != null)
          {
            return new ConstructorBinding(constructor, convertedArguments);
          }
        }
      }

      return null;
      //return new ConstructorBind(target, null, null);
    }

    /// <summary>
    /// Binds to method
    /// </summary>
    /// <param name="target">instance on an object</param>
    /// <param name="methodName">name of the method</param>
    /// <param name="genericParameters">for generic methods should specify types other vise empty array of type</param>
    /// <param name="arguments">arguments for method</param>
    /// <returns>MethodBind or null if binding is not possible</returns>
    public IBinding BindToMethod(object target, string methodName, Type[] genericParameters, object[] arguments)
    {
      return BindToMethod(target, mi => mi.Name == methodName && mi.GetParameters().Length == arguments.Length && CanBind(mi), genericParameters, arguments);
    }

    /// <summary>
    /// Binds to method
    /// </summary>
    /// <param name="target">instance on an object</param>
    /// <param name="method">specific method</param>
    /// <param name="arguments">arguments for method</param>
    /// <returns>MethodBind or null if binding is not possible</returns>
    public IBinding BindToMethod(object target, MethodInfo method, object[] arguments)
    {
      for (int i = 0; i < parameterStrategies.Count; i++)
      {
        ComposeParameterStrategy strategy = parameterStrategies[i];

        ParameterInfo[] parameterInfos = method.GetParameters();
        object[] convertedArguments = ComposeParameters(arguments, parameterInfos, strategy.Predicate, strategy.Converter);

        if (convertedArguments != null)
        {
          return new MethodBinding(method, target, convertedArguments);
        }
      }

      return null;
    }

    /// <summary>
    /// Binds to method of a given object including interface methods
    /// </summary>
    /// <param name="target">instance on an object</param>
    /// <param name="methodSelector">Predicate to select methods</param>
    /// <param name="genericParameters">for generic methods should specify types other vise empty array of type</param>
    /// <param name="arguments">arguments for method</param>
    /// <returns>MethodBind or null if binding is not possible</returns>
    protected IBinding BindToMethod(object target, Func<MethodInfo, bool> methodSelector, Type[] genericParameters, object[] arguments)
    {
      Type targetType = target as Type;
      bool processInterface = false;
      if (targetType == null)
      {
        targetType = target.GetType();
        processInterface = true;
      }

      IEnumerable<MethodInfo> methods = targetType
        .GetMethods(MethodFilter)
        .Where(methodSelector);

      //Process interfaces only if target is not a Type
      if (processInterface)
      {
        foreach (Type interfaceType in targetType.GetInterfaces())
        {
          methods = methods.Concat(interfaceType.GetMethods(MethodFilter).Where(methodSelector));
        }
      }

      for (int i = 0; i < parameterStrategies.Count; i++)
      {
        ComposeParameterStrategy strategy = parameterStrategies[i];

        foreach (MethodInfo method in methods)
        {
          MethodInfo actualMethod = method;
          if (method.IsGenericMethod)
          {
            actualMethod = method.MakeGenericMethod(genericParameters);
          }

          ParameterInfo[] parameterInfos = actualMethod.GetParameters();
          object[] convertedArguments = ComposeParameters(arguments, parameterInfos, strategy.Predicate, strategy.Converter);

          if (convertedArguments != null)
          {
            return new MethodBinding(actualMethod, target, convertedArguments);
          }
        }
      }

      return null;
    }

    /// <summary>
    /// Binds to index property
    /// </summary>
    /// <param name="target">instance on an object</param>
    /// <param name="arguments">arguments</param>
    /// <param name="setter">if true binds to setter, otherwise to getter</param>
    /// <returns>MethodBind or null if binding is not possible</returns>
    public IBinding BindToIndex(object target, object[] arguments, bool setter)
    {
      if (setter)
      {
        return BindToMethod(target, mi => (mi.Name == "set_Item" || mi.Name == "Set") && mi.GetParameters().Length == arguments.Length, null, arguments);
      }
      else
      {
        return BindToMethod(target, mi => (mi.Name == "get_Item" || mi.Name == "Get") && mi.GetParameters().Length == arguments.Length, null, arguments);
      }
    }

    /// <summary>
    /// Late binding to any member of a given object
    /// </summary>
    /// <param name="target">instance of a object</param>
    /// <param name="memberName">name of the method to bind</param>
    /// <param name="throwNotFound">if true will throw member not found exception at first attempt to invoke the binding</param>
    /// <returns>LateBoundMember</returns>
    public IMemberBinding BindToMember(object target, string memberName, bool throwNotFound)
    {
      return new DelayedMemberBinding(this, target, memberName, throwNotFound);
    }

    /// <summary>
    /// Gets value of the member
    /// </summary>
    /// <param name="name">member name</param>
    /// <param name="instance">instance</param>
    /// <param name="arguments">optional arguments</param>
    /// <returns>value</returns>
    protected internal object Get(string name, object instance, bool throwNotFound, params object[] arguments)
    {
      if (instance == null) throw new ArgumentNullException("instance");
      if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

      Type type = instance as Type;
      if (type == null) type = instance.GetType();

      foreach (IGetter getter in Getters)
      {
        object rez = getter.Get(name, instance, type, arguments);
        if (rez != NoResult)
          return rez;
      }

      if (throwNotFound)
        throw new ScriptIdNotFoundException("Member " + name + " not found");
      else
        return RuntimeHost.NullValue;
    }

    /// <summary>
    /// Sets the value to the member
    /// </summary>
    /// <param name="name">member name</param>
    /// <param name="instance">instance</param>
    /// <param name="value">value</param>
    /// <param name="throwNotFound">boolean</param>
    /// <param name="arguments">optional arguments</param>
    /// <returns>actual value. note that value may be transformed during setting</returns>
    protected internal object Set(string name, object instance, object value, bool throwNotFound, params object[] arguments)
    {
      if (instance == null) throw new ArgumentNullException("instance");
      if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

      Type type = instance as Type;
      if (type == null) type = instance.GetType();

      foreach (ISetter setter in Setters)
      {
        object rez = setter.Set(name, instance, type, value, arguments);
        if (rez != NoResult)
          return rez;
      }

      if (throwNotFound)
        throw new ScriptIdNotFoundException("Member " + name + " not found");
      else
        return RuntimeHost.NullValue;

    }

    /// <summary>
    /// Method used to convert value to target type. Should be used for any conversion during script execution
    /// </summary>
    /// <param name="value">value to convert or null</param>
    /// <param name="targetType">target type</param>
    /// <returns>Converted value or NoResult constant if conversion impossible</returns>
    public object ConvertTo(object value, Type targetType)
    {
      return ConvertToStatic(value, targetType);
    }

    /// <summary>
    /// Method used to convert value to target type. Should be used for any conversion during script execution.
    ///
    /// This method should be used by derived class for performance instead of ConvertTo
    /// </summary>
    /// <param name="value">value to convert or null</param>
    /// <param name="targetType">target type</param>
    /// <returns>Converted value or NoResult constant if conversion impossible</returns>
    protected static object ConvertToStatic(object value, Type targetType)
    {
      if (value == null) return value;
      if (targetType == typeof(object)) return value;

      Type valueType = value.GetType();

      //Interface
      if (targetType.IsInterface)
      {
        if (valueType
             .GetInterfaces()
             .Where(i => i.FullName == targetType.FullName)
             .Count() != 0)
          return value;
        //return new ExplicitInterface(value, targetType);
      }

      //Type Match
      if (targetType.IsAssignableFrom(valueType))
      {
        return value;
      }

      //Conversion operators
      MethodInfo mi = valueType.GetMethod("op_Implicit", BindingFlags.Static | BindingFlags.Public, null, new Type[] { valueType }, null);
      if (mi == null)
      {
        mi = valueType.GetMethod("op_Explicit", BindingFlags.Static | BindingFlags.Public, null, new Type[] { valueType }, null);
      }

      if (mi != null && mi.ReturnType == targetType)
      {
        return mi.Invoke(value, new object[] { value });
      }

      //NOTE: ref, out
      if (targetType.IsByRef)
      {
        if (targetType.GetElementType() == valueType)
          return value;
        else
          return ConvertToStatic(value, targetType.GetElementType());
      }

      //Convertible
      if (value is IConvertible)
      {
        return Convert.ChangeType(value, targetType, System.Globalization.CultureInfo.CurrentCulture);
      }

      return NoResult;
    }

    /// <summary>
    /// Checks binding conditions for given type member. Could be overriden in derived classes.
    /// 
    /// This implementation uses BindableAttribute for evaluating conditions
    /// </summary>
    /// <param name="member">Instance of MemberInfo</param>
    /// <returns>true if member could participate in binding</returns>
    public virtual bool CanBind(MemberInfo member)
    {
      PromoteAttribute bindable = member.GetCustomAttributes(typeof(PromoteAttribute), true).OfType<PromoteAttribute>().FirstOrDefault();
      if (bindable == null)
        bindable = member.DeclaringType.GetCustomAttributes(typeof(PromoteAttribute), true).OfType<PromoteAttribute>().FirstOrDefault();

      if (bindable == null) return true;
      return bindable.Promote;
    }
    #endregion

    #region ComposeParameterStrategy
    protected delegate bool CanConvertTypePredicate(object value, Type targetType);

    protected delegate object ConvertTypeMethod(object value, Type targetType);

    protected static object[] ComposeParameters(object[] arguments, ParameterInfo[] parameters, CanConvertTypePredicate predicate, ConvertTypeMethod converter)
    {
      if (arguments.Length != parameters.Length) return null;

      object[] result = new object[arguments.Length];

      for (int i = 0; i < parameters.Length; i++)
      {
        Type parameterType = parameters[i].ParameterType;
        object argument = arguments[i];

        //NOTE: ref, out
        //out and ref parameters handling
        ScopeValueReference vr = null;
        if (parameters[i].ParameterType.IsByRef)
        {
          vr = (ScopeValueReference)argument;
          argument = vr.Value;
        }

        if (predicate(argument, parameterType))
        {
          object converted = converter(argument, parameterType);
          if (converted == ObjectBinding.NoResult) return null;

          //NOTE: ref, out
          if (vr == null)
            result[i] = converted;
          else
          {
            vr.ConvertedValue = converted;
            result[i] = vr;
          }

        }
        else
        {
          return null;
        }

      }

      return result;
    }

    protected class ComposeParameterStrategy
    {
      public CanConvertTypePredicate Predicate { get; private set; }

      public ConvertTypeMethod Converter { get; private set; }

      public ComposeParameterStrategy(CanConvertTypePredicate predicate, ConvertTypeMethod converter)
      {
        Predicate = predicate;
        Converter = converter;
      }
    }

    #endregion

    #region Strategies
    protected static bool ComposeParametersExactPredicate(object value, Type targetType)
    {
      return value == null || value.GetType() == targetType;
    }

    protected static object ComposeParametersExactConverter(object value, Type targetType)
    {
      return value;
    }

    protected static bool ComposeParametersStrictPredicate(object value, Type targetType)
    {
      if (targetType == typeof(object)) return false;
      if (value == null) return true;

      Type vType = value.GetType();

      if (vType == targetType) return true;

      if (vType == typeof(Byte))
      {
        if (targetType == typeof(Int16)) return true;
        if (targetType == typeof(Int32)) return true;
        if (targetType == typeof(Int64)) return true;
        if (targetType == typeof(Double)) return true;
        if (targetType == typeof(Single)) return true;
      }

      if (vType == typeof(Int16))
      {
        if (targetType == typeof(Byte)) return true;
        if (targetType == typeof(Int32)) return true;
        if (targetType == typeof(Int64)) return true;
        if (targetType == typeof(Double)) return true;
        if (targetType == typeof(Single)) return true;
      }

      if (vType == typeof(Int32))
      {
        if (targetType == typeof(Byte)) return true;
        if (targetType == typeof(Int16)) return true;
        if (targetType == typeof(Int64)) return true;
        if (targetType == typeof(Double)) return true;
        if (targetType == typeof(Single)) return true;
      }

      if (vType == typeof(Int64))
      {
        if (targetType == typeof(Byte)) return true;
        if (targetType == typeof(Int16)) return true;
        if (targetType == typeof(Int32)) return true;
        if (targetType == typeof(Double)) return true;
        if (targetType == typeof(Single)) return true;
      }

      if (vType == typeof(Single))
      {
        if (targetType == typeof(Byte)) return true;
        if (targetType == typeof(Int16)) return true;
        if (targetType == typeof(Int32)) return true;
        if (targetType == typeof(Int64)) return true;
        if (targetType == typeof(Double)) return true;
      }

      if (vType == typeof(Double))
      {
        if (targetType == typeof(Byte)) return true;
        if (targetType == typeof(Int16)) return true;
        if (targetType == typeof(Int32)) return true;
        if (targetType == typeof(Int64)) return true;
        if (targetType == typeof(Single)) return true;
      }

      return vType.IsSubclassOf(targetType);
    }

    protected static object ComposeParametersStrictConverter(object value, Type targetType)
    {
      return ConvertToStatic(value, targetType);
    }

    protected static bool ComposeParametersWeekPredicate(object value, Type targetType)
    {
      return true;
    }

    protected static object ComposeParametersWeekConverter(object value, Type targetType)
    {
      return ConvertToStatic(value, targetType);
    }
    #endregion

    #region Private Interfaces
    /// <summary>
    /// Getter
    /// </summary>
    protected interface IGetter
    {
      object Get(string name, object instance, Type type, params object[] arguments);
    }

    /// <summary>
    /// Setter
    /// </summary>
    protected interface ISetter
    {
      object Set(string name, object instance, Type type, object value, params object[] arguments);
    }

    /// <summary>
    /// Getter and Setter
    /// </summary>
    protected interface IHandler : IGetter, ISetter
    {
    }
    #endregion
  }
}
