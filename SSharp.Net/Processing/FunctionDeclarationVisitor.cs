using System;
using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Parser.Ast;
using Scripting.SSharp.Runtime.Promotion;

namespace Scripting.SSharp.Processing
{
  internal class FunctionDeclarationVisitor : IPostProcessing
  {
    private Script Script;

    public FunctionDeclarationVisitor()
    {
    }

    #region IAstVisitor Members

    public void BeginVisit(AstNode node)
    {
      ScriptFunctionDefinition definition = node as ScriptFunctionDefinition;
      if (definition != null && !string.IsNullOrEmpty(definition.Name))
      {
        Script.Context.SetItem(definition.Name, definition);

        if (RuntimeHost.ContextEnabledEvents)
          EventBroker.RegisterFunction(definition, Script);
      }
    }

    public void EndVisit(AstNode node)
    {
    }

    #endregion

    #region IPostProcessing Members

    public void BeginProcessing(Script script)
    {
      Script = script;
    }

    public void EndProcessing(Script script)
    {
      if (Script != script) throw new InvalidOperationException();
      Script = null;
    }

    #endregion
  }
}
