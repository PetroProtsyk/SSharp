using System;

namespace Scripting.SSharp.Execution.Compilers
{
  // Denis Vuyka: attribute usage can be extended to support structs and interfaces as well
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  internal class CompilerTypeAttribute : Attribute
  {
    private Type _NodeType;
    public Type NodeType
    {
      get { return _NodeType; }
    }

    public CompilerTypeAttribute(Type nodeType)
    {
      _NodeType = nodeType;
    }

    public override bool Equals(object obj)
    {
      return ((obj == this) || (((obj != null) && (obj is CompilerTypeAttribute)) && (((CompilerTypeAttribute)obj)._NodeType == this._NodeType)));
    }

    public override int GetHashCode()
    {
      return _NodeType.GetHashCode();
    }
  }
}
