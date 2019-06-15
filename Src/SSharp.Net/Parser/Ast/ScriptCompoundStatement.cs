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
using System.Linq;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{  
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptCompoundStatement : ScriptStatement
  {
    public IEnumerable<ScriptAst> Statements
    {
      get
      {
        return ChildNodes.OfType<ScriptAst>().ToArray();
      }
    }

    public bool ShouldCreateScope
    {
      get;
      internal set;
    }

    public ScriptCompoundStatement(AstNodeArgs args)
      : base(args)
    {
      ShouldCreateScope = true;
    }

    //TODO: Refactor
    public override void Evaluate(IScriptContext context)
    {
      if (ChildNodes.Count == 0) return;

      //Create local scope
      if (ShouldCreateScope)
      {
        IScriptScope scope = RuntimeHost.ScopeFactory.Create(ScopeTypes.Local, context.Scope);
        context.CreateScope(scope);
      }

      try
      {
        int index = 0;
        while (index < ChildNodes.Count)
        {
          var node = (ScriptAst)ChildNodes[index];
          node.Evaluate(context);

          if (context.IsBreak() || context.IsReturn() || context.IsContinue())
          {
            break;
          }

          index++;
        }
      }
      finally
      {
        if (ShouldCreateScope)
          context.RemoveLocalScope();
      }
    }
  }
}
