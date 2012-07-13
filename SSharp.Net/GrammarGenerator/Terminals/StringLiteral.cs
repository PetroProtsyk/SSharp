using System;
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
