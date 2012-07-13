using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptElements : ScriptExpr, IEnumerable<ScriptAst>
  {
    public ScriptElements(AstNodeArgs args)
      : base(args)
    {

    }

    //TODO: Implement Breaking, Returning, etc. here
    public override void Evaluate(IScriptContext context)
    {
      if (ChildNodes.Count == 0) return;

      int index = 0;
      while (index < ChildNodes.Count)
      {
        ScriptAst node = (ScriptAst)ChildNodes[index];
        node.Evaluate(context);

        if (context.IsBreak() || context.IsReturn() || context.IsContinue())
        {
          break;
        }

        index++;
      }
    }

    #region IEnumerable<ScriptAst> Members

    public IEnumerator<ScriptAst> GetEnumerator()
    {
      return ChildNodes.OfType<ScriptAst>().GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ChildNodes.GetEnumerator();
    }

    #endregion
  }
}
