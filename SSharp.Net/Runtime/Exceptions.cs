/*
 * Copyright © 2011, Petro Protsyk, Denys Vuika
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *  http://www.apache.org/licenses/LICENSE-2.0
 *  
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
