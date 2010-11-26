using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptGlobalList : ScriptAst
  {
    private ScriptFuncParameters Parameters { get; set; }

    public ScriptGlobalList(AstNodeArgs args)
      : base(args)
    {
      Parameters = ChildNodes[1] as ScriptFuncParameters;
    }

    public override void Evaluate(IScriptContext context)
    {
      if (Parameters != null)
        context.Result = Parameters.Identifiers.ToArray();
    }
  }
}
