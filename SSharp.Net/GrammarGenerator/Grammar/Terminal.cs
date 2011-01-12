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

using System.Collections.Generic;
using Scripting.SSharp.Parser.Ast;

namespace Scripting.SSharp.Parser
{
  internal class Terminal : GrammarTerm, ITerminal
  {
    #region Constructor
    public Terminal(string name)
      : base(name)
    {
      Nullable = false;
      Priority = 0;
      Precedence = int.MaxValue;
      Category = TokenCategory.Content;
      MatchMode = TokenMatchMode.ByValueThenByType;
      Associativity = Associativity.Neutral;
      this.NodeType = typeof(TokenAst);
    }

    public Terminal(string name, TokenCategory category)
      : this(name)
    {
      Category = category;
    }
    #endregion

    #region Fields
    public TokenMatchMode MatchMode { get; protected set; }
    public TokenCategory Category { get; protected set; }
    public int Precedence {get; protected set;}
    public Associativity Associativity  { get; protected set; }
    public int Priority { get; protected set; }
    #endregion

    #region Methods
    public virtual TokenAst TryMatch(CompilerContext context, ISourceStream source)
    {
      return null;
    }

    public virtual IList<string> GetFirsts()
    {
      return null;
    }
    #endregion

    #region Comparison Methods
    public static int ByName(ITerminal x, ITerminal y)
    {
      return string.Compare(x.Name, y.Name);
    }

    public static int ByPriorityReverse(ITerminal x, ITerminal y)
    {
      return y.Priority - x.Priority;
    }
    #endregion
  }
}
