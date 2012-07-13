using System.Collections.Generic;
using Scripting.SSharp.Parser;
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
      if (ChildNodes.Count == 1)
      {
        for (int index = 0; index < ChildNodes[0].ChildNodes.Count; index++)
        {
          AstNode astNode = ChildNodes[0].ChildNodes[index];
          Identifiers.Add((astNode as TokenAst).Text);
        }
      }
      
      //if (ChildNodes[0] is Token)
      //{
      //  Identifiers.Add((ChildNodes[0] as Token).Text);
      //}
      //else
      //{
      //  for (int index = 0; index < ChildNodes[0].ChildNodes.Count; index++)
      //  {
      //    AstNode astNode = ChildNodes[0].ChildNodes[index];
      //    Identifiers.Add((astNode as Token).Text);
      //  }
      //}
    }   

    public override void Evaluate(IScriptContext context)
    {
      if (context.Result == null) return;

      object[] paramVals = (object[])context.Result;

      for (int index=0; index < paramVals.Length; index++)
        if (index < Identifiers.Count)
        {
          context.SetItem(Identifiers[index], paramVals[index]);
        }

      context.Result = RuntimeHost.NullValue;
    }
  }
}