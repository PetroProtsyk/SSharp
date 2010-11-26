namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using Parser.Ast;
  
  [CompilerType(typeof(ScriptAssignExpr))]
  internal class ScriptAssignExprCompiler : IDomCompiler
  {
    #region IDomCompiler Members

    public CodeObject Compile(AstNode syntaxNode, CodeProgram prog)
    {
      var syntaxAssignExpr = (ScriptAssignExpr)syntaxNode;
      
      if (syntaxAssignExpr.LeftExpression.IsVariable)
      {
        if (syntaxAssignExpr.Symbol == "++")
        {
          var binary = new CodeBinaryOperator
          {
            Left = new CodeVariableReference(syntaxAssignExpr.LeftExpression.Identifier),
            Right = new CodeValueReference(1),
            Type = OperatorType.Plus
          };

          var codeAssign = new CodeAssignExpression(
            syntaxAssignExpr.LeftExpression.Identifier,
            binary) {SourceSpan = syntaxNode.Span};

          return codeAssign;
        }
        else
          if (syntaxAssignExpr.Symbol == "--")
          {
            var binary = new CodeBinaryOperator
            {
              Left = new CodeVariableReference(syntaxAssignExpr.LeftExpression.Identifier),
              Right = new CodeValueReference(1),
              Type = OperatorType.Minus
            };

            var codeAssign = new CodeAssignExpression(
              syntaxAssignExpr.LeftExpression.Identifier,
              binary) {SourceSpan = syntaxNode.Span};

            return codeAssign;
          }
          else
          {
            var codeAssign = new CodeAssignExpression(
              syntaxAssignExpr.LeftExpression.Identifier,
              (CodeExpression)AstDomCompiler.Compile(syntaxAssignExpr.RightExpression, prog))
            {SourceSpan = syntaxNode.Span};

            return codeAssign;
          }
      }

      return null;
    }

    #endregion
  }
}
