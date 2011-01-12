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
using System;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptMetaExpr : ScriptExpr, IInvokable
  {
    private readonly ScriptAst _metaProg;

    public ScriptMetaExpr(AstNodeArgs args)
      : base(args)
    {
      var progArgs = new AstNodeArgs
      {
        ChildNodes = new AstNodeList { ChildNodes[1] },
        Span = args.Span,
        Term = args.Term
      };

      _metaProg = new ScriptProg(progArgs) { Parent = this };
    }

    public override void Evaluate(IScriptContext context)
    {
      context.Result = _metaProg;
    }

    #region IInvokable Members

    public bool CanInvoke()
    {
      return true;
    }

    public object Invoke(IScriptContext context, object[] args)
    {
      bool scopeOwner = false;
      
      if (args != null)
      {
        if (args.Length > 1) throw new ArgumentException("Number of arguments ");
        if (args.Length == 1)
        {
          var assigner = args[0] as ISupportAssign;
          if (assigner == null) throw new NotSupportedException("Given type of argument is not supported");
          assigner.AssignTo(context.CreateScope());
          scopeOwner = true;
        }
      }

      try
      {
        _metaProg.Evaluate(context);
        return context.Result;
      }
      finally
      {
        if (scopeOwner)
          context.RemoveLocalScope();
      }
    }

    #endregion
  }
}