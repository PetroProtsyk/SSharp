using Scripting.SSharp.Parser;

namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using Scripting.SSharp.Parser.Ast;

  [CompilerType(typeof(ScriptStatement))]
  class ScriptStatementCompiler : IDomCompiler
  {
    #region IDomCompiler Members

    public CodeObject Compile(AstNode syntaxNode, CodeProgram prog)
    {
      ScriptStatement statement = (ScriptStatement)syntaxNode;

      foreach (ScriptAst subStatement in statement)
      {
        CodeObject code = AstDomCompiler.Compile(subStatement, prog);

        if (code is CodeExpression)
        {
          return new CodeExpressionStatement((CodeExpression)code) { SourceSpan = syntaxNode.Span };
        }

        if (code is CodeStatement)
          return code;
      }

      return null;
    }

    #endregion
  }
}
