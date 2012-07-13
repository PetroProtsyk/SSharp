using System;
using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptAstTemplate : ScriptAst
  {
    public ScriptAstTemplate(AstNodeArgs args)
      : base(args)
    {

    }

    public override void Evaluate(IScriptContext context)
    {
      throw new NotSupportedException();
    }

  }
}
