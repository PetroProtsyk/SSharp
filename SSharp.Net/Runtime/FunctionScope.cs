namespace Scripting.SSharp.Runtime
{
  /// <summary>
  /// Scope with contracts on variables
  /// </summary>
  public class FunctionScope : ScriptScope, INotifyingScope
  {
    #region Fields
    public bool SearchHierarchy { get; set; }
    #endregion

    #region Constructors
    /// <summary>
    /// Default Constructor
    /// </summary>
    public FunctionScope(IScriptScope parent)
      : base(parent)
    {
    }
    #endregion

    #region Methods
    public override object GetItem(string id, bool throwException)
    {
      var args = new ScopeArgs(id, RuntimeHost.NullValue);

      OnBeforeGetItem(args);
      if (args.Cancel)
      {
        if (args.Value != RuntimeHost.NullValue)
          return args.Value;

        throw new ScriptIdNotFoundException(id);
      }
      args.Value = base.GetItem(id, throwException);

      OnAfterGetItem(args);
      if (args.Cancel)
      {
        throw new ScriptIdNotFoundException(string.Format(Strings.IdProcessingCanceledByUser, id));
      }

      return args.Value;
    }

    public override void SetItem(string id, object value)
    {
      var args = new ScopeArgs(id, value);

      OnBeforeSetItem(args);    
      if (args.Cancel) return;

      base.SetItem(id, args.Value);

      OnAfterSetItem(args);
      if (args.Cancel)
      {
        throw new ScriptIdNotFoundException(string.Format(Strings.IdProcessingCanceledByUser, id));
      }
    }

    public override IValueReference Ref(string id)
    {
      return null;
    }
    #endregion

    #region INotifyingScope Members

    protected virtual void OnBeforeGetItem(ScopeArgs args)
    {
      if (BeforeGetItem != null)
        BeforeGetItem(this, args);
    }

    protected virtual void OnAfterGetItem(ScopeArgs args)
    {
      if (AfterGetItem != null)
        AfterGetItem(this, args);
    }

    protected virtual void OnBeforeSetItem(ScopeArgs args)
    {
      if (BeforeSetItem!=null)
        BeforeSetItem(this, args);
    }

    protected virtual void OnAfterSetItem(ScopeArgs args)
    {
      if (AfterSetItem != null)
        AfterSetItem(this, args);
    }

    /// <summary>
    /// Event raised before performing getting item, allowing to
    /// cancel action or replace actual value
    /// </summary>
    public event ScopeSetEvent BeforeGetItem;

    /// <summary>
    /// Raised after performing get item action, allowing to replace
    /// resulting value or cancel action. Cancelling will raise ScriptException
    /// </summary>
    public event ScopeSetEvent AfterGetItem;

    /// <summary>
    /// Event raised before performing setting item action, allowing to
    /// cancel it or replace actual value
    /// </summary>
    public event ScopeSetEvent BeforeSetItem;

    /// <summary>
    /// Raised after performing set item action, allowing to cancel action. 
    /// Cancelling will raise ScriptException    
    /// </summary>
    public event ScopeSetEvent AfterSetItem;

    #endregion

    protected override void Cleanup()
    {
      try
      {
      }
      finally
      {
        base.Cleanup();
      }
    }
  }

  /// <summary>
  /// Default activator for a FunctionScope. May be replaced in xml file configuration
  /// by custom implementation
  /// </summary>
  public class FunctionScopeActivator : IScopeActivator
  {
    #region IScopeActivator Members

    /// <summary>
    /// Creates a new Function scope
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="args">arguments are ignored</param>
    /// <returns>a new instance  of FunctionScope</returns>
    public IScriptScope Create(IScriptScope parent, params object[] args)
    {
      return new FunctionScope(parent);
    }

    #endregion
  }


}
