using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// For statement
  /// </summary>
  internal class ScriptForStatement : ScriptStatement
  {
    private ScriptExpr init;
    private ScriptExpr cond;
    private ScriptExpr next;
    private ScriptStatement statement;

    public ScriptExpr Init { get { return init; } }
    public ScriptExpr Condition { get { return cond; } }
    public ScriptExpr Next { get { return next; } }
    public ScriptStatement Statement { get { return statement; } }

    public ScriptForStatement(AstNodeArgs args)
        : base(args)
    {
      init = (ScriptExpr)args.ChildNodes[1];
      cond = (ScriptExpr)args.ChildNodes[2];
      next = (ScriptExpr)args.ChildNodes[3];
      statement = (ScriptStatement)args.ChildNodes[4];

      ScriptCompoundStatement body = statement as ScriptCompoundStatement;
      if (body != null)
        body.ShouldCreateScope = false;
    }

    public override void Evaluate(IScriptContext context)
    {
      bool condBool;
      object result = RuntimeHost.NullValue;

      //Create local scope
      IScriptScope scope = RuntimeHost.ScopeFactory.Create(ScopeTypes.Local, context.Scope);
      context.CreateScope(scope);

      try
      {
        init.Evaluate(context);
        cond.Evaluate(context);
        condBool = context.Result == null ? true : (bool)context.Result;

        while (condBool)
        {
          statement.Evaluate(context);
          result = context.Result;

          if (context.IsBreak() || context.IsReturn())
          {
            context.SetBreak(false);
            break;
          }

          if (context.IsContinue())
          {
            context.SetContinue(false);
          }


          next.Evaluate(context);
          cond.Evaluate(context);
          condBool = context.Result == null ? true : (bool)context.Result;
        }

        context.Result = result;
      }
      finally
      {
        context.RemoveLocalScope();
      }
    }
  }
}
