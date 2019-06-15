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
  /// <summary>
  /// Indicates members and classes which could participate in binding procedure during script execution.
  /// </summary>
  [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
  public sealed class PromoteAttribute : Attribute
  {
    #region Default values

    public static readonly PromoteAttribute Yes = new PromoteAttribute(true);
    public static readonly PromoteAttribute No = new PromoteAttribute(false);
    public static readonly PromoteAttribute Default = Yes;

    #endregion

    private readonly bool _promote;
    /// <summary>
    /// If set to true the member will be processed by default object binder
    /// otherwise this member will be invisible from the script.
    /// </summary>
    public bool Promote
    {
      get { return _promote; }
    }

    public PromoteAttribute() : this(true) { }

    public PromoteAttribute(bool promote)
    {
      _promote = promote;
    }

#if SILVERLIGHT || PocketPC
    public bool IsDefaultAttribute()
    {
      return !this.Equals(Default);
    }
#else
    public override bool IsDefaultAttribute()
    {
      return !Equals(Default);
    }
#endif

    #region System.Object Implementation

    public override int GetHashCode()
    {
      return _promote.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      return ((obj == this) || (((obj != null) && (obj is PromoteAttribute)) && (((PromoteAttribute)obj).Promote == _promote)));
    }

    #endregion System.Object Implementation
  }
}
