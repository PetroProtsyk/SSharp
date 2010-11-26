using System.Collections.Generic;
using Scripting.SSharp.Parser.Ast;

namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;

  [CompilerType(typeof(ScriptSwitchRootStatement))]
  internal class ScriptSwitchStatementCompiler : IDomCompiler
  {
    #region IDomCompiler Members

    public CodeObject Compile(AstNode syntaxNode, CodeProgram prog)
    {
      var syntax = (ScriptSwitchRootStatement)syntaxNode;

      var condition = AstDomCompiler.Compile<CodeExpression>(syntax.Expression, prog);

      var cases = new List<CodeSwitchCase> ();
      foreach (var statement in syntax.Switch.Cases)
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

      var code = new CodeSwitchStatement(condition, cases);
      return code;
    }

    #endregion
  }
}
