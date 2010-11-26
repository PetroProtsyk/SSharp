using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptSwitchRootStatement : ScriptAst, IEnumerable<ScriptAst>
  {
    public ScriptExpr Expression { get { return (ScriptExpr)ChildNodes[1]; } }
    public ScriptSwitchStatement Switch { get { return (ScriptSwitchStatement)ChildNodes[2]; } }
    
    public ScriptSwitchRootStatement(AstNodeArgs args)
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
