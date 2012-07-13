using System;
using System.Diagnostics;
using System.Globalization;

namespace Scripting.SSharp.Diagnostics
{
  public static class Requires
  {
    [DebuggerStepThrough]
    public static void NotNull<T>(T value, string parameterName) where T : class
    {
      if (value == null) throw new ArgumentNullException(parameterName);
    }

    [DebuggerStepThrough]
    public static void NotNullOrEmpty(string value, string parameterName)
    {
      NotNull(value, parameterName);

      if (value.Length == 0)
        throw new ArgumentException(
          string.Format(CultureInfo.CurrentCulture, Strings.ArgumentException_EmptyString, parameterName), parameterName);
    }

    [DebuggerStepThrough]
    public static void OfType<T>(object value, string parameterName) where T : class
    {
      NotNull(value, parameterName);

      Type type = value as Type;
      if (type != null)
      {
        if (!typeof(T).IsAssignableFrom(type))
          throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.ArgumentException_Type, parameterName), parameterName);
      }
      else if (!(value is T))
        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.ArgumentException_Type, parameterName), parameterName);
    }
  }
}
