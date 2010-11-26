using System.Collections.Generic;

namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using VM;

  [CompilerType(typeof(CodeSwitchStatement))]
  internal class CodeSwitchStatementCompiler : IVMCompiler
  {
    #region IVMCompiler Members
    private static long _sid;

    public ExecutableMachine Compile(CodeObject code, ExecutableMachine machine)
    {
      var codeSwitch = (CodeSwitchStatement)code;

      //switch(a) { case c1 : s1; case c2 : s2; default : s3 }
      //~
      //switch_expr = a;
      //if (a == c1) s1
      //else if (a == c2) s2
      //else s3

      _sid++;

      var block = new CodeBlockStatement { SourceSpan = codeSwitch.SourceSpan };

      var switchExpr = new CodeAssignExpression(
        "#switch_"+_sid, codeSwitch.Expression);
         
      var ifStatement = BuildNextIf(codeSwitch.Cases, 0);

      block.Statements.Add(new CodeExpressionStatement(switchExpr));
      block.Statements.Add(ifStatement);

      CodeDomCompiler.Compile(block, machine);

      return machine;
    }

    private static CodeStatement BuildNextIf(IList<CodeSwitchCase> list, int index)
    {
      if (index < 0 || list.Count <= index) return null;

      var current = list[index];

      //Default case
      if (current.Condition == null) return current.Statement;

      CodeStatement next = new CodeIfStatement(
        new CodeBinaryOperator { Left = new CodeVariableReference("#switch_" + _sid), Right = current.Condition, Type = OperatorType.Eq },
        current.Statement,
        BuildNextIf(list, index + 1));

      return next;
    }


    #endregion
  }
}
