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
  internal class ScriptIfStatement : ScriptStatement
  {
    private readonly ScriptCondition _condition;
    private readonly ScriptStatement _statement;
    private readonly ScriptStatement _elseStatement;

    public ScriptCondition Condition { get { return _condition; } }
    public ScriptStatement Statement { get { return _statement; } }
    public ScriptStatement ElseStatement { get { return _elseStatement; } }

    public ScriptIfStatement(AstNodeArgs args)
        : base(args)
    {
      _condition = (ScriptCondition) ChildNodes[1];
      _statement = (ScriptStatement)ChildNodes[2];
      //Else exists
      if (ChildNodes.Count == 4 && ChildNodes[3].ChildNodes.Count == 2 && ChildNodes[3].ChildNodes[1] is ScriptStatement)
      {
        _elseStatement = (ScriptStatement)ChildNodes[3].ChildNodes[1];
      }
    }

    public override void Evaluate(IScriptContext context)
    {
      _condition.Evaluate(context);
      if ((bool)context.Result)
      {       
        _statement.Evaluate(context);
      }
      else
        if (_elseStatement != null)
        {
          _elseStatement.Evaluate(context);
        }
    }
  }
}