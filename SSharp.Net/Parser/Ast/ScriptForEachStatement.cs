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
using System.Collections;
using Scripting.SSharp.Runtime.Promotion;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// ForEachStatement
  /// </summary>
  internal class ScriptForEachStatement : ScriptStatement
  {
    private TokenAst _name;
    private ScriptExpr _expr;
    private ScriptStatement _statement;

    public string Id
    {
      get { return _name.Text; }
    }

    public ScriptExpr Container
    {
      get { return _expr; }
    }

    public ScriptStatement Statement
    {
      get { return _statement; }
    }

    public ScriptForEachStatement(AstNodeArgs args)
      : base(args)
    {
    }

    protected override void OnNodesReplaced() {
        base.OnNodesReplaced();

        _name = (TokenAst)ChildNodes[1];
        _expr = (ScriptExpr)ChildNodes[3];
        _statement = (ScriptStatement)ChildNodes[4];
    }

    public override void Evaluate(IScriptContext context)
    {
      _expr.Evaluate(context);

      if (context.Result == null) throw new NullReferenceException(string.Format(Strings.NullReferenceInCode, Code(context)));

      var enumeration = context.Result as IEnumerable;
      IEnumerator enumerator = null;

      if (enumeration != null)
      {
        enumerator = enumeration.GetEnumerator();
      }
      else
      {
        IBinding bind = RuntimeHost.Binder.BindToMethod(context.Result, "GetEnumerator", new Type[0], new object[0]);
        if (bind != null)
          enumerator = bind.Invoke(context, null) as IEnumerator;
      }

      if (enumerator == null)
        throw new ScriptExecutionException(string.Format(Strings.ForEachMethodNotFound, context.Result));

      enumerator.Reset();

      while (enumerator.MoveNext())
      {
        // TODO: Should be debug friendly
        context.SetItem(_name.Text, enumerator.Current);
        context.Result = enumerator.Current;

        _statement.Evaluate(context);
        if (context.IsBreak() || context.IsReturn())
        {
          context.SetBreak(false);
          break;
        }
        if (context.IsContinue())
        {
          context.SetContinue(false);
        }
      }

    }
  }
}
