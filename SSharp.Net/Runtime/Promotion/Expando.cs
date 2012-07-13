using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting.SSharp.Runtime.Promotion
{
  /// <summary>
  /// Represents implementation of Expando object via IScriptable interface.
  /// By default Expando objects created with a following syntax construct:
  ///   [name->valueExpression, name->valueExpression];
  /// This default type may be overriden by ScriptableObjectType option
  /// in script configuration.
  /// </summary>
  public class Expando : IScriptable, ISupportAssign
  {
    #region Fields
    Dictionary<string, object> fields = new Dictionary<string, object>();
    #endregion

    #region Public Members
    public IEnumerable<string> Fields
    {
      get
      {
        return fields.Keys;
      }
    }

    public void AssignTo(object target)
    {
      foreach (string field in Fields)
      {
        IMemberBinding bind = RuntimeHost.Binder.BindToMember(target, field, false);
        if (bind != null)
          bind.SetValue(fields[field]);
        //RuntimeHost.Binder.Set(field, target, fields[field], false);
      }
    }

    public object this[string fieldName]
    {
      get
      {
        object result;
        if (fields.TryGetValue(fieldName, out result)) return result;

        return null;
      }
    }

    public override string ToString()
    {
      bool first = true;
      StringBuilder builder = new StringBuilder("[");
      foreach (string field in Fields)
      {
        if (!first) builder.Append(','); else first = false;

        builder.Append(field);
        builder.Append("->");

        object value = this[field];
        if (value == null)
          builder.Append("null");
        else
          builder.Append(value.ToString());
      }
      builder.Append("]");
      return builder.ToString();
    }
    #endregion

    #region IScriptable
    [Promote(false)]
    public virtual object Instance
    {
      get { return this; }
    }

    [Promote(false)]
    public virtual IMemberBinding GetMember(string name, params object[] arguments)
    {
      if (arguments != null && arguments.Length != 0) return null;

      return new ExpandoBind(this, name);
    }

    [Promote(false)]
    public virtual IBinding GetMethod(string name, params object[] arguments)
    {
      if (arguments != null && arguments.Length != 0) return null;
      if (!fields.ContainsKey(name)) throw new ScriptMethodNotFoundException(name);
      return new ExpandoBind(this, name);
    }

    #endregion

    #region ExpandoBind
    protected class ExpandoBind : IMemberBinding
    {
      Expando expando;
      string name;

      public ExpandoBind(Expando expando, string name)
      {
        this.expando = expando;
        this.name = name;
      }

      #region IMemberBind

      public object Target
      {
        get { return expando; }
      }

      public Type TargetType
      {
        get { return typeof(object); }
      }

      public System.Reflection.MemberInfo Member
      {
        get { throw new NotSupportedException(); }
      }

      public void SetValue(object value)
      {
        if (expando.fields.ContainsKey(name))
        {
          expando.fields[name] = value;
        }
        else
        {
          expando.fields.Add(name, value);
        }
      }

      public object GetValue()
      {
        object result;
        if (expando.fields.TryGetValue(name, out result))
          return result;

        throw new ScriptIdNotFoundException(name);
      }

      public void AddHandler(object value)
      {
        throw new NotSupportedException();
      }

      public void RemoveHandler(object value)
      {
        throw new NotSupportedException();
      }

      #endregion

      #region IInvokable

      public bool CanInvoke()
      {
        return true;
      }

      public object Invoke(IScriptContext context, object[] args)
      {
        IInvokable method = GetValue() as IInvokable;
        if (method != null)
          return method.Invoke(context, args);

        throw new ScriptIdNotFoundException(string.Format("Method {0} not found", name));
      }

      #endregion
    }
    #endregion
  }
}
