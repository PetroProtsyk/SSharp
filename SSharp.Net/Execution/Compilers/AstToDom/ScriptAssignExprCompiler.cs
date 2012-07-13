using Scripting.SSharp.Parser;

namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using Scripting.SSharp.Parser.Ast;
  
  [CompilerType(typeof(ScriptAssignExpr))]
  class ScriptAssignExprCompiler : IDomCompiler
  {
    #region IDomCompiler Members

    public CodeObject Compile(AstNode syntaxNode, CodeProgram prog)
    {
      ScriptAssignExpr syntaxAssignExpr = (ScriptAssignExpr)syntaxNode;
      
      if (syntaxAssignExpr.LeftExpression.IsVariable)
      {
        if (syntaxAssignExpr.Symbol == "++")
        {
          CodeBinaryOperator binary = new CodeBinaryOperator();
          binary.Left = new CodeVariableReference(syntaxAssignExpr.LeftExpression.Identifier);
          binary.Right = new CodeValueReference(1);
          binary.Type = OperatorType.Plus;
          
          CodeAssignExpression codeAssign = new CodeAssignExpression(
            syntaxAssignExpr.LeftExpression.Identifier,
            binary);

          codeAssign.SourceSpan = syntaxNode.Span;
          return codeAssign;
        }
        else
          if (syntaxAssignExpr.Symbol == "--")
          {
            CodeBinaryOperator binary = new CodeBinaryOperator();
            binary.Left = new CodeVariableReference(syntaxAssignExpr.LeftExpression.Identifier);
            binary.Right = new CodeValueReference(1);
            binary.Type = OperatorType.Minus;

            CodeAssignExpression codeAssign = new CodeAssignExpression(
              syntaxAssignExpr.LeftExpression.Identifier,
              binary);

            codeAssign.SourceSpan = syntaxNode.Span;
            return codeAssign;
          }
          else
          {
            CodeAssignExpression codeAssign = new CodeAssignExpression(
              syntaxAssignExpr.LeftExpression.Identifier,
              (CodeExpression)AstDomCompiler.Compile(syntaxAssignExpr.RightExpression, prog));
            codeAssign.SourceSpan = syntaxNode.Span;

            return codeAssign;
          }
      }

      return null;
    }

    #endregion
  }
}
