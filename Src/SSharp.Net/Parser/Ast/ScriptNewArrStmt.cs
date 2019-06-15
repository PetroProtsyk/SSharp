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
  internal class ScriptNewArrStmt : ScriptExpr
  {
    private readonly ScriptTypeExpr _constrExpr;
    private readonly ScriptArrayResolution _arrRank;

    public ScriptNewArrStmt(AstNodeArgs args)
      : base(args)
    {
      _constrExpr = ChildNodes[1] as ScriptTypeExpr;
      _arrRank = ChildNodes[2] as ScriptArrayResolution;
    }

    //TODO: Refactor
    public override void Evaluate(IScriptContext context) {
        _constrExpr.Evaluate(context);
        var type = (Type)context.Result;

        _arrRank.Evaluate(context);
        var rank = Scripting.SSharp.CustomFunctions.ArrayFunc.FunctionDefinition.Invoke(null, (object[])context.Result);
        //(int)Convert.ChangeType(((object[])context.Result)[0], typeof(int), CultureInfo.CurrentCulture.NumberFormat);

        context.Result = Array.CreateInstance(type, (int[])rank);

        //long[] longRank = rank as long[];
        //if (longRank != null) {
        //    context.Result = Array.CreateInstance(type, longRank);
        //    return;
        //}
    }
  }
}
