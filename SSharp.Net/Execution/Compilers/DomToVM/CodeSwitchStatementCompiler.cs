using System.Collections.Generic;

namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using VM;

  [CompilerType(typeof(CodeSwitchStatement))]
  public class CodeSwitchStatementCompiler : IVMCompiler
  {
    #region IVMCompiler Members
    private static long sid = 0;

    public ExecutableMachine Compile(CodeObject code, ExecutableMachine machine)
    {
      CodeSwitchStatement codeSwitch = (CodeSwitchStatement)code;

      //switch(a) { case c1 : s1; case c2 : s2; default : s3 }
      //~
      //switch_expr = a;
      //if (a == c1) s1
      //else if (a == c2) s2
      //else s3

      sid++;

      CodeBlockStatement block = new CodeBlockStatement();
      block.SourceSpan = codeSwitch.SourceSpan;

      CodeAssignExpression switch_expr = new CodeAssignExpression(
        "#switch_"+sid, codeSwitch.Expression);
         
      CodeStatement ifStatement = BuildNextIf(codeSwitch.Cases, 0);

      block.Statements.Add(new CodeExpressionStatement(switch_expr));
      block.Statements.Add(ifStatement);

      CodeDomCompiler.Compile(block, machine);

      return machine;
    }

    private CodeStatement BuildNextIf(List<CodeSwitchCase> list, int index)
    {
      if (index < 0 || list.Count <= index) return null;

      CodeSwitchCase current = list[index];

      //Default case
      if (current.Condition == null) return current.Statement;

      CodeStatement next = new CodeIfStatement(
        new CodeBinaryOperator() { Left = new CodeVariableReference("#switch_" + sid), Right = current.Condition, Type = OperatorType.Eq },
        current.Statement,
        BuildNextIf(list, index + 1));

      return next;
    }


    #endregion
  }
}
