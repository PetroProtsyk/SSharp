namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using VM;

  [CompilerType(typeof(CodeBlockStatement))]
  internal class CodeBlockStatementCompiler : IVMCompiler
  {
    #region IVMCompiler Members

    public ExecutableMachine Compile(CodeObject code, ExecutableMachine machine)
    {
      var blockStatement = (CodeBlockStatement)code;

      foreach (var statement in blockStatement.Statements)
      {
        CodeDomCompiler.Compile(statement, machine);
      }

      return machine;
    }

    #endregion
  }
}
