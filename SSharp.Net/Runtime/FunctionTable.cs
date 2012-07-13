using System;
using System.Collections;
using System.Collections.Generic;
using Scripting.SSharp.Diagnostics;

namespace Scripting.SSharp.Runtime
{
  public class FunctionTable : IEnumerable<KeyValuePair<string, Type>>
  {
    private Dictionary<string, Type> functions = new Dictionary<string, Type>();

    public FunctionTable AddFunction<T>(string name) where T : IInvokable, new()
    {      
      Requires.NotNullOrEmpty(name, "name");

      functions[name] = typeof(T);                        

      return this;
    }

    public FunctionTable AddFunction(string name, Type functionType)
    {
      Requires.NotNullOrEmpty(name, "name");
      Requires.NotNull<Type>(functionType, "functionType");
      Requires.OfType<IInvokable>(functionType, "functionType");

      functions[name] = functionType;

      return this;
    }

    public bool Contains(string name)
    {
      Requires.NotNullOrEmpty(name, "name");

      return functions.ContainsKey(name);
    }

    #region IEnumerable<KeyValuePair<string,Type>> Members

    public IEnumerator<KeyValuePair<string, Type>> GetEnumerator()
    {
      return functions.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
      return functions.GetEnumerator();
    }

    #endregion
  }
}
