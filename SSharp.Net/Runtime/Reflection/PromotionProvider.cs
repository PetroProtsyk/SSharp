using System;
using System.Reflection;
using System.Collections.Generic;

namespace Scripting.SSharp.Runtime.Reflection
{
  internal class PromotionProvider
  {
    private static readonly Type TargetAttributeType = typeof(PromoteAttribute);
    private static readonly Dictionary<MemberInfo, PromoteAttribute> Cache = new Dictionary<MemberInfo, PromoteAttribute>();

    public static bool IsPromoted(MemberInfo member)
    {    
      PromoteAttribute attribute = null;
      if (Cache.TryGetValue(member, out attribute))
        return attribute.Promote;

      attribute = (PromoteAttribute)Attribute.GetCustomAttribute(member, TargetAttributeType) ?? PromoteAttribute.Yes;

      Cache.Add(member, attribute);
      return attribute.Promote;
    }
  }
}
