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

using System.Linq;

namespace Scripting.SSharp.Execution
{
  using Compilers;
  using Compilers.Dom;
  using Parser.Ast;

  /// <summary>
  /// Transforms AST tree to Code DOM tree
  /// </summary>
  internal class AstDomCompiler : BaseCompiler<AstNode, CodeProgram, CodeObject, IDomCompiler>
  {
    public static CodeProgram Compile(ScriptAst syntaxTree)
    {
      var syntaxProgram = syntaxTree as ScriptProg;
      var domProgram = new CodeProgram { SourceSpan = syntaxTree.Span };

      if (syntaxProgram != null)
      {
        foreach (var result in syntaxProgram.Elements.Select(element => Compile(element, domProgram)).OfType<CodeStatement>())
          domProgram.Statements.Add(result);
      }

      return domProgram;
    }

    static AstDomCompiler()
    {
      Register<ScriptStatementCompiler>();
      Register<ScriptAssignExprCompiler>();
      Register<ScriptConstExprCompiler>();
      Register<ScriptBinExprCompiler>();
      Register<ScriptTypeConvertExprCompiler>();
      Register<ScriptFlowControlStatementComiler>();
      Register<ScriptIfStatementCompiler>();
      Register<ScriptQualifiedNameCompiler>();
      Register<ScriptWhileStatementCompiler>();
      Register<ScriptSwitchStatementCompiler>();
      Register<ScriptCompoundStatementCompiler>();
      Register<ScriptForStatementCompiler>();
      Register<ScriptForEachStatementCompiler>();
      Register<ScriptFunctionCallCompiler>();     
    }
  }
}
