using System;
using System.Collections;
using Scripting.SSharp.Runtime.Promotion;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// ForEachStatement
  /// </summary>
  internal class ScriptForEachStatement : ScriptStatement
  {
    private readonly TokenAst _name;
    private readonly ScriptExpr _expr;
    private readonly ScriptStatement _statement;

    public string Id
    {
      get { return _name.Text; }
    }

    public ScriptExpr Container
    {
      get { return _expr; }
    }

    public ScriptStatement Statement
    {
      get { return _statement; }
    }

    public ScriptForEachStatement(AstNodeArgs args)
      : base(args)
    {
      _name = (TokenAst)ChildNodes[1];
      _expr = (ScriptExpr)ChildNodes[3];
      _statement = (ScriptStatement)ChildNodes[4];
    }

    public override void Evaluate(IScriptContext context)
    {
      _expr.Evaluate(context);

      if (context.Result == null) throw new NullReferenceException(string.Format(Strings.NullReferenceInCode, Code(context)));

      var enumeration = context.Result as IEnumerable;
      IEnumerator enumerator = null;

      if (enumeration != null)
      {
        enumerator = enumeration.GetEnumerator();
      }
      else
      {
        IBinding bind = RuntimeHost.Binder.BindToMethod(context.Result, "GetEnumerator", new Type[0], new object[0]);
        if (bind != null)
          enumerator = bind.Invoke(context, null) as IEnumerator;
      }

      if (enumerator == null)
        throw new ScriptExecutionException(string.Format(Strings.ForEachMethodNotFound, context.Result));

      enumerator.Reset();

      while (enumerator.MoveNext())
      {
        context.SetItem(_name.Text, enumerator.Current);
        _statement.Evaluate(context);
        if (context.IsBreak() || context.IsReturn())
        {
          context.SetBreak(false);
          break;
        }
        if (context.IsContinue())
        {
          context.SetContinue(false);
        }
      }

    }
  }
}
