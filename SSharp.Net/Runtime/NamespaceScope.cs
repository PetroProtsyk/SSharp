/*
 * Copyright © 2011, Petro Protsyk, Denys Vuika
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *  http://www.apache.org/licenses/LICENSE-2.0
 *  
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Scripting.SSharp.Runtime.Promotion;

namespace Scripting.SSharp.Runtime
{
  public class NamespaceScope : ScriptScope
  {
    private string _name;
    private HashSet<string> _names = new HashSet<string>();

    internal const string NameFormat = "{0}_{1}";

    public NamespaceScope(IScriptScope parent, string name) :
        base(parent)
    {
      if (parent == null) throw new ArgumentNullException("parent");
      if (name == null) throw new ArgumentNullException("usingObject");
      _name = name;
    }

    public string FormatId(string id)
    {
      return string.Format(NameFormat, _name, id);
    }

    protected override object GetVariableInternal(string id, bool searchHierarchy)
    {
      if (HasVariable(id))
        return Parent.GetItem(FormatId(id), true);

      return base.GetVariableInternal(id, searchHierarchy);
    }

    public override bool HasVariable(string id)
    {
      return _names.Contains(id);
    }

    public override void SetItem(string id, object value)
    {
      if (base.GetVariableInternal(id, true)!=RuntimeHost.NoVariable)
      {
        base.SetItem(id, value);
      }
      else
      {
        string n_id = FormatId(id);
        Parent.SetItem(n_id, value);
        _names.Add(id);
      }
    }

    public override IValueReference Ref(string id)
    {
      if (HasVariable(id))
        return Parent.Ref(FormatId(id));
      else
        return Parent.Ref(id);
    }

  }

  public class NamespaceScopeActivator : IScopeActivator
  {
    #region IScopeActivator Members

    public IScriptScope Create(IScriptScope parent, params object[] args)
    {
      if (args.Length == 1)
        return new NamespaceScope(parent, (string)args[0]);

      throw new NotSupportedException();
    }

    #endregion
  }

}
