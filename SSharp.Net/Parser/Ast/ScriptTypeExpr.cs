using System;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Script Array Constructor Expression
  /// </summary>
  internal class ScriptTypeExpr : ScriptExpr
  {
    private readonly ScriptGenericsPostfix _genericsPostfix;
    private readonly ScriptTypeExpr _typeExpr;
    private readonly string _identifier;

    public ScriptTypeExpr(AstNodeArgs args)
      : base(args)
    {
      if (ChildNodes.Count == 2 && ChildNodes[1].ChildNodes.Count == 0)
      {
        _identifier = ((TokenAst)ChildNodes[0]).Text;
      }
      else
        if (ChildNodes[0] is ScriptTypeExpr)
        {
          _typeExpr = ChildNodes[0] as ScriptTypeExpr;
          _identifier = ((TokenAst) ChildNodes[2].ChildNodes[0]).Text;
          _genericsPostfix = ChildNodes[2].ChildNodes[1] as ScriptGenericsPostfix;
        }
        else
        {
          _genericsPostfix = (ScriptGenericsPostfix)ChildNodes[1];
          _identifier = _genericsPostfix.GetGenericTypeName(((TokenAst)ChildNodes[0]).Text);
        }
    }

    private static string EvaluateName(ScriptTypeExpr expr)
    {
      return expr._typeExpr != null ? EvaluateName(expr._typeExpr) + "." + expr._identifier : expr._identifier;
    }

    public override void Evaluate(IScriptContext context)
    {
      if (_typeExpr == null && _genericsPostfix == null)
      {
        context.Result = RuntimeHost.GetType(_identifier);
        return;
      }
     
      if (_typeExpr != null)
      {
        var name = string.Format("{0}.{1}", EvaluateName(_typeExpr), _identifier);
        Type type;

        if (_genericsPostfix != null)
        {
          var genericType = RuntimeHost.GetType(_genericsPostfix.GetGenericTypeName(name));
          _genericsPostfix.Evaluate(context);
          type = genericType.MakeGenericType((Type[])context.Result);
        }
        else
        {
          type = RuntimeHost.GetType(name);
        }

        context.Result = type;
      }
      else
      {
        Type genericType = RuntimeHost.GetType(_identifier);
        _genericsPostfix.Evaluate(context);
        context.Result = genericType.MakeGenericType((Type[])context.Result);
      }
    }
  }
}