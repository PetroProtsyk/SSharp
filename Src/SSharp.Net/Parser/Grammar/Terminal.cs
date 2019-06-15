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
using Scripting.SSharp.Parser.Ast;

namespace Scripting.SSharp.Parser.FastGrammar
{
  internal class Terminal : ITerminal
  {
    #region Constructor
    public Terminal(string name)
    {
      Name = name;
      Key = Name + "\b"; 
      Priority = 0;
      Category = TokenCategory.Content;
      DisplayName = name;
      MatchMode = TokenMatchMode.ByValueThenByType;
      Associativity = Associativity.Neutral;
      Precedence = int.MaxValue;
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
    public int Precedence { get; protected set; }
    public Associativity Associativity { get; protected set; }
    public int Priority { get; protected set; }
    #endregion

    #region Methods
    public virtual TokenAst TryMatch(CompilerContext context, ISourceStream source)
    {
      return null;
    }
    #endregion

    #region IGrammarTerm Members

    public string Name
    {
      get;
      protected set;
    }

    public string Key
    {
      get;
      protected set;
    }

    public string DisplayName
    {
      get;
      protected set;
    }

    public Type NodeType
    {
      get { return typeof(TokenAst); }
    }

    protected TermOptions Options { get; set; }

    public bool IsSet(TermOptions option)
    {
      return (Options & option) != 0;
    }

    public void SetOption(TermOptions option)
    {
      SetOption(option, true);
    }

    public void SetOption(TermOptions option, bool value)
    {
      if (value)
        Options |= option;
      else
        Options &= ~option;
    }
    #endregion
  }
}
