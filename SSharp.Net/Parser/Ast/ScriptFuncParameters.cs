using System.Collections.Generic;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptFuncParameters : ScriptExpr
  {
    internal readonly List<string> Identifiers = new List<string>();

    public ScriptFuncParameters(AstNodeArgs args)
      : base(args)
    {
      if (ChildNodes.Count != 1) return;

      foreach (var astNode in ChildNodes[0].ChildNodes)
      {
        Identifiers.Add(((TokenAst) astNode).Text);
      }
    }

    public override void Evaluate(IScriptContext context)
    {
      if (context.Result == null) return;

      var paramVals = (object[])context.Result;

      for (var index=0; index < paramVals.Length; index++)
        if (index < Identifiers.Count)
        {
          context.SetItem(Identifiers[index], paramVals[index]);
        }

      context.Result = RuntimeHost.NullValue;
    }
  }
}