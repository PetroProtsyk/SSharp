using System;

namespace Scripting.SSharp.Runtime
{
  public class RuntimeException : Exception
  {
    public RuntimeException(string message) : base(message) { }
  }
}
