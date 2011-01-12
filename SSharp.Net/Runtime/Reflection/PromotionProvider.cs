/*
 * Copyright © 2011, Petro Protsyk, Denys Vuika
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *  http://www.apache.org/licenses/LICENSE-2.0
 *  
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
      PromoteAttribute attribute;
      if (Cache.TryGetValue(member, out attribute))
        return attribute.Promote;

      attribute = (PromoteAttribute)Attribute.GetCustomAttribute(member, TargetAttributeType) ?? PromoteAttribute.Yes;

      Cache.Add(member, attribute);
      return attribute.Promote;
    }
  }
}
