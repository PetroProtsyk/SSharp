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
  /// Script Array Constructor Expression
  /// </summary>
  internal class ScriptWhileStatement : ScriptStatement
  {
    public ScriptCondition Condition { get; private set; }
    public ScriptStatement Statement { get; private set; }

    public ScriptWhileStatement(AstNodeArgs args)
        : base(args)
    {
      Condition = args.ChildNodes[1] as ScriptCondition;
      Statement = args.ChildNodes[2] as ScriptStatement;
    }

    public override void Evaluate(IScriptContext context)
    {
      Condition.Evaluate(context);
      object lastResult = RuntimeHost.NullValue;

      while ((bool)context.Result)
      {
        Statement.Evaluate(context);
        lastResult = context.Result;

        if (context.IsBreak() || context.IsReturn())
        {
          context.SetBreak(false);
          break;
        }

        if (context.IsContinue())
        {
          context.SetContinue(false);
        }

        Condition.Evaluate(context);
      }

      context.Result = lastResult;
    }
  }
}