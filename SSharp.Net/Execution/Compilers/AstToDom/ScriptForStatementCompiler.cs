namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using Parser.Ast;

  [CompilerType(typeof(ScriptForStatement))]
  internal class ScriptForStatementCompiler : IDomCompiler
  {
    #region IDomCompiler Members

    public CodeObject Compile(AstNode syntaxNode, CodeProgram prog)
    {
      var syntax = (ScriptForStatement)syntaxNode;

      var code = new CodeForStatement(
         AstDomCompiler.Compile<CodeExpression>(syntax.Init, prog),
         AstDomCompiler.Compile<CodeExpression>(syntax.Condition, prog),
         AstDomCompiler.Compile<CodeExpression>(syntax.Next, prog),
         AstDomCompiler.Compile<CodeStatement>(syntax.Statement, prog));

      return code;
    }

    #endregion
  }
}
