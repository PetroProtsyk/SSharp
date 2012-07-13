using System;
using System.Collections.Generic;

namespace Scripting.SSharp.Runtime
{
  public class LocalScope : ScriptScope
  {
    public LocalScope(IScriptScope parent):
      base(parent)
    {
      if (parent == null)
        throw new NotSupportedException("Can't create stand-alone local scope");
    }

    public override void SetItem(string id, object value)
    {
      if (HasVariable(id))
        base.SetItem(id, value);
      else
        Parent.SetItem(id, value);
    }

    public void CreateVariable(string id, object value)
    {
      base.SetItem(id, value);
    }
  }
}
