using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripting.SSharp.Parser.FastGrammar
{
  internal class ActionRecord
  {
    public ActionRecord(ParserActionType ActionType, ParserState NewState, NonTerminal NonTerminal, int PopCount)
    {
      this.ActionType = ActionType;
      this.NewState = NewState;
      this.NonTerminal = NonTerminal;
      this.PopCount = PopCount;
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
