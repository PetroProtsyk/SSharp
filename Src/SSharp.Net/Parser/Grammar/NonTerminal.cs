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

using System;

namespace Scripting.SSharp.Parser.FastGrammar
{
  internal class NonTerminal : IGrammarTerm
  {
    #region Constructors
    public NonTerminal(string name, Type nodeType, string key, TermOptions options, int id)
    {
      Name = name;
      NodeType = nodeType;
      Key = key;
      Options = options;
      this.id = id;
    }
    #endregion

    internal int id;
    public Type NodeType { get; private set; }
    public string Key { get; private set; }
    private TermOptions Options { get; set; }

    public bool IsSet(TermOptions option)
    {
      return (Options & option) != 0;
    }

    public string Name
    {
      get;
      private set;
    }

    public string DisplayName
    {
      get;
      private set;
    }
  }
}
