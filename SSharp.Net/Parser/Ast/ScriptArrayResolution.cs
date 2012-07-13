using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptArrayResolution : ScriptAst
  {
    private ScriptExprList args;
    private static readonly object Empty = new object[0];

    public ScriptArrayResolution(AstNodeArgs args)
        : base(args)
    {
      if (args.ChildNodes.Count != 0)
        this.args = args.ChildNodes[0] as ScriptExprList;
    }

    public override void Evaluate(IScriptContext context)
    {
      if (args == null)
      {
        context.Result = Empty;
        return;
      }
      args.Evaluate(context);
    }
  }
}