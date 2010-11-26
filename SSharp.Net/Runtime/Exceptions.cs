using System;

namespace Scripting.SSharp.Runtime
{
  #region Exceptions
  /// <summary>
  /// Represents errors that occur during script execution. 
  /// </summary>
  public abstract class ScriptException : Exception
  {
    public ScriptException(string message)
      : base(message)
    {
    }

    public ScriptException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }

  /// <summary>
  /// Represents errors that occur in runtime engine or due to its invalid behavior
  /// </summary>
  public class ScriptRuntimeException : ScriptException
  {
    public ScriptRuntimeException(string message)
      : base(message)
    {
    }

    public ScriptRuntimeException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }

  /// <summary>
  /// Represents errors that occur during script execution
  /// </summary>
  public class ScriptExecutionException : ScriptException
  {
    public ScriptExecutionException(string message)
      : base(message)
    {
    }

    public ScriptExecutionException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }

  /// <summary>
  /// Syntax error exception
  /// </summary>
  public class ScriptSyntaxErrorException : ScriptException
  {
      public ScriptSyntaxErrorException(string message)
          : base(message)
      {
      }
  }

  /// <summary>
  /// Exception being thrown when given id of variable, function, namespace, etc was not found
  /// </summary>
  public class ScriptIdNotFoundException : ScriptRuntimeException
  {
    public ScriptIdNotFoundException(string message)
      : base(message)
    {
    }
  }

  /// <summary>
  /// Represents errors that occur when method was not found
  /// </summary>
  public class ScriptMethodNotFoundException : ScriptRuntimeException
  {
    public ScriptMethodNotFoundException(string message)
      : base(message)
    {
    }
  }

  /// <summary>
  /// Represents errors that occur during run-time verification of the script. 
  /// </summary>
  public class ScriptVerificationException : ScriptExecutionException
  {
    public ScriptVerificationException(string message)
      : base(message)
    {
    }
  }

  /// <summary>
  /// Represents errors that occur during event processing. 
  /// </summary>
  public class ScriptEventException : ScriptRuntimeException
  {
    public ScriptEventException(string message)
      : base(message)
    {
    }
  }
  #endregion
}
