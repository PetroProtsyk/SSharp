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

namespace Scripting.SSharp.Parser
{
  internal class NonTerminal : GrammarTerm
  {
    #region Constructors
    public NonTerminal(string name)
      : base(name)
    {
      id = counter++;
    }

    internal int id;
    private static int counter;

    public NonTerminal(string name, Type nodeType)
      : this(name)
    {
      base.NodeType = nodeType;
    }

    public NonTerminal(string name, Type nodeType, string key, TermOptions Options, int id)
      : this(name)
    {
      base.NodeType = nodeType;
      base.Key = key;
      base.Options = Options;
      this.id = id;
    }
    #endregion

    #region Properties
    public GrammarExpression Rule
    {
      get;
      set;
    }

    public readonly ProductionList Productions = new ProductionList();
    public readonly StringSet Firsts = new StringSet();
    public readonly NonTerminalList PropagateFirstsTo = new NonTerminalList();
    #endregion
  }
}
