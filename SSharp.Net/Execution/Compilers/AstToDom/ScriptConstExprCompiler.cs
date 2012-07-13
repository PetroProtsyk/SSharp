using Scripting.SSharp.Parser;

namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using Scripting.SSharp.Parser.Ast;

  [CompilerType(typeof(ScriptConstExpr))]
  class ScriptConstExprCompiler : IDomCompiler
  {
    #region IDomCompiler Members

    public CodeObject Compile(AstNode syntaxNode, CodeProgram prog)
    {
      ScriptConstExpr syntaxConstExpr = (ScriptConstExpr)syntaxNode;

      return new CodeValueReference(syntaxConstExpr.Value) { SourceSpan = syntaxNode.Span };
    }

    #endregion
  }
}