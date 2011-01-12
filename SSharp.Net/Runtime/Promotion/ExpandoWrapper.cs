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


namespace Scripting.SSharp.Runtime.Promotion
{
  /// <summary>
  /// Wraps object, allows to create new fields and access members form inner object
  /// Note: ExpandoWrapper does not support generic methods
  /// </summary>
  public class ExpandoWrapper : Expando
  {
    private readonly object _instance;

    public ExpandoWrapper(object instance)
    {
      _instance = instance;
    }

    [Promote(false)]
    public override object Instance
    {
      get
      {
        return _instance;
      }
    }

    [Promote(false)]
    public override IMemberBinding GetMember(string name, params object[] arguments)
    {
      IMemberBinding bind = RuntimeHost.Binder.BindToMember(_instance, name, true);
      if (bind != null)
        return bind;

      return base.GetMember(name, arguments);
    }

    [Promote(false)]
    public override IBinding GetMethod(string name, params object[] arguments)
    {
      IBinding bind = RuntimeHost.Binder.BindToMethod(_instance, name, null, arguments);
      if (bind != null)
        return bind;

      return base.GetMethod(name, arguments);
    }
  }
}
