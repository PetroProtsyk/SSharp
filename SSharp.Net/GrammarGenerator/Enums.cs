
namespace Scripting.SSharp.Parser
{
  internal enum TokenCategory
  {
    Content,
    Outline,
    Comment,
    Error,
  }

  internal enum TokenMatchMode
  {
    ByValue = 1,
    ByType = 2,
    ByValueThenByType = ByValue | ByType,
  }

  internal enum Associativity
  {
    Left,
    Right,
    Neutral
  }

  internal enum ParserActionType
  {
    Shift,
    Reduce,
    Operator
  }

  internal enum TermOptions
  {
    None = 0,
    IsOperator = 0x01,
    IsGrammarSymbol = 0x02,
    IsPunctuation = 0x20,
    IsList = 0x80,
    IsStarList = 0x100,
    IsNonGrammar = 0x0200,
  }
}
