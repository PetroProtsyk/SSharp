using System.Collections.Generic;
using Scripting.SSharp.Parser;

namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using Scripting.SSharp.Parser.Ast;

  [CompilerType(typeof(ScriptFunctionCall))]
  class ScriptFunctionCallCompiler : IDomCompiler
  {
    #region IDomCompiler Members

    public CodeObject Compile(AstNode syntaxNode, CodeProgram prog)
    {
      ScriptFunctionCall syntax = (ScriptFunctionCall)syntaxNode;

      List<CodeExpression> parameters = new List<CodeExpression>();
      foreach (ScriptExpr expr in syntax.Parameters)
        parameters.Add(AstDomCompiler.Compile<CodeExpression>(expr, prog));
      
      CodeObjectFunctionCall code = new CodeObjectFunctionCall(parameters);

      return code;
    }

    #endregion
  }
}
