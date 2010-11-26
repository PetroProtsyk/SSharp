using System.Linq;

namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using Parser.Ast;

  [CompilerType(typeof(ScriptStatement))]
  internal class ScriptStatementCompiler : IDomCompiler
  {
    #region IDomCompiler Members

    public CodeObject Compile(AstNode syntaxNode, CodeProgram prog)
    {
      var statement = (ScriptStatement)syntaxNode;

      foreach (var code in statement.Select(subStatement => AstDomCompiler.Compile(subStatement, prog)))
      {
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
