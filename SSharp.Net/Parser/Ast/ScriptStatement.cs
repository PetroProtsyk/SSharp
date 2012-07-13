using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripting.SSharp.Parser;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptStatement : ScriptAst, IEnumerable<ScriptAst>
  {
    public ScriptStatement(AstNodeArgs args)
      : base(args)
    {

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
