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
using System.Globalization;

namespace Scripting.SSharp.Diagnostics
{
  internal static class Requires
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

      var type = value as Type;
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
