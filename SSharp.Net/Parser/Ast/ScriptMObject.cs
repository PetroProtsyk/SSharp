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

using System.Collections.Generic;
using Scripting.SSharp.Runtime.Promotion;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptMObject : ScriptExpr
  {
    private readonly List<ScriptMObjectPart> _objectParts;

    public ScriptMObject(AstNodeArgs args)
      : base(args)
    {
      _objectParts = new List<ScriptMObjectPart>();

      if (ChildNodes[0] is ScriptMObjectPart)
      {
        var part = (ScriptMObjectPart)ChildNodes[0];
        _objectParts.Add(part);
      }
      else
      {
        foreach (ScriptMObjectPart part in ChildNodes[0].ChildNodes)
        {
          _objectParts.Add(part);
        }
      }
    }

    public override void Evaluate(IScriptContext context)
    {
      var typeName = RuntimeHost.GetSettingsItem("ScriptableObjectType") as string;
      var mobjectType = typeof(Expando);
      if (!string.IsNullOrEmpty(typeName))
        mobjectType = RuntimeHost.GetType(typeName);

      var mobject = RuntimeHost.Activator.CreateInstance(mobjectType) as IScriptable;

      foreach (var part in _objectParts)
      {
        part.Evaluate(context);
        var rez = (object[])context.Result;

        if (mobject != null) mobject.GetMember((string)rez[0], null).SetValue(rez[1]);
      }

      context.Result = mobject;
    }
  }
}