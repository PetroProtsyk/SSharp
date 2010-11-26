namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using Parser.Ast;

  [CompilerType(typeof(ScriptConstExpr))]
  internal class ScriptConstExprCompiler : IDomCompiler
  {
    #region IDomCompiler Members

    public CodeObject Compile(AstNode syntaxNode, CodeProgram prog)
    {
      var syntaxConstExpr = (ScriptConstExpr)syntaxNode;

      return new CodeValueReference(syntaxConstExpr.Value) { SourceSpan = syntaxNode.Span };
    }

    #endregion
  }
}