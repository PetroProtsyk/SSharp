namespace Scripting.SSharp.Parser.Ast
{
  internal interface IAstVisitor
  {
    void BeginVisit(AstNode node);
    void EndVisit(AstNode node);
  }
}