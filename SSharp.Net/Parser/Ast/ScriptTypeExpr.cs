using System;
using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Script Array Constructor Expression
  /// </summary>
  internal class ScriptTypeExpr : ScriptExpr
  {
    private ScriptGenericsPostfix GenericsPostfix;
    private ScriptTypeExpr TypeExpr;
    private string Identifier;

    public ScriptTypeExpr(AstNodeArgs args)
      : base(args)
    {
      if (ChildNodes.Count == 2 && ChildNodes[1].ChildNodes.Count == 0)
      {
        Identifier = ((TokenAst)ChildNodes[0]).Text;
      }
      else
        if (ChildNodes[0] is ScriptTypeExpr)
        {
          TypeExpr = ChildNodes[0] as ScriptTypeExpr;
          Identifier = (ChildNodes[2].ChildNodes[0] as TokenAst).Text;
          GenericsPostfix = ChildNodes[2].ChildNodes[1] as ScriptGenericsPostfix;
        }
        else
        {
          GenericsPostfix = (ScriptGenericsPostfix)ChildNodes[1];
          Identifier = GenericsPostfix.GetGenericTypeName(((TokenAst)ChildNodes[0]).Text);
        }
    }

    private string EvaluateName(ScriptTypeExpr expr)
    {
      if (expr.TypeExpr != null)
      {
        return EvaluateName(expr.TypeExpr) + "." + expr.Identifier;
      }
      else
        return expr.Identifier;
    }

    public override void Evaluate(IScriptContext context)
    {
      if (TypeExpr == null && GenericsPostfix == null)
      {
        context.Result = RuntimeHost.GetType(Identifier);
        return;
      }
     
      if (TypeExpr != null)
      {
        string name = string.Format("{0}.{1}", EvaluateName(TypeExpr), Identifier);
        Type type = null;

        if (GenericsPostfix != null)
        {
          Type genericType = RuntimeHost.GetType(GenericsPostfix.GetGenericTypeName(name));
          GenericsPostfix.Evaluate(context);
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
        Type genericType = RuntimeHost.GetType(Identifier);
        GenericsPostfix.Evaluate(context);
        context.Result = genericType.MakeGenericType((Type[])context.Result);
      }
    }
  }
}