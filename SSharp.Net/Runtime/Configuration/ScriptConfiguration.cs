using System.Collections.Generic;
using System.Xml.Serialization;

namespace Scripting.SSharp.Runtime.Configuration
{
  /// <summary>
  /// Xml Serializable configuration data class
  /// </summary>  
  [XmlRoot(ConfigSchema.Configuration)]
  public class ScriptConfiguration
  {
    /// <summary>
    /// References section in config xml
    /// </summary>
    [XmlArray(ConfigSchema.References)]
    [XmlArrayItem(ConfigSchema.Assembly)]
    public List<Reference> References { get; set; }

    /// <summary>
    /// Types section in config xml
    /// </summary>
    [XmlArray(ConfigSchema.Types)]
    [XmlArrayItem(ConfigSchema.Type)]
    public List<TypeXml> Types { get; set; }

    /// <summary>
    /// Scopes section in config xml
    /// </summary>
    [XmlArray(ConfigSchema.Scopes)]
    [XmlArrayItem(ConfigSchema.Scope)]
    public List<ScopeDefinition> Scopes { get; set; }

    /// <summary>
    /// Settings section in config xml
    /// </summary>
    [XmlArray(ConfigSchema.Settings)]
    [XmlArrayItem(ConfigSchema.Item)]
    public List<SettingXml> SettingXml { get; set; }

    /// <summary>
    /// Operators section in config xml
    /// </summary>
    [XmlArray(ConfigSchema.Operators)]
    [XmlArrayItem(ConfigSchema.Operator)]
    public List<OperatorDefinition> Operators { get; set; }

    /// <summary>
    /// Initialization script
    /// </summary>
    [XmlElement(ConfigSchema.Initialization)]
    [XmlText]
    public string Initialization { get; set; }

    /// <summary>
    /// Creates empty configuration
    /// </summary>
    public ScriptConfiguration()
    {
      References = new List<Reference>();
      Scopes = new List<ScopeDefinition>();
      Types = new List<TypeXml>();
      SettingXml = new List<SettingXml>();
      Operators = new List<OperatorDefinition>();
      Initialization = string.Empty;
    }
  }
}
