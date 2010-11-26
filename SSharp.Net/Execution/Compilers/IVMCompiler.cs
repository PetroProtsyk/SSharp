namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using VM;

  internal interface IVMCompiler : ICompiler<CodeObject, ExecutableMachine, ExecutableMachine>
  {
    //ExecutableMachine Compile(CodeObject code, ExecutableMachine machine);
  }
}
