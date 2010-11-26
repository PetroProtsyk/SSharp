using System;
using System.Collections.Generic;

namespace Scripting.SSharp.Parser
{
  internal abstract class GrammarTerm : IGrammarTerm
  {
    #region Constructors
    public GrammarTerm(string name)
      : this(name, name)
    {
    }

    public GrammarTerm(string name, string displayName)
    {
      Name = name;
      DisplayName = displayName;
      Key = Name + "\b"; 
    }

    public virtual void Init(Grammar grammar)
    {

    }
    #endregion

    #region Properties
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Key { get; protected set; }
    public TermOptions Options;
    public Type NodeType { get; protected set; }
    public bool Nullable;
    #endregion

    #region Methods
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

    #region Kleene operators: Q(), Plus(), Star()
    private NonTerminal _star;

    public GrammarExpression Q()
    {
      GrammarExpression q = Grammar.Empty | this;
      q.Name = this.Name + "?";
      return q;
    }

    public NonTerminal Star()
    {
      if (_star != null) return _star;
      string name = this.Name + "*";
      _star = new NonTerminal(name);
      _star.SetOption(TermOptions.IsList);
      _star.Rule = Grammar.Empty | _star + this;
      return _star;
    }
    #endregion

    #region Operators: +, |, implicit
    public static GrammarExpression operator +(GrammarTerm term1, GrammarTerm term2)
    {
      return Op_Plus(term1, term2);    
    }

    public static GrammarExpression operator +(GrammarTerm term1, string symbol2)
    {
      return Op_Plus(term1, SymbolTerminal.GetSymbol(symbol2));
    }
    
    public static GrammarExpression operator +(string symbol1, GrammarTerm term2)
    {
      return Op_Plus(SymbolTerminal.GetSymbol(symbol1), term2);
    }

    public static GrammarExpression operator |(GrammarTerm term1, GrammarTerm term2)
    {
      return Op_Pipe(term1, term2);
    }

    public static GrammarExpression operator |(GrammarTerm term1, string symbol2)
    {
      return Op_Pipe(term1, SymbolTerminal.GetSymbol(symbol2));
    }

    internal static GrammarExpression Op_Plus(GrammarTerm term1, GrammarTerm term2)
    {
      GrammarExpression expr1 = term1 as GrammarExpression;
      if (expr1 == null || expr1.Data.Count > 1) //either not expression at all, or Pipe-type expression (count > 1)
        expr1 = new GrammarExpression(term1);
      expr1.Data[expr1.Data.Count - 1].Add(term2);
      return expr1;
    }

    internal static GrammarExpression Op_Pipe(GrammarTerm term1, GrammarTerm term2)
    {
      GrammarExpression expr1 = term1 as GrammarExpression;
      if (expr1 == null) 
        expr1 = new GrammarExpression(term1);

      GrammarExpression expr2 = term2 as GrammarExpression;

      if (expr2 != null && expr2.Data.Count == 1)
      {
        expr1.Data.Add(expr2.Data[0]);
        return expr1;
      }
      expr1.Data.Add(new BnfTermList());
      expr1.Data[expr1.Data.Count - 1].Add(term2); 
      return expr1;
    }

    #endregion
  }
}

