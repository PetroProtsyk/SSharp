namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using Parser.Ast;

  [CompilerType(typeof(ScriptWhileStatement))]
  internal class ScriptWhileStatementCompiler : IDomCompiler
  {
    #region IDomCompiler Members

    public CodeObject Compile(AstNode syntaxNode, CodeProgram prog)
    {
      var syntax = (ScriptWhileStatement)syntaxNode;

      var code = new CodeWhileStatement(
         AstDomCompiler.Compile<CodeExpression>(syntax.Condition.Expression, prog),
         AstDomCompiler.Compile<CodeStatement>(syntax.Statement, prog));

      return code;
    }

    #endregion
  }
}
