using System;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptTryCatchFinallyStatement : ScriptExpr
  {
    private readonly ScriptStatement _tryBlock;
    private readonly ScriptStatement _catchBlock;
    private readonly ScriptStatement _finallyBlock;
    private readonly string _expName;

    public ScriptTryCatchFinallyStatement(AstNodeArgs args)
        : base(args)
    {
      _tryBlock = ChildNodes[1] as ScriptStatement;
      _expName = ((TokenAst) ChildNodes[3]).Text;
      _catchBlock = ChildNodes[4] as ScriptStatement;
      _finallyBlock = ChildNodes[6] as ScriptStatement;
    }

    public override void Evaluate(IScriptContext context)
    {
      try
      {
        _tryBlock.Evaluate(context);
      }
      catch(Exception e)
      {
        context.SetItem(_expName, e);
        _catchBlock.Evaluate(context);
      }
      finally
      {
        _finallyBlock.Evaluate(context);
      }
    }
  }
}
