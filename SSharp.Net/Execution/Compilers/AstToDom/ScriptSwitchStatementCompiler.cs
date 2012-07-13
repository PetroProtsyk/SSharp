using System.Collections.Generic;
using Scripting.SSharp.Parser;
using Scripting.SSharp.Parser.Ast;

namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;

  [CompilerType(typeof(ScriptSwitchRootStatement))]
  class ScriptSwitchStatementCompiler : IDomCompiler
  {
    #region IDomCompiler Members

    public CodeObject Compile(AstNode syntaxNode, CodeProgram prog)
    {
      ScriptSwitchRootStatement syntax = (ScriptSwitchRootStatement)syntaxNode;

      CodeExpression condition = AstDomCompiler.Compile<CodeExpression>(syntax.Expression, prog);

      List<CodeSwitchCase> cases = new List<CodeSwitchCase> ();
      foreach (ScriptSwitchCaseStatement statement in syntax.Switch.Cases)
      {
        cases.Add(new CodeSwitchCase(
          AstDomCompiler.Compile<CodeExpression>(statement.Condition, prog),
          AstDomCompiler.Compile<CodeStatement>(statement.Statement, prog)));
      }
      
      if (syntax.Switch.DefaultCase != null)
      {
        cases.Add(new CodeSwitchCase(
            null,
            AstDomCompiler.Compile<CodeStatement>(syntax.Switch.DefaultCase.Statement, prog)));
      }

      CodeSwitchStatement code = new CodeSwitchStatement(condition, cases);
      return code;
    }

    #endregion
  }
}
