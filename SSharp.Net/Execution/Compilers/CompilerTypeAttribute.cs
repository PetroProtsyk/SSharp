using System;

namespace Scripting.SSharp.Execution.Compilers
{
  // Denis Vuyka: attribute usage can be extended to support structs and interfaces as well
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  internal class CompilerTypeAttribute : Attribute
  {
    public Type NodeType { get; private set; }

    public CompilerTypeAttribute(Type nodeType)
    {
      NodeType = nodeType;
    }

    public override bool Equals(object obj)
    {
      return ((obj == this) || (((obj != null) && (obj is CompilerTypeAttribute)) && (((CompilerTypeAttribute)obj).NodeType == this.NodeType)));
    }

    public override int GetHashCode()
    {
      return NodeType.GetHashCode();
    }
  }
}
