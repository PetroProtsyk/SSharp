using System.Collections.Generic;

namespace Scripting.SSharp.Execution.Compilers.Dom
{
  internal class CodeObjectFunctionCall : CodeObject
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
