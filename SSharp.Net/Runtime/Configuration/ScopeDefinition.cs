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
  }
}
