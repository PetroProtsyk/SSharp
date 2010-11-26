using Scripting.SSharp.Parser.Ast;

namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;

  [CompilerType(typeof(ScriptCompoundStatement))]
  internal class ScriptCompoundStatementCompiler : IDomCompiler
  {
    #region IDomCompiler Members

    public CodeObject Compile(AstNode syntaxNode, CodeProgram prog)
    {
      var syntax = (ScriptCompoundStatement)syntaxNode;

      var block = new CodeBlockStatement();

      foreach (var syntaxStatement in syntax.Statements)
        block.Statements.Add(
          (CodeStatement)AstDomCompiler.Compile(syntaxStatement, prog));

      return block;
    }

    #endregion
  }
}
