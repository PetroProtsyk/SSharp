using System.Linq;

namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using Parser.Ast;

  [CompilerType(typeof(ScriptFunctionCall))]
  internal class ScriptFunctionCallCompiler : IDomCompiler
  {
    #region IDomCompiler Members

    public CodeObject Compile(AstNode syntaxNode, CodeProgram prog)
    {
      var syntax = (ScriptFunctionCall)syntaxNode;
      var parameters = syntax.Parameters.Select(expr => AstDomCompiler.Compile<CodeExpression>(expr, prog)).ToList();
      var code = new CodeObjectFunctionCall(parameters);
      return code;
    }

    #endregion
  }
}
