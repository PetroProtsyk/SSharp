using Scripting.SSharp.Parser;

namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using Scripting.SSharp.Parser.Ast;

  [CompilerType(typeof(ScriptForStatement))]
  class ScriptForStatementCompiler : IDomCompiler
  {
    #region IDomCompiler Members

    public CodeObject Compile(AstNode syntaxNode, CodeProgram prog)
    {
      ScriptForStatement syntax = (ScriptForStatement)syntaxNode;

      CodeForStatement code = new CodeForStatement(
         AstDomCompiler.Compile<CodeExpression>(syntax.Init, prog),
         AstDomCompiler.Compile<CodeExpression>(syntax.Condition, prog),
         AstDomCompiler.Compile<CodeExpression>(syntax.Next, prog),
         AstDomCompiler.Compile<CodeStatement>(syntax.Statement, prog));

      return code;
    }

    #endregion
  }
}
