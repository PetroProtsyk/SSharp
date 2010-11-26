using System.Collections.Generic;
using System.Dynamic;

namespace Scripting.SSharp.Runtime
{
  public sealed class RuntimeObject : DynamicObject
  {
    private Dictionary<string, object> fields = new Dictionary<string, object>();

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
      // Converting the property name to lowercase so that property names become case-insensitive.
      string name = binder.Name.ToLower();

      // If the property name is found in a dictionary,
      // set the result parameter to the property value and return true.
      // Otherwise, return false.        
      return fields.TryGetValue(name, out result);
    }

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
      // Converting the property name to lowercase
      // so that property names become case-insensitive.
      fields[binder.Name.ToLower()] = value;

      // You can always add a value to a dictionary,
      // so this method always returns true.
      return true;
    }
  }
}
