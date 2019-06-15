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

namespace Scripting.SSharp.Execution.Compilers
{
  using Parser.Ast;
  using Dom;

  [CompilerType(typeof(ScriptQualifiedName))]
  internal class ScriptQualifiedNameCompiler : IDomCompiler
  {
    #region IDomCompiler Members

    public CodeObject Compile(AstNode syntaxNode, CodeProgram prog)
    {
      var syntax = (ScriptQualifiedName)syntaxNode;

      if (syntax.IsVariable)
      {
        var code = new CodeVariableReference(syntax.Identifier) {SourceSpan = syntaxNode.Span};
        return code;
      }

      var modifiers = syntax.Modifiers.Select(modifier => AstDomCompiler.Compile(modifier, prog)).ToList();

      CodeObjectReference objectReference;

      if (syntax.NextFirst)
      {
        var o = AstDomCompiler.Compile(syntax.NextPart, prog);
        var variable = o as CodeVariableReference;

        if (variable != null)
        {
          objectReference = new CodeObjectReference(
            variable.Id,
            new CodeObjectReference(syntax.Identifier, null, modifiers),
            new CodeObject[0]);

        }
        else
        {
          objectReference = (CodeObjectReference)o;
          ((CodeObjectReference)objectReference.Next).Next = new CodeObjectReference(syntax.Identifier, null, modifiers);
        }

      }
      else
      {
        objectReference = new CodeObjectReference(
          syntax.Identifier,
          AstDomCompiler.Compile(syntax.NextPart, prog),
          modifiers);
      }

      return objectReference;
    }

    #endregion
  }
}
