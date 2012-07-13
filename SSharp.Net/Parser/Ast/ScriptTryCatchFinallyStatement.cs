using System;
using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptTryCatchFinallyStatement : ScriptExpr
  {
    private ScriptStatement tryBlock;
    private ScriptStatement catchBlock;
    private ScriptStatement finallyBlock;
    private string expName;

    public ScriptTryCatchFinallyStatement(AstNodeArgs args)
        : base(args)
    {
      tryBlock = ChildNodes[1] as ScriptStatement;
      expName = (ChildNodes[3] as TokenAst).Text;
      catchBlock = ChildNodes[4] as ScriptStatement;
      finallyBlock = ChildNodes[6] as ScriptStatement;
    }

    public override void Evaluate(IScriptContext context)
    {
      try
      {
        tryBlock.Evaluate(context);
      }
      catch(Exception e)
      {
        context.SetItem(expName, e);
        catchBlock.Evaluate(context);
      }
      finally
      {
        finallyBlock.Evaluate(context);
      }
    }
  }
}
