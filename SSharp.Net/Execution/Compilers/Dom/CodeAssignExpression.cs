using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripting.SSharp.Execution.Compilers.Dom
{
  public class CodeAssignExpression : CodeExpression
  {
    public string Id { get; private set; }
    public CodeExpression RightExpression { get; private set; }

    public CodeAssignExpression(string id, CodeExpression rightExpression)
    {
      Id = id;
      RightExpression = rightExpression;
    }
  }
}
