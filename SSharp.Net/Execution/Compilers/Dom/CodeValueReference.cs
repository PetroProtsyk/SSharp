using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripting.SSharp.Execution.Compilers.Dom
{
  public class CodeValueReference : CodeExpression
  {
    public object Value { get; private set; }

    public CodeValueReference(object value)
    {
      Value = value;
    }
  }
}
