using System;
using System.Collections;
using System.Collections.Generic;
using Scripting.SSharp.Diagnostics;

namespace Scripting.SSharp.Runtime
{
  public class FunctionTable : IEnumerable<KeyValuePair<string, Type>>
  {
    private readonly Dictionary<string, Type> _functions = new Dictionary<string, Type>();

    public FunctionTable AddFunction<T>(string name) where T : IInvokable, new()
    {      
      Requires.NotNullOrEmpty(name, "name");

      _functions[name] = typeof(T);                        

      return this;
    }

    public FunctionTable AddFunction(string name, Type functionType)
    {
      Requires.NotNullOrEmpty(name, "name");
      Requires.NotNull(functionType, "functionType");
      Requires.OfType<IInvokable>(functionType, "functionType");

      _functions[name] = functionType;

      return this;
    }

    public bool Contains(string name)
    {
      Requires.NotNullOrEmpty(name, "name");

      return _functions.ContainsKey(name);
    }

    #region IEnumerable<KeyValuePair<string,Type>> Members

    public IEnumerator<KeyValuePair<string, Type>> GetEnumerator()
    {
      return _functions.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
      return _functions.GetEnumerator();
    }

    #endregion
  }
}
