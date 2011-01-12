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

namespace Scripting.SSharp.Runtime.Configuration
{
  #region Schema
  /// <summary>
  /// Internal configuration schema
  /// </summary>
  internal static class ConfigSchema
  {
    public const string Configuration = "Configuration";
    public const string References = "References";
    public const string Assembly = "Assembly";
    public const string Initialization = "Initialization";
    public const string Settings = "Settings";
    public const string Operators = "Operators";
    public const string Scopes = "Scopes";

    public const string Item = "Item";
    public const string Types = "Types";
    public const string Type = "Type";
    public const string Scope = "Scope";
    public const string Operator = "Operator";

    public const string Alias = "alias";
    public const string Name = "name";
    public const string Id = "id";
    public const string Value = "value";
    public const string Converter = "converter";
    public const string TypeAttribute = "type";
    public const string ActivatorAttribute = "type";
    public const string ScopeFactoryAttribute = "ScopeFactory";

    public const string IsStrongNamed = "sn";
  }
  #endregion
}
