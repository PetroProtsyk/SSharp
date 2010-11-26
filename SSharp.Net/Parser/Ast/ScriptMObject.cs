using System.Collections.Generic;
using Scripting.SSharp.Runtime.Promotion;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptMObject : ScriptExpr
  {
    private readonly List<ScriptMObjectPart> _objectParts;

    public ScriptMObject(AstNodeArgs args)
      : base(args)
    {
      _objectParts = new List<ScriptMObjectPart>();

      if (ChildNodes[0] is ScriptMObjectPart)
      {
        var part = (ScriptMObjectPart)ChildNodes[0];
        _objectParts.Add(part);
      }
      else
      {
        foreach (ScriptMObjectPart part in ChildNodes[0].ChildNodes)
        {
          _objectParts.Add(part);
        }
      }
    }

    public override void Evaluate(IScriptContext context)
    {
      var typeName = RuntimeHost.GetSettingsItem("ScriptableObjectType") as string;
      var mobjectType = typeof(Expando);
      if (!string.IsNullOrEmpty(typeName))
        mobjectType = RuntimeHost.GetType(typeName);

      var mobject = RuntimeHost.Activator.CreateInstance(mobjectType) as IScriptable;

      foreach (var part in _objectParts)
      {
        part.Evaluate(context);
        var rez = (object[])context.Result;

        if (mobject != null) mobject.GetMember((string)rez[0], null).SetValue(rez[1]);
      }

      context.Result = mobject;
    }
  }
}