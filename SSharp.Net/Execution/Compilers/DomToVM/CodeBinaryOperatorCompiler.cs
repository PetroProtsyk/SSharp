using System.Collections.Generic;

namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using VM;
  using VM.Operations;

  [CompilerType(typeof(CodeBinaryOperator))]
  internal class CodeBinaryOperatorCompiler : IVMCompiler
  {
    private static readonly Dictionary<OperatorType, string> Mapping = new Dictionary<OperatorType, string>
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
      var codeExpression = (CodeBinaryOperator)code;

      CodeDomCompiler.Compile(codeExpression.Right, machine);
      var op = machine.CreateOperation<PushOperation>();
      op.SourceObject = codeExpression.Right;

      CodeDomCompiler.Compile(codeExpression.Left, machine);
      op = machine.CreateOperation<PushOperation>();
      op.SourceObject = codeExpression.Left;

      Operation sop;
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
          var gop = machine.CreateOperation<GenericOperation>();
          gop.Symbol = Mapping[codeExpression.Type];
          sop = gop;
          break;
      }
      sop.SourceObject = codeExpression;

      var pop = machine.CreateOperation<PopOperation>();
      pop.SourceObject = codeExpression;

      return machine;
    }

    #endregion
  }
}
