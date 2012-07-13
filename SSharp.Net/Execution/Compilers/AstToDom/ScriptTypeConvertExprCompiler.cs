using Scripting.SSharp.Parser;

namespace Scripting.SSharp.Execution.Compilers
{

  using Dom;
  using Scripting.SSharp.Parser.Ast;

  [CompilerType(typeof(ScriptTypeConvertExpr))]
  class ScriptTypeConvertExprCompiler : IDomCompiler
  {
    #region IDomCompiler Members

    public CodeObject Compile(AstNode syntaxNode, CodeProgram prog)
    {
      ScriptTypeConvertExpr syntax = (ScriptTypeConvertExpr)syntaxNode;

      if (syntax.TypeExpression == null)
        return AstDomCompiler.Compile(syntax.Expression, prog);
      else
      {
        CodeExpression typeExpr = (CodeExpression)AstDomCompiler.Compile(syntax.TypeExpression, prog);
        CodeExpression expr = (CodeExpression)AstDomCompiler.Compile(syntax.Expression, prog);
      }

      return null;
    }

    #endregion
  }
}
