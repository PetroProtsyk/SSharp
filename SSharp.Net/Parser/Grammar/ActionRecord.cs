namespace Scripting.SSharp.Parser.FastGrammar
{
  internal class ActionRecord
  {
    public ActionRecord(ParserActionType actionType, ParserState newState, NonTerminal nonTerminal, int popCount)
    {
      ActionType = actionType;
      NewState = newState;
      NonTerminal = nonTerminal;
      PopCount = popCount;
    }

    public ParserActionType ActionType = ParserActionType.Shift;
    public ParserState NewState;

    public NonTerminal NonTerminal
    {
      get;
      private set;
    }

    public int PopCount
    {
      get;
      private set;
    }
  }
}
