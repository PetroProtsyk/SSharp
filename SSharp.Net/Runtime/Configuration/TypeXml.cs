using System.Xml.Serialization;

namespace Scripting.SSharp.Runtime.Configuration
{
  /// <summary>
  /// Represents single Type node in script configuration
  /// </summary>
  public class TypeXml
  {
    /// <summary>
    /// Friendly name of the type
    /// </summary>
    [XmlAttribute(ConfigSchema.Alias)]
    public string Alias { get; set; }

    /// <summary>
    /// Fully qualified name of the type
    /// </summary>
    [XmlAttribute(ConfigSchema.Name)]
    public string QualifiedName { get; set; }

    public TypeXml()
    {
    }
  }
}
