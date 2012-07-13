using System;

namespace Scripting.SSharp.Runtime
{
  #region Exceptions
  /// <summary>
  /// Represents errors that occur during script execution. 
  /// </summary>
  public class ScriptException : Exception
  {
    public ScriptException(string Message)
      : base(Message)
    {
    }
  }

  /// <summary>
  /// Syntax error exception
  /// </summary>
  public class ScriptSyntaxErrorException : ScriptException
  {
      public ScriptSyntaxErrorException(string Message)
          : base(Message)
      {
      }
  }

  /// <summary>
  /// Exception being thrown when given id of variable, function, namespace, etc was not found
  /// </summary>
  public class ScriptIdNotFoundException : ScriptException
  {
    public ScriptIdNotFoundException(string Message)
      : base(Message)
    {
    }
  }

  /// <summary>
  /// Represents errors that occur when method was not found
  /// </summary>
  public class ScriptMethodNotFoundException : ScriptException
  {
    public ScriptMethodNotFoundException(string Message)
      : base(Message)
    {
    }
  }

  /// <summary>
  /// Represents errors that occur during run-time verification of the script. 
  /// </summary>
  public class ScriptVerificationException : ScriptException
  {
    public ScriptVerificationException(string Message)
      : base(Message)
    {
    }
  }

  /// <summary>
  /// Represents errors that occur during event processing. 
  /// </summary>
  public class ScriptEventException : ScriptException
  {
    public ScriptEventException(string Message)
      : base(Message)
    {
    }
  }
  #endregion
}
