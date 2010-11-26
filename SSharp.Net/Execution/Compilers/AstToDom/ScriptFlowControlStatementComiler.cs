using System;

namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using Parser.Ast;

  [CompilerType(typeof(ScriptFlowControlStatement))]
  internal class ScriptFlowControlStatementComiler : IDomCompiler
  {
    #region IDomCompiler Members

    public CodeObject Compile(AstNode syntaxNode, CodeProgram prog)
    {
      var syntax = (ScriptFlowControlStatement)syntaxNode;

      if (syntax.Symbol == "return")
      {
        var code = new CodeReturnStatement((CodeExpression)AstDomCompiler.Compile(syntax.Expression, prog));
        return code;
      }

      throw new NotImplementedException();
    }

    #endregion
  }
}
