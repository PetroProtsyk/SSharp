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
  internal class StringLiteral : Terminal
  {
    #region Constructor
    public StringLiteral()
      : base("string")
    {
    }
    #endregion

    #region Fields
    private static List<string> firsts = new List<string>() { "'", "\"", "@" };
    #endregion

    #region Init
    public override void Init(Grammar grammar)
    {
      base.Init(grammar);
    }

    public override IList<string> GetFirsts()
    {
      return firsts;
    }

    public override TokenAst TryMatch(CompilerContext context, ISourceStream source)
    {
      bool isVerbatim = false;
      int start = source.Position;

      if (source.CurrentChar == '@')
      {
        isVerbatim = true;
        source.Position++;
        start++;
      }

      if (IsCurrentQuote(source))
      {
        source.Position++;
        start++;
      }
      else 
        return null;

      while (!source.EOF())
      {
        if (!isVerbatim)
        {
          if (source.CurrentChar == '\\')
          {
            //TODO: Escape processing
            source.Position += 2;
            continue;
          }
          else
            //Single line string ends incorrectly
            if (ParserData.LineTerminators.IndexOf(source.CurrentChar) >= 0)
              return null;
        }

        if (IsCurrentQuote(source)) break;
        
        source.Position++;
      }

      if (IsCurrentQuote(source))
        source.Position++;
      else
        return null;
      
      string lexeme = source.GetLexeme();
      string body = source.Text.Substring(start, source.Position - start - 1);
      //TODO: handle this in escape processing
      if (!isVerbatim)
        body = body.Replace("\\'", "'").Replace("\\\"", "\"").Replace("\\\\", "\\");

      TokenAst token = TokenAst.Create(this, context, source.TokenStart, lexeme, body);     
      return token;

      //return Grammar.CreateSyntaxErrorToken(context, source.TokenStart, "Failed to convert the value");
    }

    private bool IsCurrentQuote(ISourceStream source)
    {
      return source.CurrentChar == '\'' || source.CurrentChar == '"';
    }
    #endregion
  }
}
