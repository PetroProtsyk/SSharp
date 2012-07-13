using System.Collections.Generic;
using Scripting.SSharp.Parser;

namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using Scripting.SSharp.Parser.Ast;

  [CompilerType(typeof(ScriptBinExpr))]
  class ScriptBinExprCompiler : IDomCompiler
  {
    #region IDomCompiler Members

    private static Dictionary<string, OperatorType> mapping = new Dictionary<string, OperatorType>()
    {
      {"+", OperatorType.Plus},
      {"-", OperatorType.Minus},
      {"*", OperatorType.Mul},
      {"/", OperatorType.Div},
      {"%", OperatorType.Mod},
      {"^", OperatorType.Pow},

      {">", OperatorType.Greater},
      {"<", OperatorType.Less},
      {">=", OperatorType.GreaterEq},
      {"<=", OperatorType.LessEq},
      {"==", OperatorType.Eq},
      {"!=", OperatorType.Neq},
    };

    public CodeObject Compile(AstNode syntaxNode, CodeProgram prog)
    {
      ScriptBinExpr syntaxBinExpr = (ScriptBinExpr)syntaxNode;

      CodeBinaryOperator binary = new CodeBinaryOperator();
      binary.Left = (CodeExpression)AstDomCompiler.Compile(syntaxBinExpr.Left, prog);
      binary.Right = (CodeExpression)AstDomCompiler.Compile(syntaxBinExpr.Right, prog);
      binary.Type = mapping[syntaxBinExpr.Symbol];
      
      return binary;
    }

    #endregion
  }
}
