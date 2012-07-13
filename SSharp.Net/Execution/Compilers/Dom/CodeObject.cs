using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripting.SSharp.Execution.Compilers.Dom
{
  public abstract class CodeObject
  {
    protected readonly List<CodeObject> Children = new List<CodeObject>();

    public object SourceSpan { get; set; }
  }
}
