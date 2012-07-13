using System.Xml.Serialization;

namespace Scripting.SSharp.Runtime.Configuration
{
  /// <summary>
  /// Represents single Operator node in script configuration
  /// </summary>
  public class OperatorDefinition
  {
    [XmlAttribute(ConfigSchema.Name)]
    public string Name { get; set; }
    [XmlAttribute(ConfigSchema.TypeAttribute)]
    public string Type { get; set; }

    public OperatorDefinition()
    {
    }
  }
}
