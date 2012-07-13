namespace Scripting.SSharp.Execution.Compilers
{
  using Compilers.Dom;
  using VM;

  public interface IVMCompiler : ICompiler<CodeObject, ExecutableMachine, ExecutableMachine>
  {
    ExecutableMachine Compile(CodeObject code, ExecutableMachine machine);
  }
}
