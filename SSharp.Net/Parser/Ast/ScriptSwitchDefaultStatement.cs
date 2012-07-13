using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Script Array Constructor Expression
  /// </summary>
  internal class ScriptSwitchDefaultStatement : ScriptStatement
  {
    private ScriptStatement statement;
    public ScriptStatement Statement { get { return statement; } }

    public ScriptSwitchDefaultStatement(AstNodeArgs args)
        : base(args)
    {
      statement = ChildNodes[2] as ScriptStatement;
    }

    public override void Evaluate(IScriptContext context)
    {      
      statement.Evaluate(context);      
    }
  }
}