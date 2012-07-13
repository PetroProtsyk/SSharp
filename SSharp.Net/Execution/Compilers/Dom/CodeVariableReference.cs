using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripting.SSharp.Execution.Compilers.Dom
{
  public class CodeVariableReference : CodeExpression
  {
    public string Id { get; private set; }

    public CodeVariableReference(string id)
    {
      Id = id;
    }
  }
}
