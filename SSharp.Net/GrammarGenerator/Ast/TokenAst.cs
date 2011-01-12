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

namespace Scripting.SSharp.Parser.Ast
{
  internal class TokenAst : AstNode
  {
    #region Constructor
    protected TokenAst(AstNodeArgs args)
      : base(args)
    {
    }
    #endregion

    #region Properties
    public ITerminal Terminal
    {
      get { return Term as ITerminal; }
    }

    public string Text
    {
      get;
      set;
    }

    public object Value
    {
      get;
      set;
    }

    public bool IsError()
    {
      return Terminal.Category == TokenCategory.Error;
    }

    public int Length
    {
      get { return Text == null ? 0 : Text.Length; }
    }

    public bool IsKeyword
    {
      get;
      set;
    }

    public bool MatchByValue
    {
      get
      {
        if (IsKeyword) return true;
        if (Text == null) return false;
        return (Terminal.MatchMode & TokenMatchMode.ByValue) != 0;
      }
    }

    public bool MatchByType
    {
      get
      {
        if (IsKeyword) return false;
        return (Terminal.MatchMode & TokenMatchMode.ByType) != 0;
      }
    }
    #endregion

    #region Methods
    public static TokenAst Create(ITerminal term, CompilerContext context, SourceLocation location, string text)
    {
      return Create(term, context, location, text, text);
    }

    public static TokenAst Create(ITerminal term, CompilerContext context, SourceLocation location, string text, object value)
    {
      int textLen = text == null ? 0 : text.Length;
      var span = new SourceSpan(location, textLen);
      var args = new AstNodeArgs(term, span, null);
      var token = new TokenAst(args) { Text = text, Value = value };
      return token;
    }
    #endregion
  }
}
