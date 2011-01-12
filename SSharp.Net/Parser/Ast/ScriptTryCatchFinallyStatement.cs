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
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptTryCatchFinallyStatement : ScriptExpr
  {
    private readonly ScriptStatement _tryBlock;
    private readonly ScriptStatement _catchBlock;
    private readonly ScriptStatement _finallyBlock;
    private readonly string _expName;

    public ScriptTryCatchFinallyStatement(AstNodeArgs args)
        : base(args)
    {
      _tryBlock = ChildNodes[1] as ScriptStatement;
      _expName = ((TokenAst) ChildNodes[3]).Text;
      _catchBlock = ChildNodes[4] as ScriptStatement;
      _finallyBlock = ChildNodes[6] as ScriptStatement;
    }

    public override void Evaluate(IScriptContext context)
    {
      try
      {
        _tryBlock.Evaluate(context);
      }
      catch(Exception e)
      {
        context.SetItem(_expName, e);
        _catchBlock.Evaluate(context);
      }
      finally
      {
        _finallyBlock.Evaluate(context);
      }
    }
  }
}
