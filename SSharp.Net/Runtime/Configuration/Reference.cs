using System.Reflection;
using System.Xml.Serialization;

namespace Scripting.SSharp.Runtime.Configuration
{
  /// <summary>
  /// Represents single Reference node in script configuration
  /// </summary>
  public sealed class Reference
  {
    [XmlAttribute(ConfigSchema.Name)]
    public string Name { get; set; }
    [XmlAttribute(ConfigSchema.IsStrongNamed)]
    public bool StrongNamed { get; set; }

    public Reference()
    {
    }

    public Reference(string name, bool sn)
    {
      Name = name;
      StrongNamed = sn;
    }

    /// <summary>
    /// Loads assembly to current application domain
    /// </summary>
    /// <returns></returns>
    public Assembly Load()
    {
      if (StrongNamed) return Assembly.Load(Name);

      return Assembly.LoadFrom(Name);
    }

    public override string ToString()
    {
      return Name;
    }
  }
}
