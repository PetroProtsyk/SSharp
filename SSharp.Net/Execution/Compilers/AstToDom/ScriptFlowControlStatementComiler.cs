using System;
using Scripting.SSharp.Parser;

namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using Scripting.SSharp.Parser.Ast;

  [CompilerType(typeof(ScriptFlowControlStatement))]
  class ScriptFlowControlStatementComiler : IDomCompiler
  {
    #region IDomCompiler Members

    public CodeObject Compile(AstNode syntaxNode, CodeProgram prog)
    {
      ScriptFlowControlStatement syntax = (ScriptFlowControlStatement)syntaxNode;

      if (syntax.Symbol == "return")
      {
        CodeReturnStatement code = new CodeReturnStatement((CodeExpression)AstDomCompiler.Compile(syntax.Expression, prog));
        return code;
      }

      throw new NotImplementedException();
    }

    #endregion
  }
}
