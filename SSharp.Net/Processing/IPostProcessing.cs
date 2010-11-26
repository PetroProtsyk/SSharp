using Scripting.SSharp.Parser.Ast;

namespace Scripting.SSharp.Processing
{
  /// <summary>
  /// Processing procedures for script AST
  /// </summary>
  internal interface IPostProcessing : IAstVisitor
  {
    void BeginProcessing(Script script);

    void EndProcessing(Script script);
  }
}
