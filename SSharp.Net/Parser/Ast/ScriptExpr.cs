using System;
using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Base Node for Expression
  /// </summary>
  internal class ScriptExpr : ScriptAst
  {
    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="args">arguments</param>
    public ScriptExpr(AstNodeArgs args)
      : base(args)
    {

    }

    internal protected static event EventHandler<HandleOperatorArgs> HandleOperator;

    protected static HandleOperatorArgs OnHandleOperator(object sender, IScriptContext context, string symbol, params object[] parameters)
    {
      HandleOperatorArgs args = new HandleOperatorArgs(context, symbol, parameters);

      if (HandleOperator != null)
        HandleOperator.Invoke(sender, args);

      return args;
    }

  }
}