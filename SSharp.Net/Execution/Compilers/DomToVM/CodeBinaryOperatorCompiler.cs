using System.Collections.Generic;

namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using VM;
  using VM.Operations;

  [CompilerType(typeof(CodeBinaryOperator))]
  public class CodeBinaryOperatorCompiler : IVMCompiler
  {
    private static Dictionary<OperatorType, string> mapping = new Dictionary<OperatorType, string>()
    {
      {OperatorType.Plus, "+" },
      {OperatorType.Minus,"-" },
      {OperatorType.Mul,"*" },
      {OperatorType.Div,"/" },
      {OperatorType.Mod,"%" },
      {OperatorType.Pow, "^"},

      {OperatorType.Greater,">" },
      {OperatorType.Less,"<" },
      {OperatorType.GreaterEq,">=" },
      {OperatorType.LessEq,"<=" },
      {OperatorType.Eq,"==" },
      {OperatorType.Neq,"!=" }
    };

    #region IVMCompiler Members

    public ExecutableMachine Compile(CodeObject code, ExecutableMachine machine)
    {
      CodeBinaryOperator codeExpression = (CodeBinaryOperator)code;

      CodeDomCompiler.Compile(codeExpression.Right, machine);
      PushOperation op = machine.CreateOperation<PushOperation>();
      op.SourceObject = codeExpression.Right;

      CodeDomCompiler.Compile(codeExpression.Left, machine);
      op = machine.CreateOperation<PushOperation>();
      op.SourceObject = codeExpression.Left;

      Operation sop = null;
      switch (codeExpression.Type)
      {
        case OperatorType.Plus:
          sop = machine.CreateOperation<AddOperation>();
          break;
        case OperatorType.Minus:
          sop = machine.CreateOperation<SubOperation>();
          break;
        case OperatorType.Mul:
          sop = machine.CreateOperation<MulOperation>();
          break;
        case OperatorType.Mod:
          sop = machine.CreateOperation<ModOperation>();
          break;
        case OperatorType.Div:
          sop = machine.CreateOperation<DivOperation>();
          break;
        default:
          GenericOperation gop = machine.CreateOperation<GenericOperation>();
          gop.Symbol = mapping[codeExpression.Type];
          sop = gop;
          break;
      }
      sop.SourceObject = codeExpression;

      PopOperation pop = machine.CreateOperation<PopOperation>();
      pop.SourceObject = codeExpression;

      return machine;
    }

    #endregion
  }
}
