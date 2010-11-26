using System;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Parser.Ast;
using Scripting.SSharp.Runtime.Promotion;

namespace Scripting.SSharp.Processing
{
  internal class FunctionDeclarationVisitor : IPostProcessing
  {
    private Script _script;

    #region IAstVisitor Members

    public void BeginVisit(AstNode node)
    {
      var definition = node as ScriptFunctionDefinition;
      if (definition == null || string.IsNullOrEmpty(definition.Name)) return;

      definition._owner = _script;
      _script.Context.SetItem(definition.Name, definition);

      EventBroker.RegisterFunction(definition, _script);
    }

    public void EndVisit(AstNode node)
    {
    }

    #endregion

    #region IPostProcessing Members

    public void BeginProcessing(Script script)
    {
      _script = script;
    }

    public void EndProcessing(Script script)
    {
      if (_script != script) throw new InvalidOperationException();
      _script = null;
    }

    #endregion
  }
}
