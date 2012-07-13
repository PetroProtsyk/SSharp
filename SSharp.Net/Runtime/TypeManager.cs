using System;
using System.Collections.Generic;
using Scripting.SSharp.Runtime.Promotion;

namespace Scripting.SSharp.Runtime
{
  public class TypeManager
  {    
    private Dictionary<Type, Dictionary<string, IBinding>> bindings = new Dictionary<Type, Dictionary<string, IBinding>>();

    public TypeManager AddExtensionMethod(Type type, string methodName, IBinding methodBinding)
    {
      if (type == null) throw new ArgumentNullException("type");
      if (string.IsNullOrEmpty("methodName")) throw new ArgumentNullException("methodName");
      if (methodBinding == null) throw new ArgumentNullException("methodBinding");

      Dictionary<string, IBinding> methods = null;
      if (!bindings.TryGetValue(type, out methods))
      {
        methods = new Dictionary<string, IBinding>();
        bindings.Add(type, methods);
      }

      methods[methodName] = methodBinding;
      return this;
    }

    public IBinding GetExtensionMethod(object instance, string methodName)
    {
      if (instance == null) throw new ArgumentNullException("instance");
      if (string.IsNullOrEmpty(methodName)) throw new ArgumentNullException("methodName");

      Type type = instance as Type ?? instance.GetType();
      
      if (type != null)
      {
        Dictionary<string, IBinding> methods = null;
        if (bindings.TryGetValue(type, out methods))
        {
          IBinding binding = null;
          if (methods.TryGetValue(methodName, out binding))
            return binding;
        }
      }

      return null;
    }
  }
}
