using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Script Array Constructor Expression
  /// </summary>
  internal class ScriptIfStatement : ScriptStatement
  {
    private readonly ScriptCondition _condition;
    private readonly ScriptStatement _statement;
    private readonly ScriptStatement _elseStatement;

    public ScriptCondition Condition { get { return _condition; } }
    public ScriptStatement Statement { get { return _statement; } }
    public ScriptStatement ElseStatement { get { return _elseStatement; } }

    public ScriptIfStatement(AstNodeArgs args)
        : base(args)
    {
      _condition = (ScriptCondition) ChildNodes[1];
      _statement = (ScriptStatement)ChildNodes[2];
      //Else exists
      if (ChildNodes.Count == 4 && ChildNodes[3].ChildNodes.Count == 2 && ChildNodes[3].ChildNodes[1] is ScriptStatement)
      {
        _elseStatement = (ScriptStatement)ChildNodes[3].ChildNodes[1];
      }
    }

    public override void Evaluate(IScriptContext context)
    {
      _condition.Evaluate(context);
      if ((bool)context.Result)
      {       
        _statement.Evaluate(context);
      }
      else
        if (_elseStatement != null)
        {
          _elseStatement.Evaluate(context);
        }
    }
  }
}