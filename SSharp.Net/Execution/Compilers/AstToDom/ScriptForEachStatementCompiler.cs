namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using Parser.Ast;

  [CompilerType(typeof(ScriptForEachStatement))]
  internal class ScriptForEachStatementCompiler : IDomCompiler
  {
    #region IDomCompiler Members

    public CodeObject Compile(AstNode syntaxNode, CodeProgram prog)
    {
      var syntax = (ScriptForEachStatement)syntaxNode;

      var code = new CodeForEachStatement(
         new CodeVariableReference(syntax.Id),
         AstDomCompiler.Compile<CodeExpression>(syntax.Container, prog),
         AstDomCompiler.Compile<CodeStatement>(syntax.Statement, prog));

      return code;
    }

    #endregion
  }
}
