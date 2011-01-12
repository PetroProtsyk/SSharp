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
  internal class SymbolTerminal : Terminal
  {
    #region Constructor
    private SymbolTerminal(string symbol)
      : base(symbol)
    {
      Symbol = symbol;
      Key = symbol.Trim();
      base.Priority = int.MinValue + symbol.Length;
    }
    #endregion

    #region Properties
    public string Symbol
    {
      get;
      private set;
    }
    #endregion

    #region Methods
    public override TokenAst TryMatch(CompilerContext context, ISourceStream source)
    {
      if (!source.MatchSymbol(Symbol, false))
        return null;
    
      source.Position += Symbol.Length;
      TokenAst tokenAst = TokenAst.Create(this, context, source.TokenStart, Symbol);

      return tokenAst;
    }
    
    public override IList<string> GetFirsts()
    {
      return new string[] { Symbol };
    }

    private static Dictionary<string, SymbolTerminal> _symbols = new Dictionary<string, SymbolTerminal>();

    public static SymbolTerminal GetSymbol(string symbol)
    {
      SymbolTerminal term;
      if (_symbols.TryGetValue(symbol, out term))
      {
        return term;
      }

      term = new SymbolTerminal(symbol);
      term.SetOption(TermOptions.IsGrammarSymbol, true);
      _symbols[symbol] = term;

      return term;
    }

    public static void ClearSymbols()
    {
      _symbols.Clear();
    }

    public static void RegisterPunctuation(params string[] symbols)
    {
      foreach (string symbol in symbols)
      {
        SymbolTerminal term = SymbolTerminal.GetSymbol(symbol);
        term.SetOption(TermOptions.IsPunctuation);
      }
    }

    public static void RegisterOperators(int precedence, params string[] opSymbols)
    {
      RegisterOperators(precedence, Associativity.Left, opSymbols);
    }

    public static void RegisterOperators(int precedence, Associativity associativity, params string[] opSymbols)
    {
      foreach (string op in opSymbols)
      {
        SymbolTerminal opSymbol = SymbolTerminal.GetSymbol(op);
        opSymbol.SetOption(TermOptions.IsOperator, true);
        opSymbol.Precedence = precedence;
        opSymbol.Associativity = associativity;
      }
    }
    #endregion
  }
}
