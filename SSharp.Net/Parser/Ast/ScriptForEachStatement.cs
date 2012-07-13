using System;
using System.Collections;
using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime.Promotion;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// ForEachStatement
  /// </summary>
  internal class ScriptForEachStatement : ScriptStatement
  {
    private TokenAst name;
    private ScriptExpr expr;
    private ScriptStatement statement;

    public string Id
    {
      get
      {
        return name.Text;
      }
    }

    public ScriptExpr Container
    {
      get
      {
        return expr;
      }
    }

    public ScriptStatement Statement
    {
      get
      {
        return statement;
      }
    }

    public ScriptForEachStatement(AstNodeArgs args)
        : base(args)
    {
      name = (TokenAst)ChildNodes[1];
      expr = (ScriptExpr)ChildNodes[3];
      statement = (ScriptStatement)ChildNodes[4];
    }

    public override void Evaluate(IScriptContext context)
    {
      expr.Evaluate(context);
      
      IEnumerable enumeration = context.Result as IEnumerable;
      IEnumerator enumerator = null;
     
      if (enumeration != null)
      {
        enumerator = enumeration.GetEnumerator();
      }
      else
      {
        IBinding bind = RuntimeHost.Binder.BindToMethod(context.Result, "GetEnumerator",new Type[0], new object[0]);
        if (bind != null)
          enumerator = bind.Invoke(context, null) as IEnumerator;
      }

      if (enumerator == null)
        throw new ScriptException("GetEnumerator() method did not found in object: " + context.Result.ToString());

      enumerator.Reset();
      
      while(enumerator.MoveNext())
      {
        context.SetItem(name.Text, enumerator.Current);
        statement.Evaluate(context);
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
