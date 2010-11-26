using System.Collections.Generic;

namespace Scripting.SSharp.Execution.Compilers.Dom
{
  internal abstract class CodeObject
  {
    protected readonly List<CodeObject> Children = new List<CodeObject>();

    public object SourceSpan { get; set; }
  }
}
