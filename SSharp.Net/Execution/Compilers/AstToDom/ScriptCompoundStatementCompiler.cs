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

using Scripting.SSharp.Parser.Ast;

namespace Scripting.SSharp.Execution.Compilers
{
  using Dom;

  [CompilerType(typeof(ScriptCompoundStatement))]
  internal class ScriptCompoundStatementCompiler : IDomCompiler
  {
    #region IDomCompiler Members

    public CodeObject Compile(AstNode syntaxNode, CodeProgram prog)
    {
      var syntax = (ScriptCompoundStatement)syntaxNode;

      var block = new CodeBlockStatement();

      foreach (var syntaxStatement in syntax.Statements)
        block.Statements.Add(
          (CodeStatement)AstDomCompiler.Compile(syntaxStatement, prog));

      return block;
    }

    #endregion
  }
}
