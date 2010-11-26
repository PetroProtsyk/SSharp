namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using Parser.Ast;
  
  [CompilerType(typeof(ScriptIfStatement))]
  internal class ScriptIfStatementCompiler : IDomCompiler
  {
    #region IDomCompiler Members

    public CodeObject Compile(AstNode syntaxNode, CodeProgram prog)
    {
      var syntax = (ScriptIfStatement)syntaxNode;

      var code = new CodeIfStatement(
         AstDomCompiler.Compile<CodeExpression>(syntax.Condition.Expression, prog),
         AstDomCompiler.Compile<CodeStatement>(syntax.Statement, prog),
         syntax.ElseStatement == null ? null : AstDomCompiler.Compile<CodeStatement>(syntax.ElseStatement, prog));


      return code;
    }

    #endregion
  }
}
