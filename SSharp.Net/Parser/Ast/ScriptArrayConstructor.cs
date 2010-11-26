using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{  
  /// <summary>
  /// Script Array Constructor Expression
  /// </summary>
  internal class ScriptArrayConstructor : ScriptExpr
  {
    private readonly ScriptExprList _exprList;

    public ScriptArrayConstructor(AstNodeArgs args)
        : base(args)
    {
      _exprList = (ScriptExprList)ChildNodes[0];
    }

    public override void Evaluate(IScriptContext context)
    {      
      _exprList.Evaluate(context);
    }
  }
}

