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
  /// Scriptable object that can be used to rename properties from the given instance
  /// </summary>
  public class MemberRename : IScriptable
  {
    private readonly IMemberBinding _oldMember;
    private string _original;
    private readonly string _newName;

    public MemberRename(object instance, string original, string newName)
    {
      Instance = instance;
      _oldMember = RuntimeHost.Binder.BindToMember(instance, original, true);
      if (_oldMember == null)
        throw new ScriptIdNotFoundException(original);

      _newName = newName;
      _original = original;
    }

    #region IScriptable
    [Promote(false)]
    public object Instance
    {
      get;
      private set;
    }

    [Promote(false)]
    public IMemberBinding GetMember(string name, params object[] arguments)
    {
      return name == _newName ? _oldMember : null;
    }

    [Promote(false)]
    public IBinding GetMethod(string name, params object[] arguments)
    {
      return null;
    }

    #endregion
  }
}
