namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using Scripting.SSharp.Parser.Ast;

  internal interface IDomCompiler : ICompiler<AstNode, CodeProgram, CodeObject>
  {
  }
}
