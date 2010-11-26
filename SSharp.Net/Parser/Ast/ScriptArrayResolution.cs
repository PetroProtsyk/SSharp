using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptArrayResolution : ScriptAst
  {
    private readonly ScriptExprList _args;
    private static readonly object Empty = new object[0];

    public ScriptArrayResolution(AstNodeArgs args)
        : base(args)
    {
      if (args.ChildNodes.Count != 0)
        _args = args.ChildNodes[0] as ScriptExprList;
    }

    public override void Evaluate(IScriptContext context)
    {
      if (_args == null)
      {
        context.Result = Empty;
        return;
      }
      _args.Evaluate(context);
    }
  }
}