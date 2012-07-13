using Scripting.SSharp.Parser;

namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using Scripting.SSharp.Parser.Ast;

  [CompilerType(typeof(ScriptForEachStatement))]
  class ScriptForEachStatementCompiler : IDomCompiler
  {
    #region IDomCompiler Members

    public CodeObject Compile(AstNode syntaxNode, CodeProgram prog)
    {
      ScriptForEachStatement syntax = (ScriptForEachStatement)syntaxNode;

      CodeForEachStatement code = new CodeForEachStatement(
         new CodeVariableReference(syntax.Id),
         AstDomCompiler.Compile<CodeExpression>(syntax.Container, prog),
         AstDomCompiler.Compile<CodeStatement>(syntax.Statement, prog));

      return code;
    }

    #endregion
  }
}
