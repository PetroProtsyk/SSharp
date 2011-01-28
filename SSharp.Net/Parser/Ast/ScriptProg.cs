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
  /// Root of any script program
  /// </summary>
  internal class ScriptProg : ScriptAst, IInvokable
  {
    internal ScriptElements Elements
    {
      get;
      set;
    }

    public ScriptProg(AstNodeArgs args)
      : base(args)
    {
      Elements = ChildNodes[0] as ScriptElements;
    }

    public override void Evaluate(IScriptContext context)
    {
      context.SetItem("Context", context);
      context.SetItem("prog", this);
      //Reset flags in context
      context.ResetControlFlags();

      base.Evaluate(context);

      context.ResetControlFlags();
    }

    #region IInvokable Members

    public bool CanInvoke()
    {
      return true;
    }

    public object Invoke(IScriptContext context, object[] args)
    {
      Evaluate(context);
      return context.Result;
    }

    #endregion
  }
}