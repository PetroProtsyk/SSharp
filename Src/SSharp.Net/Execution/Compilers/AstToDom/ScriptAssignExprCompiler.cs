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
  using Parser.Ast;
  
  [CompilerType(typeof(ScriptAssignExpr))]
  internal class ScriptAssignExprCompiler : IDomCompiler
  {
    #region IDomCompiler Members

    public CodeObject Compile(AstNode syntaxNode, CodeProgram prog)
    {
      var syntaxAssignExpr = (ScriptAssignExpr)syntaxNode;
      
      if (syntaxAssignExpr.LeftExpression.IsVariable)
      {
        if (syntaxAssignExpr.Symbol == "++")
        {
          var binary = new CodeBinaryOperator
          {
            Left = new CodeVariableReference(syntaxAssignExpr.LeftExpression.Identifier),
            Right = new CodeValueReference(1),
            Type = OperatorType.Plus
          };

          var codeAssign = new CodeAssignExpression(
            syntaxAssignExpr.LeftExpression.Identifier,
            binary) {SourceSpan = syntaxNode.Span};

          return codeAssign;
        }
        else
          if (syntaxAssignExpr.Symbol == "--")
          {
            var binary = new CodeBinaryOperator
            {
              Left = new CodeVariableReference(syntaxAssignExpr.LeftExpression.Identifier),
              Right = new CodeValueReference(1),
              Type = OperatorType.Minus
            };

            var codeAssign = new CodeAssignExpression(
              syntaxAssignExpr.LeftExpression.Identifier,
              binary) {SourceSpan = syntaxNode.Span};

            return codeAssign;
          }
          else
          {
            var codeAssign = new CodeAssignExpression(
              syntaxAssignExpr.LeftExpression.Identifier,
              (CodeExpression)AstDomCompiler.Compile(syntaxAssignExpr.RightExpression, prog))
            {SourceSpan = syntaxNode.Span};

            return codeAssign;
          }
      }

      return null;
    }

    #endregion
  }
}
