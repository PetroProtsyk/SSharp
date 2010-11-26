using System.Collections.Generic;
using System.Linq;
using Scripting.SSharp.Parser.Ast;

namespace Scripting.SSharp.Parser.FastGrammar
{
  internal class SymbolTerminal : Terminal
  {
    #region Constructor
    private SymbolTerminal(string symbol)
      : base(symbol)
    {
      Symbol = symbol;
      Key = symbol.Trim();
      Priority = int.MinValue + symbol.Length;
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
      var tokenAst = TokenAst.Create(this, context, source.TokenStart, Symbol);

      return tokenAst;
    }
    
    private static readonly Dictionary<string, SymbolTerminal> Symbols = new Dictionary<string, SymbolTerminal>();

    public static SymbolTerminal GetSymbol(string symbol)
    {
      SymbolTerminal term;
      if (Symbols.TryGetValue(symbol, out term))
      {
        return term;
      }

      term = new SymbolTerminal(symbol);
      term.SetOption(TermOptions.IsGrammarSymbol, true);
      Symbols[symbol] = term;

      return term;
    }

    public static void ClearSymbols()
    {
      Symbols.Clear();
    }

    public static void RegisterPunctuation(params string[] symbols)
    {
      foreach (var term in symbols.Select(GetSymbol))
        term.SetOption(TermOptions.IsPunctuation);
    }

    public static void RegisterOperators(int precedence, params string[] opSymbols)
    {
      RegisterOperators(precedence, Associativity.Left, opSymbols);
    }

    public static void RegisterOperators(int precedence, Associativity associativity, params string[] opSymbols)
    {
      foreach (var opSymbol in opSymbols.Select(GetSymbol))
      {
        opSymbol.SetOption(TermOptions.IsOperator, true);
        opSymbol.Precedence = precedence;
        opSymbol.Associativity = associativity;
      }
    }

    #endregion
  }
}
