using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Root of any script program
  /// </summary>
  internal class ScriptProg : ScriptAst, IInvokable
  {
    internal ScriptElements Elements
    {
      get;
      set;
    }

    public ScriptProg(AstNodeArgs args)
      : base(args)
    {
      Elements = ChildNodes[0] as ScriptElements;
    }

    public override void Evaluate(IScriptContext context)
    {
      context.SetItem("Context", context);
      context.SetItem("prog", this);

      base.Evaluate(context);
    }

    #region IInvokable Members

    public bool CanInvoke()
    {
      return true;
    }

    public object Invoke(IScriptContext context, object[] args)
    {
      Evaluate(context);
      return context.Result;
    }

    #endregion
  }
}