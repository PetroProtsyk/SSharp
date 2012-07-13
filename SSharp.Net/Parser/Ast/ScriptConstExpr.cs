using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Constant Expression
  /// </summary>
  internal class ScriptConstExpr : ScriptExpr
  {
    private object m_value;

    /// <summary>
    /// Value of the constant
    /// </summary>
    public object Value
    {
      get { return m_value; }
      set { m_value = value; }
    }

    public ScriptConstExpr(AstNodeArgs args)
        : base(args)
    {
      TokenAst cons = (TokenAst)ChildNodes[0];
      Value = cons.Value;

      if (Value.Equals("true")) Value = true;
      if (Value.Equals("false")) Value = false;
      if (Value.Equals("null")) Value = null;
    }

    public override void Evaluate(IScriptContext context)
    {
      context.Result = m_value;
    }
  }
}