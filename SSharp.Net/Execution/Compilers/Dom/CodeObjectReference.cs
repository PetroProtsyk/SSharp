using System.Collections.Generic;

namespace Scripting.SSharp.Execution.Compilers.Dom
{
  internal class CodeObjectReference : CodeExpression
  {
    public string Id { get; set; }

    public CodeObject Next { get; set; }

    public List<CodeObject> Modifiers { get; private set; }

    public CodeObjectReference(string id, CodeObject next, IEnumerable<CodeObject> modifiers)
    {
      Id = id;
      Modifiers = new List<CodeObject>();
      if (modifiers != null)
        Modifiers.AddRange(modifiers);
      Next = next;
    }
  }
}
