using System.Collections.Generic;

namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using Parser.Ast;

  [CompilerType(typeof(ScriptBinExpr))]
  internal class ScriptBinExprCompiler : IDomCompiler
  {
    #region IDomCompiler Members

    private static readonly Dictionary<string, OperatorType> Mapping = new Dictionary<string, OperatorType>
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
      var syntaxBinExpr = (ScriptBinExpr)syntaxNode;

      var binary = new CodeBinaryOperator
      {
        Left = (CodeExpression) AstDomCompiler.Compile(syntaxBinExpr.Left, prog),
        Right = (CodeExpression) AstDomCompiler.Compile(syntaxBinExpr.Right, prog),
        Type = Mapping[syntaxBinExpr.Symbol]
      };

      return binary;
    }

    #endregion
  }
}
