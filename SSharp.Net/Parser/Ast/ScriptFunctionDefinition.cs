using System.Collections.Generic;
using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{  
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptFunctionDefinition : ScriptAst, IInvokable
  {
    public string Name { get; private set; }
    private ScriptFuncParameters Parameters;
    private ScriptFuncContract Contract;
    private ScriptGlobalList GlobalList;
    private ScriptCompoundStatement Body;
    private IScriptContext activeContext;

    public ScriptFunctionDefinition(AstNodeArgs args)
        : base(args)
    {
      TokenAst funcName = ChildNodes[1] as TokenAst;
      int index = 0;

      if (funcName != null)
      {
        Name = funcName.Text;
      }
      else
      //Function expression
      {
        Name = null;
        index = 1;
      }

      if (ChildNodes.Count == 5-index)
      {
        Contract = ChildNodes[3 - index] as ScriptFuncContract;
        Parameters = ChildNodes[3 - index] as ScriptFuncParameters;
      }

      if (ChildNodes.Count == 6 - index)
      {
        Parameters = ChildNodes[2 - index] as ScriptFuncParameters;
        GlobalList = ChildNodes[3 - index] as ScriptGlobalList;
        Contract = ChildNodes[4 - index] as ScriptFuncContract;
      }

      Body = (ScriptCompoundStatement)ChildNodes[ChildNodes.Count - 1];
      Body.ShouldCreateScope = false;
    }

    public override void Evaluate(IScriptContext context)
    {
      if (Name != null)
        context.SetItem(Name, this);

      context.Result = this;
    }

    #region IInvokable Members

    public bool CanInvoke()
    {
      return true;
    }

    //TODO: Review this approach
    public object ThreadInvoke(IScriptContext context)
    {
     return Invoke(context, null);
    }

    public object Invoke(IScriptContext context, object[] args)
    {
        activeContext = context;
        object result = RuntimeHost.NullValue;

        INotifyingScope functionScope = (INotifyingScope)RuntimeHost.ScopeFactory.Create(ScopeTypes.Function, context.Scope, context);
        context.CreateScope(functionScope);
      
        try
        {
          if (Parameters != null)
          {
            context.Result = args;
            Parameters.Evaluate(context);
          }

          functionScope.BeforeSetItem += ScopeBeforeSetItem;

          if (Contract != null)
          {
            functionScope.AfterSetItem += CheckContractInvariant;
            Contract.CheckPre(context);
            Contract.CheckInv(context);
          }

          context.Result = RuntimeHost.NullValue;
          Body.Evaluate(context);
          result = context.Result;

          if (Contract != null)
          {
            functionScope.AfterSetItem -= CheckContractInvariant;
            Contract.CheckInv(context);
            Contract.CheckPost(context);
          }
        }
        finally
        {
          context.RemoveLocalScope();
          context.SetBreak(false);
          context.SetContinue(false);
          context.SetReturn(false);
          context.Result = result;

          functionScope.BeforeSetItem -= ScopeBeforeSetItem;
          activeContext = null;
        }

        return result;
    }

    private void ScopeBeforeSetItem(IScriptScope sender, ScopeArgs args)
    {
      //TODO: Performance improvement. Should be evaluated once per function call
      List<string> globalNames = GetGlobalNames(activeContext);

      if (globalNames.Contains(args.Name))
      {
        ScriptQualifiedName.SetToParentScope(sender.Parent, args.Name, args.Value);        
        args.Cancel = true;
      }

      //if (!sender.HasVariable(args.Name))
      //{
      //  args.Cancel = SetToParentScope(sender.Parent, args.Name, args.Value);
      //}
    }

    private void CheckContractInvariant(object sender, ScopeArgs args)
    {
      Contract.CheckInv(activeContext);
    }

    private List<string> GetGlobalNames(IScriptContext context)
    {
      List<string> globalNames = new List<string>();
      if (GlobalList != null)
      {
        GlobalList.Evaluate(context);
        globalNames = new List<string>((string[]) context.Result);
      }

      return globalNames;
    }
    #endregion

    public override string ToString()
    {
      return Name == null ? "_anonimous_func" : "_func:" + Name;
    }
  }
}