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

  [CompilerType(typeof(CodeForEachStatement))]
  internal class CodeForEachStatementCompiler : IVMCompiler
  {
    #region IVMCompiler Members

    private static long _sid;

    public ExecutableMachine Compile(CodeObject code, ExecutableMachine machine)
    {
      var forStatement = (CodeForEachStatement)code;

      //foreach (i in c) statement ~
      //
      //f_ = c.GetEnumerator();
      //while(f_.Next()) {i = f_.Current; statement; }

      _sid++;
      string fName = "#ForEach_" + _sid;

      var fblock = new CodeBlockStatement();

      var f = new CodeAssignExpression(fName, forStatement.Container);
      var f1 = new CodeAssignExpression(fName, 
        new CodeObjectReference(fName, 
           new CodeObjectReference("GetEnumerator", null,
            new CodeObject[] { new CodeObjectFunctionCall(new CodeExpression[0]) }), new CodeObject[0]));

      var fwhileBlock = new CodeBlockStatement();

      var i = new CodeAssignExpression(forStatement.Id.Id,
        new CodeObjectReference(fName,
           new CodeObjectReference("get_Current", null,
            new CodeObject[] { new CodeObjectFunctionCall(new CodeExpression[0]) }), new CodeObject[0]));

      fwhileBlock.Statements.Add(new CodeExpressionStatement(i));
      fwhileBlock.Statements.Add(forStatement.Statement);

      var fwhile = new CodeWhileStatement(
        new CodeObjectReference(fName,
           new CodeObjectReference("MoveNext", null,
            new CodeObject[] { new CodeObjectFunctionCall(new CodeExpression[0]) }), new CodeObject[0]),
        fwhileBlock);

      fblock.Statements.Add(new CodeExpressionStatement(f));
      fblock.Statements.Add(new CodeExpressionStatement(f1));
      fblock.Statements.Add(fwhile);

      CodeDomCompiler.Compile(fblock, machine);

      return machine;
    }

    #endregion
  }
}
