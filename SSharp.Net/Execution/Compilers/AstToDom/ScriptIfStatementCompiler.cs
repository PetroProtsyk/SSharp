using Scripting.SSharp.Parser;

namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using Scripting.SSharp.Parser.Ast;
  
  [CompilerType(typeof(ScriptIfStatement))]
  class ScriptIfStatementCompiler : IDomCompiler
  {
    #region IDomCompiler Members

    public CodeObject Compile(AstNode syntaxNode, CodeProgram prog)
    {
      ScriptIfStatement syntax = (ScriptIfStatement)syntaxNode;

      CodeIfStatement code = new CodeIfStatement(
         AstDomCompiler.Compile<CodeExpression>(syntax.Condition.Expression, prog),
         AstDomCompiler.Compile<CodeStatement>(syntax.Statement, prog),
         syntax.ElseStatement == null ? null : AstDomCompiler.Compile<CodeStatement>(syntax.ElseStatement, prog));


      return code;
    }

    #endregion
  }
}
