namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using Parser.Ast;

  [CompilerType(typeof(ScriptTypeConvertExpr))]
  internal class ScriptTypeConvertExprCompiler : IDomCompiler
  {
    #region IDomCompiler Members

    public CodeObject Compile(AstNode syntaxNode, CodeProgram prog)
    {
      var syntax = (ScriptTypeConvertExpr)syntaxNode;

      if (syntax.TypeExpression == null)
        return AstDomCompiler.Compile(syntax.Expression, prog);

      var typeExpr = (CodeExpression)AstDomCompiler.Compile(syntax.TypeExpression, prog);
      var expr = (CodeExpression)AstDomCompiler.Compile(syntax.Expression, prog);

      return null;
    }

    #endregion
  }
}
