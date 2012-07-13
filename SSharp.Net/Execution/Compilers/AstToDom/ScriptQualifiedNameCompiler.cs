using System;
using System.Collections.Generic;
using Scripting.SSharp.Parser;

namespace Scripting.SSharp.Execution.Compilers
{
  using Scripting.SSharp.Parser.Ast;
  using Dom;

  [CompilerType(typeof(ScriptQualifiedName))]
  class ScriptQualifiedNameCompiler : IDomCompiler
  {
    #region IDomCompiler Members

    public CodeObject Compile(AstNode syntaxNode, CodeProgram prog)
    {
      ScriptQualifiedName syntax = (ScriptQualifiedName)syntaxNode;

      if (syntax.IsVariable)
      {
        CodeVariableReference code = new CodeVariableReference(syntax.Identifier);
        code.SourceSpan = syntaxNode.Span;

        return code;
      }
      else
      {
        List<CodeObject> modifiers = new List<CodeObject> ();

        foreach (ScriptAst modifier in syntax.Modifiers)
          modifiers.Add(AstDomCompiler.Compile(modifier, prog));

        CodeObjectReference objectReference;

        if (syntax.NextFirst)
        {
          CodeVariableReference variable = (CodeVariableReference)AstDomCompiler.Compile(syntax.NextPart, prog);

          objectReference = new CodeObjectReference(variable.Id,
              new CodeObjectReference(syntax.Identifier, null, modifiers),
              new CodeObject[0]);
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

      throw new NotSupportedException();
    }

    #endregion
  }
}
