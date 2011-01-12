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
