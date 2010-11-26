using System.Collections.Generic;

namespace Scripting.SSharp.Parser.FastGrammar
{
  internal class ParserState
  {
    public ParserState(long id)
    {
      Id = id;
    }

    public ActionsRecord Actions = new ActionsRecord();

    public long Id { get; private set; }
  }

  internal class ActionsRecord : Dictionary<string, ActionRecord>
  {
 
  }

  //internal class ActionsRecord : FastDictionary<ActionRecord> //Dictionary<string, ActionRecord>
  //{
  //  public ActionsRecord(KeyValuePair<int, KeyValuePair<string, ActionRecord>[]>[] values) :
  //    base(values)
  //  {
  //  }
  //}
}
