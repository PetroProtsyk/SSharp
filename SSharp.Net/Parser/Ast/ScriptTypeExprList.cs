using System;
using System.Linq;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Script Array Expression List Expression
  /// </summary>
  internal class ScriptTypeExprList : ScriptExpr
  {
    internal ScriptTypeExpr[] ExprList
    {
      get
      {
        return ChildNodes.OfType<ScriptTypeExpr>().ToArray();
      }
    }
  
    public ScriptTypeExprList(AstNodeArgs args)
        : base(args)
    {
    }

    public override void Evaluate(IScriptContext context)
    {
      var listObjects = new Type[ExprList.Length];
      for (var i = 0; i < ExprList.Length; i++)
      {
        ExprList[i].Evaluate(context);
        listObjects[i] = (Type)context.Result;
      }
      context.Result = listObjects;
    }
  }
}
