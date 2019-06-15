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

namespace Scripting.SSharp.Runtime.Operators
{
  /// <summary>
  /// Implementation of negate operator
  /// </summary>
  public class NegateOperator1 : IOperator
  {
    public virtual string Name
    {
      get { return "-"; }
    }

    public bool Unary
    {
      get { return true; }
    }

    public object Evaluate(object value)
    {
      if (value == null) throw new NullReferenceException("Cannot negate null value");
      Type type = value.GetType();

      if (type == typeof(Boolean))
        return !(Boolean)value;
      if (type == typeof(Int16))
        return -(Int16)value;
      if (type == typeof(Int32))
        return -(Int32)value;
      if (type == typeof(Int64))
        return -(Int64)value;
      if (type == typeof(Double))
        return -(Double)value;
      if (type == typeof(float))
        return -(float)value;

      throw new Exception("Cannot negate value of type: " + type.Name);
    }

    public object Evaluate(object left, object right)
    {
      throw new NotImplementedException();
    }
  }

  /// <summary>
  /// Implementation of negate operator
  /// </summary>
  public class NegateOperator2 : NegateOperator1
  {
    public override string Name
    {
      get { return "~"; }
    }
  }

  /// <summary>
  /// Implementation of negate operator
  /// </summary>
  public class NegateOperator3 : NegateOperator1
  {
    public override string Name
    {
      get { return "!"; }
    }    
  }
}
