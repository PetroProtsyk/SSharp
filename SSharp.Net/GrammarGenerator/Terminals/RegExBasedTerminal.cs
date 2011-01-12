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

using System.Text.RegularExpressions;
using Scripting.SSharp.Parser.Ast;

namespace Scripting.SSharp.Parser
{
  internal abstract class RegexBasedTerminal : Terminal
  {
    public RegexBasedTerminal(string name, string pattern)
      : base(name)
    {
      Expression = new Regex(pattern);
    }

    public Regex Expression
    {
      get;
      private set;
    }

    public override TokenAst TryMatch(CompilerContext context, ISourceStream source)
    {
      Match result = Expression.Match(source.Text, source.Position);
      if (!result.Success)
        return null;
      source.Position += result.Length;
      
      return CreateToken(context, source);
    }

    protected virtual TokenAst CreateToken(CompilerContext context, ISourceStream source)
    {
      string lexeme = source.GetLexeme();
      TokenAst token = TokenAst.Create(this, context, source.TokenStart, lexeme, lexeme);
      return token;
    }
  }
}
