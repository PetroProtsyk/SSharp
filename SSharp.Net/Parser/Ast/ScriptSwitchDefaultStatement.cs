using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Script Array Constructor Expression
  /// </summary>
  internal class ScriptSwitchDefaultStatement : ScriptStatement
  {
    public ScriptStatement Statement { get; private set; }

    public ScriptSwitchDefaultStatement(AstNodeArgs args)
        : base(args)
    {
      Statement = ChildNodes[2] as ScriptStatement;
    }

    public override void Evaluate(IScriptContext context)
    {      
      Statement.Evaluate(context);      
    }
  }
}