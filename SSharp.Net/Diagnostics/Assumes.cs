using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Scripting.SSharp.Diagnostics
{
  internal static class Assumes
  {
    // The exception that is thrown when an internal assumption failed.
#if !SILVERLIGHT || PocketPC
    [Serializable]
#endif
    [SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic")]
    private class InternalErrorException : Exception
    {
      public InternalErrorException(string message)
        : base(string.Format(CultureInfo.CurrentCulture, Strings.InternalExceptionMessage, message)) { }
    }

    [DebuggerStepThrough]
    internal static void IsTrue(bool condition)
    {
      if (!condition)
      {
        Fail(null);
      }
    }

    [DebuggerStepThrough]
    internal static void IsTrue(bool condition, string message)
    {
      if (!condition)
      {
        Fail(message);
      }
    }

    [DebuggerStepThrough]
    internal static void Fail(string message)
    {
      throw new InternalErrorException(message);
    }
  }
}
