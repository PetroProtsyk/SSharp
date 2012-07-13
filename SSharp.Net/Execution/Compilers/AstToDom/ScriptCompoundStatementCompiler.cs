using Scripting.SSharp.Parser;
using Scripting.SSharp.Parser.Ast;

namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;

  [CompilerType(typeof(ScriptCompoundStatement))]
  class ScriptCompoundStatementCompiler : IDomCompiler
  {
    #region IDomCompiler Members

    public CodeObject Compile(AstNode syntaxNode, CodeProgram prog)
    {
      ScriptCompoundStatement syntax = (ScriptCompoundStatement)syntaxNode;

      CodeBlockStatement block = new CodeBlockStatement();

      foreach (var syntaxStatement in syntax.Statements)
        block.Statements.Add(
          (CodeStatement)AstDomCompiler.Compile(syntaxStatement, prog));

      return block;
    }

    #endregion
  }
}
