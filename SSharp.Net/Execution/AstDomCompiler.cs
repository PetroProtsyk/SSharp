using Scripting.SSharp.Parser;

namespace Scripting.SSharp.Execution
{
  using Compilers;
  using Compilers.Dom;
  using Scripting.SSharp.Parser.Ast;

  /// <summary>
  /// Transforms AST tree to Code DOM tree
  /// </summary>
  public class AstDomCompiler : BaseCompiler<AstNode, CodeProgram, CodeObject, IDomCompiler>
  {
    public static CodeProgram Compile(ScriptAst syntaxTree)
    {
      ScriptProg syntaxProgram = syntaxTree as ScriptProg;
      CodeProgram domProgram = new CodeProgram() { SourceSpan = syntaxTree.Span };

      foreach (ScriptAst element in syntaxProgram.Elements)
      {
        CodeObject result = Compile(element, domProgram);

        if (result is CodeStatement)
          domProgram.Statements.Add((CodeStatement)result);
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
