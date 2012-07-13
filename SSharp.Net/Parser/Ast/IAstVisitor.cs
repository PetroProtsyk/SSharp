namespace Scripting.SSharp.Parser.Ast
{
  public interface IAstVisitor
  {
    void BeginVisit(AstNode node);
    void EndVisit(AstNode node);
  }
}