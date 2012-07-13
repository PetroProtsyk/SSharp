using Scripting.SSharp.Parser;
using Scripting.SSharp.Parser.Ast;

namespace Scripting.SSharp.Processing
{
  /// <summary>
  /// Processing procedures for script AST
  /// </summary>
  public interface IPostProcessing : IAstVisitor
  {
    void BeginProcessing(Script script);

    void EndProcessing(Script script);
  }
}
