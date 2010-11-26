using System.Collections.Generic;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{  
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptSwitchStatement : ScriptStatement
  {
    private readonly List<ScriptSwitchCaseStatement> _cases;
    private readonly ScriptSwitchDefaultStatement _defaultCase;

    public List<ScriptSwitchCaseStatement> Cases { get { return _cases; } }
    public ScriptSwitchDefaultStatement DefaultCase { get { return _defaultCase; } }

    public ScriptSwitchStatement(AstNodeArgs args)
        : base(args)
    {
      _cases = new List<ScriptSwitchCaseStatement>();
      foreach (ScriptSwitchCaseStatement caseStatement in ChildNodes[0].ChildNodes)
      {
        _cases.Add(caseStatement);
      }
      if (ChildNodes.Count == 2)
        _defaultCase = ChildNodes[1] as ScriptSwitchDefaultStatement;
    }

    public override void Evaluate(IScriptContext context)
    {
      foreach (ScriptSwitchCaseStatement caseStatement in _cases)
      {
        caseStatement.Evaluate(context);
        if (context.IsBreak() || context.IsReturn())
        {
          context.SetBreak(false);
          return;
        }
      }

      if (_defaultCase != null)
        _defaultCase.Evaluate(context);
    }

  }
}
