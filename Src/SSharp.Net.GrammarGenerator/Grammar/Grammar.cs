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
  /// <summary>
  /// This class represents S# grammar
  /// </summary>
  internal sealed class Grammar
  {
    #region Constructor
    private Grammar() { }
    #endregion

    #region Properties
    public readonly TerminalList NonGrammarTerminals = new TerminalList();
    public NonTerminal Root  { get; set;}
    #endregion

    #region Methods
    public static TokenAst CreateSyntaxErrorToken(CompilerContext context, SourceLocation location, string message, params object[] args)
    {
      if (args != null && args.Length > 0)
        message = string.Format(message, args);
      return TokenAst.Create(Grammar.SyntaxError, context, location, message);
    }

    private void CreateGrammar(bool expressionGrammar)
    {
      #region 1. Terminals
      NumberLiteral n = new NumberLiteral();
      IdentifierTerminal v = new IdentifierTerminal();
      Terminal s = new StringLiteral();

      Terminal @ref = SymbolTerminal.GetSymbol("ref");
      Terminal @out = SymbolTerminal.GetSymbol("out");

      Terminal @is = SymbolTerminal.GetSymbol("is");
      Terminal dot = SymbolTerminal.GetSymbol(".");
      Terminal less = SymbolTerminal.GetSymbol("<");
      Terminal greater = SymbolTerminal.GetSymbol(">");
      Terminal arrow = SymbolTerminal.GetSymbol("->");
      Terminal LSb = SymbolTerminal.GetSymbol("[");
      Terminal RSb = SymbolTerminal.GetSymbol("]");
      Terminal LCb = SymbolTerminal.GetSymbol("(");
      Terminal RCb = SymbolTerminal.GetSymbol(")");
      Terminal RFb = SymbolTerminal.GetSymbol("}");
      Terminal LFb = SymbolTerminal.GetSymbol("{");
      Terminal LMb = SymbolTerminal.GetSymbol("<!");
      Terminal RMb = SymbolTerminal.GetSymbol("!>");
      Terminal LGb = SymbolTerminal.GetSymbol("<|");
      Terminal RGb = SymbolTerminal.GetSymbol("|>");
      Terminal comma = SymbolTerminal.GetSymbol(",");
      Terminal semicolon = SymbolTerminal.GetSymbol(";");
      Terminal colon = SymbolTerminal.GetSymbol(":");

      ITerminal Comment = new CommentTerminal("Comment", "/*", "*/");
      NonGrammarTerminals.Add(Comment);
      ITerminal LineComment = new CommentTerminal("LineComment", "//", "\n");
      NonGrammarTerminals.Add(LineComment);
      #endregion

      #region 2. Non-terminals
      #region 2.1 Expressions
      NonTerminal Expr = new NonTerminal("Expr", typeof(ScriptExpr));
      NonTerminal ConstExpr = new NonTerminal("ConstExpr", typeof(ScriptConstExpr));
      NonTerminal BinExpr = new NonTerminal("BinExpr", typeof(ScriptBinExpr));
      NonTerminal UnaryExpr = new NonTerminal("UnaryExpr", typeof(ScriptUnaryExpr));
      NonTerminal AssignExpr = new NonTerminal("AssignExpr", typeof(ScriptAssignExpr));
      NonTerminal TypeConvertExpr = new NonTerminal("TypeConvertExpr", typeof(ScriptTypeConvertExpr));
      NonTerminal IsExpr = new NonTerminal("IsExpr", typeof(ScriptIsExpr));
      NonTerminal MetaExpr = new NonTerminal("MetaExpr", typeof(ScriptMetaExpr));
      NonTerminal FuncDefExpr = new NonTerminal("FuncDefExpr", typeof(ScriptFunctionDefinition));  //typeof(ScriptFunctionDefExpression));

      NonTerminal RefExpr = new NonTerminal("RefExpr", typeof(ScriptRefExpr));
      NonTerminal VarExpr = new NonTerminal("VarExpr", typeof(ScriptVarExpr));
      NonTerminal TypeExpr = new NonTerminal("TypeExpr", typeof(ScriptTypeExpr));
      NonTerminal TypeConstructor = new NonTerminal("TypeConstructor", typeof(ScriptTypeConstructor));
      NonTerminal FunctionCall = new NonTerminal("FunctionCall", typeof(ScriptFunctionCall));
      NonTerminal ArrayResolution = new NonTerminal("ArrayResolution", typeof(ScriptArrayResolution));

      NonTerminal BinOp = new NonTerminal("BinOp");
      NonTerminal LUnOp = new NonTerminal("LUnOp");
      NonTerminal RUnOp = new NonTerminal("RUnOp");

      NonTerminal ArrayConstructor = new NonTerminal("ArrayConstructor", typeof(ScriptArrayConstructor));
      NonTerminal MObjectConstructor = new NonTerminal("MObjectConstructor", typeof(ScriptMObject));
      NonTerminal MObjectPart = new NonTerminal("MObjectPart", typeof(ScriptMObjectPart));
      NonTerminal MObjectParts = new NonTerminal("MObjectPart", typeof(ScriptAst));

      NonTerminal TypeList = new NonTerminal("TypeList", typeof(ScriptTypeExprList));
      #endregion

      #region 2.2 QualifiedName
      //Expression List:  expr1, expr2, expr3, ..
      NonTerminal ExprList = new NonTerminal("ExprList", typeof(ScriptExprList));

      //A name in form: a.b.c().d[1,2].e ....
      NonTerminal NewStmt = new NonTerminal("NewStmt", typeof(ScriptNewStmt));
      NonTerminal NewArrStmt = new NonTerminal("NewArrStmt", typeof(ScriptNewArrStmt));
      NonTerminal QualifiedName = new NonTerminal("QualifiedName", typeof(ScriptQualifiedName));
      NonTerminal GenericsPostfix = new NonTerminal("GenericsPostfix", typeof(ScriptGenericsPostfix));

      NonTerminal GlobalList = new NonTerminal("GlobalList", typeof(ScriptGlobalList));
      #endregion

      #region 2.3 Statement
      NonTerminal Condition = new NonTerminal("Condition", typeof(ScriptCondition));
      NonTerminal Statement = new NonTerminal("Statement", typeof(ScriptStatement));

      NonTerminal IfStatement = new NonTerminal("IfStatement", typeof(ScriptIfStatement));
      NonTerminal WhileStatement = new NonTerminal("WhileStatement", typeof(ScriptWhileStatement));
      NonTerminal ForStatement = new NonTerminal("ForStatement", typeof(ScriptForStatement));
      NonTerminal ForEachStatement = new NonTerminal("ForEachStatement", typeof(ScriptForEachStatement));
      NonTerminal OptionalExpression = new NonTerminal("OptionalExpression", typeof(ScriptExpr));
      NonTerminal SwitchStatement = new NonTerminal("SwitchStatement", typeof(ScriptSwitchRootStatement));
      NonTerminal SwitchStatements = new NonTerminal("SwitchStatements", typeof(ScriptSwitchStatement));
      NonTerminal SwitchCaseStatement = new NonTerminal("SwitchCaseStatement", typeof(ScriptSwitchCaseStatement));
      NonTerminal SwitchDefaultStatement = new NonTerminal("SwitchDefaultStatement", typeof(ScriptSwitchDefaultStatement));
      NonTerminal UsingStatement = new NonTerminal("UsingStatement", typeof(ScriptUsingStatement));
      NonTerminal TryCatchFinallyStatement = new NonTerminal("TryCatchFinallyStatement", typeof(ScriptTryCatchFinallyStatement));
      NonTerminal FlowControlStatement = new NonTerminal("FlowControl", typeof(ScriptFlowControlStatement));
      NonTerminal ExprStatement = new NonTerminal("ExprStatement", typeof(ScriptStatement));

      //Block
      NonTerminal BlockStatement = new NonTerminal("BlockStatement", typeof(ScriptStatement));
      NonTerminal Statements = new NonTerminal("Statements(Compound)", typeof(ScriptCompoundStatement));
      #endregion

      #region 2.4 Program and Functions
      NonTerminal Prog = new NonTerminal("Prog", typeof(ScriptProg));
      NonTerminal Element = new NonTerminal("Element", typeof(ScriptAst));
      NonTerminal Elements = new NonTerminal("Elements", typeof(ScriptElements));
      NonTerminal FuncDef = new NonTerminal("FuncDef", typeof(ScriptFunctionDefinition));
      NonTerminal FuncContract = new NonTerminal("FuncContract", typeof(ScriptFuncContract));
      NonTerminal ParameterList = new NonTerminal("ParamaterList", typeof(ScriptFuncParameters));

      NonTerminal FuncContractPre = new NonTerminal("Pre Conditions", typeof(ScriptFuncContractPre));
      NonTerminal FuncContractPost = new NonTerminal("Post Conditions", typeof(ScriptFuncContractPost));
      NonTerminal FuncContractInv = new NonTerminal("Invariant Conditions", typeof(ScriptFuncContractInv));
      #endregion

      #endregion

      #region 3. BNF rules
      #region 3.1 Expressions
      ConstExpr.Rule = SymbolTerminal.GetSymbol("true")
                      | "false"
                      | "null"
                      | s
                      | n;

      BinExpr.Rule = Expr + BinOp + Expr
                     | IsExpr;

      UnaryExpr.Rule = LUnOp + Expr;

      IsExpr.Rule = Expr + @is + TypeExpr;

      TypeConvertExpr.Rule = LCb + Expr + RCb + Expr.Q();

      AssignExpr.Rule = QualifiedName + "=" + Expr
                       | VarExpr + "=" + Expr
                       | QualifiedName + "++"
                       | QualifiedName + "--"
                       | QualifiedName + ":=" + Expr
                       | QualifiedName + "+=" + Expr
                       | QualifiedName + "-=" + Expr;

      //TODO: MetaFeatures;
      // <[    ] + > because of conflict a[1]>2
      MetaExpr.Rule = LMb + Elements + RMb;

      GlobalList.Rule = "global" + LCb + ParameterList + RCb;

      FuncDefExpr.Rule = "function" + LCb + ParameterList + RCb
        + GlobalList.Q()
        + FuncContract.Q()
        + BlockStatement;

      Expr.Rule = ConstExpr
                  | BinExpr
                  | UnaryExpr
                  | QualifiedName
                  | AssignExpr
                  | NewStmt
                  | FuncDefExpr
                  | NewArrStmt
                  | ArrayConstructor
                  | MObjectConstructor
                  | TypeConvertExpr
                  | MetaExpr
        //NOTE: ref, out
                  | RefExpr
                  | VarExpr
                  ;

      VarExpr.Rule = "var" + v;

      RefExpr.Rule = (@ref | @out) + v;

      NewStmt.Rule = "new" + TypeConstructor;
      NewArrStmt.Rule = "new" + TypeExpr + ArrayResolution;
      BinOp.Rule = SymbolTerminal.GetSymbol("+") | "-" | "*" | "/" | "%" | "^" | "&" | "|"
                  | "&&" | "||" | "==" | "!=" | greater | less
                  | ">=" | "<=";

      LUnOp.Rule = SymbolTerminal.GetSymbol("~") | "-" | "!" | "$";

      ArrayConstructor.Rule = LSb + ExprList + RSb;

      MObjectPart.Rule = v + arrow + Expr;
      MObjectParts.Rule = MakePlusRule(MObjectParts, comma, MObjectPart);
      MObjectConstructor.Rule = LSb + MObjectParts + RSb;

      OptionalExpression.Rule = Expr.Q();
      #endregion

      #region 3.2 QualifiedName
      TypeExpr.Rule = //MakePlusRule(TypeExpr, dot, v);
          v + GenericsPostfix.Q()
          | TypeExpr + dot + (v + GenericsPostfix.Q());

      GenericsPostfix.Rule = LGb + TypeList + RGb;
      FunctionCall.Rule = LCb + ExprList.Q() + RCb;
      ArrayResolution.Rule = LSb + ExprList + RSb;

      QualifiedName.Rule = v + (GenericsPostfix | ArrayResolution | FunctionCall).Star()
                          | QualifiedName + dot + v + (GenericsPostfix | ArrayResolution | FunctionCall).Star();

      ExprList.Rule = MakePlusRule(ExprList, comma, Expr);
      TypeList.Rule = MakePlusRule(TypeList, comma, TypeExpr);
      TypeConstructor.Rule = TypeExpr + FunctionCall;
      #endregion

      #region 3.3 Statement
      Condition.Rule = LCb + Expr + RCb;
      IfStatement.Rule = "if" + Condition + Statement + ("else" + Statement).Q();
      WhileStatement.Rule = "while" + Condition + Statement;
      ForStatement.Rule = "for" + LCb + OptionalExpression + semicolon + OptionalExpression + semicolon + OptionalExpression + RCb + Statement;
      ForEachStatement.Rule = "foreach" + LCb + v + "in" + Expr + RCb + Statement;
      UsingStatement.Rule = "using" + LCb + Expr + RCb + BlockStatement;
      TryCatchFinallyStatement.Rule = "try" + BlockStatement + "catch" + LCb + v + RCb + BlockStatement + "finally" + BlockStatement;
      SwitchStatement.Rule = "switch" + LCb + Expr + RCb + LFb + SwitchStatements + RFb;
      ExprStatement.Rule = Expr + semicolon;
      FlowControlStatement.Rule = "break" + semicolon
                                | "continue" + semicolon
                                | "return" + Expr + semicolon
                                | "throw" + Expr + semicolon;

      Statement.Rule = semicolon
                      | IfStatement                 //1. If
                      | WhileStatement              //2. While
                      | ForStatement                //3. For
                      | ForEachStatement            //4. ForEach
                      | UsingStatement              //5. Using
                      | SwitchStatement             //6. Switch
                      | BlockStatement              //7. Block
                      | TryCatchFinallyStatement    //8. TryCatch
                      | ExprStatement               //9. Expr
                      | FlowControlStatement;       //10. FlowControl

      Statements.SetOption(TermOptions.IsList);
      Statements.Rule = Statements + Statement | Empty;
      BlockStatement.Rule = LFb + Statements + RFb;

      SwitchStatements.Rule = SwitchCaseStatement.Star() + SwitchDefaultStatement.Q();
      SwitchCaseStatement.Rule = SymbolTerminal.GetSymbol("case") + Expr + colon + Statements;
      SwitchDefaultStatement.Rule = "default" + colon + Statements;
      #endregion

      #region 3.4 Prog
      FuncContract.Rule = LSb +
                           FuncContractPre + semicolon +
                           FuncContractPost + semicolon +
                           FuncContractInv + semicolon +
                          RSb;
      FuncContractPre.Rule = "pre" + LCb + ExprList.Q() + RCb;
      FuncContractPost.Rule = "post" + LCb + ExprList.Q() + RCb;
      FuncContractInv.Rule = "invariant" + LCb + ExprList.Q() + RCb;

      ParameterList.Rule = MakeStarRule(ParameterList, comma, v);
      FuncDef.Rule = "function" + v + LCb + ParameterList + RCb
        + GlobalList.Q()
        + FuncContract.Q()
        + BlockStatement;

      Element.Rule = Statement | FuncDef;
      Elements.SetOption(TermOptions.IsList);
      Elements.Rule = Elements + Element | Empty;

      Prog.Rule = Elements + Eof;
      #endregion
      #endregion

      #region 4. Set starting symbol
      if (!expressionGrammar)
        Root = Prog; // Set grammar root
      else
        Root = Expr;
      #endregion

      #region 5. Operators precedence
      SymbolTerminal.RegisterOperators(1, "=", "+=", "-=", ":=");
      SymbolTerminal.RegisterOperators(2, "|", "||");
      SymbolTerminal.RegisterOperators(3, "&", "&&");
      SymbolTerminal.RegisterOperators(4, "==", "!=", ">", "<", ">=", "<=");
      SymbolTerminal.RegisterOperators(5, "is");
      SymbolTerminal.RegisterOperators(6, "+", "-");
      SymbolTerminal.RegisterOperators(7, "*", "/", "%");
      SymbolTerminal.RegisterOperators(8, Associativity.Right, "^");
      SymbolTerminal.RegisterOperators(9, "~", "!", "$", "++", "--");
      SymbolTerminal.RegisterOperators(10, ".");

      //RegisterOperators(10, Associativity.Right, ".",",", ")", "(", "]", "[", "{", "}");
      //RegisterOperators(11, Associativity.Right, "else");
      #endregion

      #region 6. Punctuation symbols
      SymbolTerminal.RegisterPunctuation("(", ")", "[", "]", "{", "}", ",", ";");
      #endregion
    }

    public static Grammar CreateScriptGrammar(bool expressionGrammar)
    {
      Grammar result = new Grammar();
      result.CreateGrammar(expressionGrammar);
      return result;
    }
    #endregion

    #region Helpers
    public GrammarExpression MakePlusRule(NonTerminal listNonTerminal, GrammarTerm delimiter, GrammarTerm listMember)
    {
      listNonTerminal.SetOption(TermOptions.IsList);
      if (delimiter == null)
        listNonTerminal.Rule = listMember | listNonTerminal + listMember;
      else
        listNonTerminal.Rule = listMember | listNonTerminal + delimiter + listMember;
      return listNonTerminal.Rule;
    }

    public GrammarExpression MakeStarRule(NonTerminal listNonTerminal, GrammarTerm delimiter, GrammarTerm listMember)
    {
      if (delimiter == null)
      {
        //it is much simpler case
        listNonTerminal.SetOption(TermOptions.IsList);
        listNonTerminal.Rule = Empty | listNonTerminal + listMember;
        return listNonTerminal.Rule;
      }
      NonTerminal tmp = new NonTerminal(listMember.Name + "+");
      MakePlusRule(tmp, delimiter, listMember);
      listNonTerminal.Rule = Empty | tmp;
      listNonTerminal.SetOption(TermOptions.IsStarList);
      return listNonTerminal.Rule;
    }
    #endregion

    #region Pseudo terminals
    public readonly static Terminal Empty = new Terminal("EMPTY");
    public readonly static Terminal Eof = new Terminal("EOF", TokenCategory.Outline);
    public readonly static Terminal SyntaxError = new Terminal("SYNTAX_ERROR", TokenCategory.Error);

    internal bool IsPseudoTerminal(Terminal term)
    {
      return term == Empty || term == Eof || term == SyntaxError;        
    }
    #endregion
  }
}
