using System.Xml.Serialization;

namespace Scripting.SSharp.Runtime.Configuration
{
  /// <summary>
  /// Represents single Scope node in script configuration
  /// </summary>
  public class ScopeDefinition
  {
    /// <summary>
    /// Type of the scope: 0 - default, 1 - function, 2 - using, 3 - event.
    /// </summary>
    [XmlAttribute(ConfigSchema.Id)]
    public int Id { get; set; }
    /// <summary>
    /// Fully qualified name of the scope activator.
    /// </summary>
    [XmlAttribute(ConfigSchema.ActivatorAttribute)]
    public string Type { get; set; }

    /// <summary>
    /// Default constructor
    /// </summary>
    public ScopeDefinition()
    {
    }
  }
}
