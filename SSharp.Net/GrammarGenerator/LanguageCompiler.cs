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

using Scripting.SSharp.Parser.Ast;

namespace Scripting.SSharp.Parser
{
  internal class LanguageCompiler
  {
    public LanguageCompiler(Grammar grammar)
    {
      var builder = new GrammarDataBuilder(grammar);
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
      var tokenStream = Scanner.BeginScan();
      var rootNode = Parser.Parse(Context, tokenStream);
      return rootNode;
    }
  }
}
