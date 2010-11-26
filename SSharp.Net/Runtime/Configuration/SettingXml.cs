using System.Xml.Serialization;

namespace Scripting.SSharp.Runtime.Configuration
{
  /// <summary>
  /// Represents single Setting Item node in script configuration
  /// </summary>
  public class SettingXml
  {
    /// <summary>
    /// Unique name of the item
    /// </summary>
    [XmlAttribute(ConfigSchema.Id)]
    public string Name { get; set; }

    /// <summary>
    /// String value
    /// </summary>
    [XmlAttribute(ConfigSchema.Value)]
    public string Value { get; set; }

    /// <summary>
    /// Type name of the converter
    /// </summary>
    [XmlAttribute(ConfigSchema.Converter)]
    public string Converter { get; set; }
  }
}
