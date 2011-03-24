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

using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// For statement
  /// </summary>
  internal class ScriptForStatement : ScriptStatement
  {
    private ScriptExpr _init;
    private ScriptExpr _cond;
    private ScriptExpr _next;
    private ScriptStatement _statement;

    public ScriptExpr Init { get { return _init; } }
    public ScriptExpr Condition { get { return _cond; } }
    public ScriptExpr Next { get { return _next; } }
    public ScriptStatement Statement { get { return _statement; } }

    public ScriptForStatement(AstNodeArgs args)
        : base(args)
    {

    }

    protected override void OnNodesReplaced() {
        _init = (ScriptExpr)ChildNodes[1];
        _cond = (ScriptExpr)ChildNodes[2];
        _next = (ScriptExpr)ChildNodes[3];
        _statement = (ScriptStatement)ChildNodes[4];

        var body = _statement as ScriptCompoundStatement;
        if (body != null)
            body.ShouldCreateScope = false;
    }

    public override void Evaluate(IScriptContext context)
    {
      var result = RuntimeHost.NullValue;

      //Create local scope
      var scope = RuntimeHost.ScopeFactory.Create(ScopeTypes.Local, context.Scope);
      context.CreateScope(scope);

      try
      {
        _init.Evaluate(context);
        _cond.Evaluate(context);
        bool condBool = context.Result == null ? true : (bool)context.Result;

        while (condBool)
        {
          _statement.Evaluate(context);
          result = context.Result;

          if (context.IsBreak() || context.IsReturn())
          {
            context.SetBreak(false);
            break;
          }

          if (context.IsContinue())
          {
            context.SetContinue(false);
          }


          _next.Evaluate(context);
          _cond.Evaluate(context);
          condBool = context.Result == null ? true : (bool)context.Result;
        }

        context.Result = result;
      }
      finally
      {
        context.RemoveLocalScope();
      }
    }
  }
}
