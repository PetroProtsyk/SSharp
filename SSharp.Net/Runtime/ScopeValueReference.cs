
namespace Scripting.SSharp.Runtime
{
  internal class ScopeValueReference
  {
    IScriptScope Scope { get; set; }
    public string Id { get; private set; }

    public object Value
    {
      get
      {
        return Scope.GetItem(Id, true);
      }
      set
      {
        Scope.SetItem(Id, value);
      }
    }

    public object ConvertedValue { get; set; }

    public ScopeValueReference(IScriptScope scope, string id)
    {
      Scope = scope;
      Id = id;
    }
  }
}
