using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripting.SSharp.Execution.Compilers.Dom
{
  public class CodeObjectFunctionCall : CodeObject
  {
    public List<CodeExpression> Parameters { get; private set; }

    public CodeObjectFunctionCall(IEnumerable<CodeExpression> parameters)
    {
      Parameters = new List<CodeExpression>();
      if (parameters != null)
        Parameters.AddRange(parameters);
    }
  }
}
