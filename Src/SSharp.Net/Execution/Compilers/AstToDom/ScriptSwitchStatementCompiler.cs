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
using Scripting.SSharp.Parser.Ast;

namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;

  [CompilerType(typeof(ScriptSwitchRootStatement))]
  internal class ScriptSwitchStatementCompiler : IDomCompiler
  {
    #region IDomCompiler Members

    public CodeObject Compile(AstNode syntaxNode, CodeProgram prog)
    {
      var syntax = (ScriptSwitchRootStatement)syntaxNode;

      var condition = AstDomCompiler.Compile<CodeExpression>(syntax.Expression, prog);

      var cases = new List<CodeSwitchCase> ();
      foreach (var statement in syntax.Switch.Cases)
      {
        cases.Add(new CodeSwitchCase(
          AstDomCompiler.Compile<CodeExpression>(statement.Condition, prog),
          AstDomCompiler.Compile<CodeStatement>(statement.Statement, prog)));
      }
      
      if (syntax.Switch.DefaultCase != null)
      {
        cases.Add(new CodeSwitchCase(
            null,
            AstDomCompiler.Compile<CodeStatement>(syntax.Switch.DefaultCase.Statement, prog)));
      }

      var code = new CodeSwitchStatement(condition, cases);
      return code;
    }

    #endregion
  }
}
