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
