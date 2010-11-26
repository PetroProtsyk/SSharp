using System;
using System.Collections.Generic;
using System.Diagnostics;
using Scripting.SSharp.Parser.Ast;

namespace Scripting.SSharp.Parser
{
  internal class LanguageCompiler
  {
    public LanguageCompiler(Grammar grammar)
    {
      GrammarDataBuilder builder = new GrammarDataBuilder(grammar);
      Data = builder.Build();
      Parser = new Parser(Data);
      Scanner = new Scanner(Data);
    }

    public readonly ParserData Data;
    public readonly Scanner Scanner;
    public readonly Parser Parser;

    public CompilerContext Context
    {
      get;
      private set;
    }

    public AstNode Parse(string source)
    {
      Context = new CompilerContext(this);
      Scanner.Prepare(Context, new SourceFile(source, "Source"));
      IEnumerable<TokenAst> tokenStream = Scanner.BeginScan();
      AstNode rootNode = Parser.Parse(Context, tokenStream);
      return rootNode;
    }
  }
}
