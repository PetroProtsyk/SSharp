using System;

namespace Scripting.SSharp.Runtime
{
  /// <summary>
  /// Indicates members and classes which could participate in binding procedure during script execution.
  /// </summary>
  [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
  public class PromoteAttribute : Attribute
  {
    #region Default values

    public static readonly PromoteAttribute Yes = new PromoteAttribute(true);
    public static readonly PromoteAttribute No = new PromoteAttribute(false);
    public static readonly PromoteAttribute Default = Yes;

    #endregion

    private readonly bool _Promote;
    /// <summary>
    /// If set to true the member will be processed by default object binder
    /// otherwise this member will be invisible from the script.
    /// </summary>
    public bool Promote
    {
      get { return _Promote; }
    }

    public PromoteAttribute() : this(true) { }

    public PromoteAttribute(bool promote)
    {
      _Promote = promote;
    }

#if SILVERLIGHT || PocketPC
    public bool IsDefaultAttribute()
    {
      return !this.Equals(Default);
    }
#else
    public override bool IsDefaultAttribute()
    {
      return !this.Equals(Default);
    }
#endif

    #region System.Object Implementation

    public override int GetHashCode()
    {
      return _Promote.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      return ((obj == this) || (((obj != null) && (obj is PromoteAttribute)) && (((PromoteAttribute)obj).Promote == _Promote)));
    }

    #endregion System.Object Implementation
  }
}
