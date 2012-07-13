using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptMetaExpr : ScriptExpr, IInvokable
  {
    private ScriptAst metaProg;

    public ScriptMetaExpr(AstNodeArgs args)
      : base(args)
    {
      AstNodeArgs progArgs = new AstNodeArgs();
      progArgs.ChildNodes = new AstNodeList();
      progArgs.ChildNodes.Add(ChildNodes[1]);
      progArgs.Span = args.Span;
      progArgs.Term = args.Term;
      
      metaProg = new ScriptProg(progArgs);
      metaProg.Parent = this;
    }

    public override void Evaluate(IScriptContext context)
    {
      context.Result = metaProg;
    }

    #region IInvokable Members

    public bool CanInvoke()
    {
      return true;
    }

    public object Invoke(IScriptContext context, object[] args)
    {
      metaProg.Evaluate(context);
      return context.Result;
    }

    #endregion
  }
}