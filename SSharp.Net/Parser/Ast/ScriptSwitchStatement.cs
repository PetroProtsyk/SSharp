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
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{  
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptSwitchStatement : ScriptStatement
  {
    private readonly List<ScriptSwitchCaseStatement> _cases;
    private readonly ScriptSwitchDefaultStatement _defaultCase;

    public List<ScriptSwitchCaseStatement> Cases { get { return _cases; } }
    public ScriptSwitchDefaultStatement DefaultCase { get { return _defaultCase; } }

    public ScriptSwitchStatement(AstNodeArgs args)
        : base(args)
    {
      _cases = new List<ScriptSwitchCaseStatement>();
      foreach (ScriptSwitchCaseStatement caseStatement in ChildNodes[0].ChildNodes)
      {
        _cases.Add(caseStatement);
      }
      if (ChildNodes.Count == 2)
        _defaultCase = ChildNodes[1] as ScriptSwitchDefaultStatement;
    }

    public override void Evaluate(IScriptContext context)
    {
      foreach (ScriptSwitchCaseStatement caseStatement in _cases)
      {
        caseStatement.Evaluate(context);
        if (context.IsBreak() || context.IsReturn())
        {
          context.SetBreak(false);
          return;
        }
      }

      if (_defaultCase != null)
        _defaultCase.Evaluate(context);
    }

  }
}
