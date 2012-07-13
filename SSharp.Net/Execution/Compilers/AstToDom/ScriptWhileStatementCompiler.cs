using Scripting.SSharp.Parser;

namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using Scripting.SSharp.Parser.Ast;

  [CompilerType(typeof(ScriptWhileStatement))]
  class ScriptWhileStatementCompiler : IDomCompiler
  {
    #region IDomCompiler Members

    public CodeObject Compile(AstNode syntaxNode, CodeProgram prog)
    {
      ScriptWhileStatement syntax = (ScriptWhileStatement)syntaxNode;

      CodeWhileStatement code = new CodeWhileStatement(
         AstDomCompiler.Compile<CodeExpression>(syntax.Condition.Expression, prog),
         AstDomCompiler.Compile<CodeStatement>(syntax.Statement, prog));

      return code;
    }

    #endregion
  }
}
