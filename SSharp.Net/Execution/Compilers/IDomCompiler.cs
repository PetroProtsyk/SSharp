using Scripting.SSharp.Parser;

namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using Scripting.SSharp.Parser.Ast;

  public interface IDomCompiler : ICompiler<AstNode, CodeProgram, CodeObject>
  {
  }
}
