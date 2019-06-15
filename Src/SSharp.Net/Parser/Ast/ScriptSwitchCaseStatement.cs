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
  internal class ScriptSwitchCaseStatement : ScriptStatement
  {
    private readonly ScriptExpr _cond;
    private readonly ScriptStatement _statement;

    public ScriptExpr Condition { get { return _cond; } }
    public ScriptStatement Statement { get { return _statement; } }

    public ScriptSwitchCaseStatement(AstNodeArgs args)
        : base(args)
    {
      _cond = ChildNodes[1] as ScriptExpr;
      _statement = ChildNodes[3] as ScriptStatement;
    }

    public override void Evaluate(IScriptContext context)
    {
      var switchValue = context.Result;     
      _cond.Evaluate(context);
      if (switchValue.Equals(context.Result))
      {
        _statement.Evaluate(context);
        context.SetBreak(true);
      }
      else
        context.Result = switchValue;
    }
  }
}
