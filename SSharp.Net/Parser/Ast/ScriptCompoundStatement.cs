using System.Collections.Generic;
using System.Linq;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{  
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptCompoundStatement : ScriptStatement
  {
    public IEnumerable<ScriptAst> Statements
    {
      get
      {
        return ChildNodes.OfType<ScriptAst>().ToArray();
      }
    }

    public bool ShouldCreateScope
    {
      get;
      internal set;
    }

    public ScriptCompoundStatement(AstNodeArgs args)
      : base(args)
    {
      ShouldCreateScope = true;
    }

    //TODO: Refactor
    public override void Evaluate(IScriptContext context)
    {
      if (ChildNodes.Count == 0) return;

      //Create local scope
      if (ShouldCreateScope)
      {
        IScriptScope scope = RuntimeHost.ScopeFactory.Create(ScopeTypes.Local, context.Scope);
        context.CreateScope(scope);
      }

      try
      {
        int index = 0;
        while (index < ChildNodes.Count)
        {
          var node = (ScriptAst)ChildNodes[index];
          node.Evaluate(context);

          if (context.IsBreak() || context.IsReturn() || context.IsContinue())
          {
            break;
          }

          index++;
        }
      }
      finally
      {
        if (ShouldCreateScope)
          context.RemoveLocalScope();
      }
    }
  }
}
