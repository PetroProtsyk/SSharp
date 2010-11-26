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
