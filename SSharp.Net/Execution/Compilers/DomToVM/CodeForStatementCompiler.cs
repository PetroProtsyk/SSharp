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

namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;
  using VM;

  [CompilerType(typeof(CodeForStatement))]
  internal class CodeForStatementCompiler : IVMCompiler
  {
    #region IVMCompiler Members

    public ExecutableMachine Compile(CodeObject code, ExecutableMachine machine)
    {
      var forStatement = (CodeForStatement)code;

      //for (init; cond; next) statement ~
      //
      //init;
      //while(cond) { statement; next;}

      CodeDomCompiler.Compile(forStatement.Init, machine);

      var body = new CodeBlockStatement();
      body.Statements.Add(forStatement.Statement);
      body.Statements.Add(new CodeExpressionStatement(forStatement.Next));
      var newWhile = new CodeWhileStatement(forStatement.Condition, body);

      CodeDomCompiler.Compile(newWhile, machine);
      return machine;
    }

    #endregion
  }
}
