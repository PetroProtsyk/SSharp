using System;
using System.Collections.Generic;
using Scripting.SSharp.Parser;
using Scripting.SSharp.Runtime.Promotion;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptMObject : ScriptExpr
  {
    private List<ScriptMObjectPart> objectParts;

    public ScriptMObject(AstNodeArgs args)
      : base(args)
    {
      objectParts = new List<ScriptMObjectPart>();

      if (ChildNodes[0] is ScriptMObjectPart)
      {
        ScriptMObjectPart part = (ScriptMObjectPart)ChildNodes[0];
        objectParts.Add(part);
      }
      else
      {
        foreach (ScriptMObjectPart part in ChildNodes[0].ChildNodes)
        {
          objectParts.Add(part);
        }
      }
    }

    public override void Evaluate(IScriptContext context)
    {
      string typeName = RuntimeHost.GetSettingsItem("ScriptableObjectType") as string;
      Type mobjectType = typeof(Expando);
      if (!string.IsNullOrEmpty(typeName))
        mobjectType = RuntimeHost.GetType(typeName);

      IScriptable mobject = RuntimeHost.Activator.CreateInstance(mobjectType) as IScriptable;

      foreach (ScriptMObjectPart part in objectParts)
      {
        part.Evaluate(context);
        object[] rez = (object[])context.Result;

        mobject.GetMember((string)rez[0], null).SetValue(rez[1]);
      }

      context.Result = mobject;
    }
  }
}