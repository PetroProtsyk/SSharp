namespace Scripting.SSharp.Parser.Ast
{
  internal struct AstNodeArgs
  {
    internal IGrammarTerm Term;
    public SourceSpan Span;
    public AstNodeList ChildNodes;
   
    internal AstNodeArgs(IGrammarTerm term, SourceSpan span, AstNodeList childNodes)
    {
      Term = term;
      Span = span;
      ChildNodes = childNodes;
    }
  }
}
