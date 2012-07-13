using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripting.SSharp.Parser.FastGrammar
{
  internal class ParserState
  {
    public ParserState(long id)
    {
      ID = id;
    }

    public ActionsRecord Actions = new ActionsRecord();

    public long ID { get; private set; }
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
