using Scripting.SSharp.Runtime;
using System;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptMetaExpr : ScriptExpr, IInvokable
  {
    private readonly ScriptAst _metaProg;

    public ScriptMetaExpr(AstNodeArgs args)
      : base(args)
    {
      var progArgs = new AstNodeArgs
      {
        ChildNodes = new AstNodeList { ChildNodes[1] },
        Span = args.Span,
        Term = args.Term
      };

      _metaProg = new ScriptProg(progArgs) { Parent = this };
    }

    public override void Evaluate(IScriptContext context)
    {
      context.Result = _metaProg;
    }

    #region IInvokable Members

    public bool CanInvoke()
    {
      return true;
    }

    public object Invoke(IScriptContext context, object[] args)
    {
      bool scopeOwner = false;
      
      if (args != null)
      {
        if (args.Length > 1) throw new ArgumentException("Number of arguments ");
        if (args.Length == 1)
        {
          var assigner = args[0] as ISupportAssign;
          if (assigner == null) throw new NotSupportedException("Given type of argument is not supported");
          assigner.AssignTo(context.CreateScope());
          scopeOwner = true;
        }
      }

      try
      {
        _metaProg.Evaluate(context);
        return context.Result;
      }
      finally
      {
        if (scopeOwner)
          context.RemoveLocalScope();
      }
    }

    #endregion
  }
}