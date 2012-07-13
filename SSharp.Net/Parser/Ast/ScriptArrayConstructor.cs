using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{  
  /// <summary>
  /// Script Array Constructor Expression
  /// </summary>
  internal class ScriptArrayConstructor : ScriptExpr
  {
    private ScriptExprList exprList;

    public ScriptArrayConstructor(AstNodeArgs args)
        : base(args)
    {
      exprList = (ScriptExprList)ChildNodes[0];
    }

    public override void Evaluate(IScriptContext context)
    {      
      exprList.Evaluate(context);
    }
  }
}

