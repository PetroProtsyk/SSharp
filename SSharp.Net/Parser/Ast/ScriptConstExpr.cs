using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Constant Expression
  /// </summary>
  internal class ScriptConstExpr : ScriptExpr
  {
    /// <summary>
    /// Value of the constant
    /// </summary>
    public object Value { get; set; }

    protected internal override bool IsConst {
        get {
            return true;
        }
    }

    public ScriptConstExpr(AstNodeArgs args)
        : base(args)
    {
      var cons = (TokenAst)ChildNodes[0];
      Value = cons.Value;

      if (Value.Equals("true")) Value = true;
      if (Value.Equals("false")) Value = false;
      if (Value.Equals("null")) Value = null;
    }

    public override void Evaluate(IScriptContext context)
    {
      context.Result = Value;
    }
  }
}