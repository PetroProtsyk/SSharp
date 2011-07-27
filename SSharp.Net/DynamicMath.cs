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
using System.Collections.Generic;

namespace Scripting.SSharp
{
  public static class DynamicMath
  {
    static Dictionary<Type, Dictionary<Type, Func<dynamic, dynamic, Tuple<dynamic, dynamic>>>> converters = new Dictionary<Type, Dictionary<Type, Func<dynamic, dynamic, Tuple<dynamic, dynamic>>>>();

    public static void AddConverter<TLeft, TRight>(Func<dynamic, dynamic, Tuple<dynamic, dynamic>> function)
    {
      Type leftType = typeof(TLeft);
      Dictionary<Type, Func<dynamic, dynamic, Tuple<dynamic, dynamic>>> funcs = null;
      if (!converters.TryGetValue(leftType, out funcs))
      {
        funcs = new Dictionary<Type, Func<dynamic, dynamic, Tuple<dynamic, dynamic>>>();
        converters[leftType] = funcs;
      }

      funcs[typeof(TRight)] = function;
    }

    public static Tuple<dynamic, dynamic> TryConvert(dynamic left, dynamic right)
    {
      // Denys Vuika: the code below crash for mono 2.8 though should work fine, 
      // slightly rewriting to make the code mono-compatible
      /*
      Dictionary<Type, Func<dynamic, dynamic, Tuple<dynamic, dynamic>>> funcs = null;
      if (converters.TryGetValue(left.GetType(), out funcs))
      {
        Func<dynamic, dynamic, Tuple<dynamic, dynamic>> function = null;
        if (funcs.TryGetValue(right.GetType(), out function))
          return function(left, right);
      }
      */

      if (left == null || right == null)
        return null;

      Type leftType = left.GetType();
      if (converters.ContainsKey(leftType))
      {
        var funcs = converters[leftType];
        Type rightType = right.GetType();

        if (funcs.ContainsKey(rightType))
          return funcs[rightType](left, right);
      }

      return null;
    }

    static DynamicMath()
    {
      AddConverter<decimal, double>((left, right) => new Tuple<dynamic, dynamic>((double)left, right));
      AddConverter<double, decimal>((left, right) => new Tuple<dynamic, dynamic>(left, (double)right));

      AddConverter<decimal, float>((left, right) => new Tuple<dynamic, dynamic>((float)left, right));
      AddConverter<float, decimal>((left, right) => new Tuple<dynamic, dynamic>(left, (float)right));
      
      Func<dynamic, dynamic, Tuple<dynamic, dynamic>> rightToString = (left, right) => new Tuple<dynamic, dynamic>(left, right.ToString());
      Func<dynamic, dynamic, Tuple<dynamic, dynamic>> leftToString = (left, right) => new Tuple<dynamic, dynamic>(left.ToString(), right);

      AddConverter<string, Int16>(rightToString);
      AddConverter<string, Int32>(rightToString);
      AddConverter<string, Int64>(rightToString);
      AddConverter<string, float>(rightToString);
      AddConverter<string, decimal>(rightToString);
      AddConverter<string, double>(rightToString);

      AddConverter<Int16, string>(leftToString);
      AddConverter<Int32, string>(leftToString);
      AddConverter<Int64, string>(leftToString);
      AddConverter<float, string>(leftToString);
      AddConverter<decimal, string>(leftToString);
      AddConverter<double, string>(leftToString);

      AddConverter<string, char>(rightToString);
      AddConverter<char, string>(leftToString);
    }

    public static bool Eq(dynamic left, dynamic right)
    {
      Tuple<dynamic, dynamic> converted = TryConvert(left, right);
      if (converted != null)
        return ((converted.Item1 == converted.Item2) || (((converted.Item1 != null) && (converted.Item2 != null)) && converted.Item1.Equals(converted.Item2)));

      // Use such comparsion instead of object.Equals(left, right); in order to force appropriate == operator
      // and to avoid boxing for primitive types
      return ((left == right) || (((left != null) && (right != null)) && left.Equals(right)));
    }

    public static dynamic Add(dynamic left, dynamic right)
    {
      Tuple<dynamic, dynamic> converted = TryConvert(left, right);
      if (converted != null) return converted.Item1 + converted.Item2;

      return (left + right);
    }

    public static dynamic Subtract(dynamic left, dynamic right)
    {
      Tuple<dynamic, dynamic> converted = TryConvert(left, right);
      if (converted != null) return converted.Item1 - converted.Item2;

      return (left - right);
    }

    public static dynamic Multiply(dynamic left, dynamic right)
    {
      Tuple<dynamic, dynamic> converted = TryConvert(left, right);
      if (converted != null) return converted.Item1 * converted.Item2;

      return left * right;
    }

    public static bool Less(dynamic left, dynamic right)
    {
      if (left is string && right is string)
        return string.Compare(left, right) < 0;

      Tuple<dynamic, dynamic> converted = TryConvert(left, right);
      if (converted != null) return converted.Item1 < converted.Item2;

      return left < right;
    }

    public static bool LessOrEquals(dynamic left, dynamic right)
    {
      if (left is string && right is string)
        return string.Compare(left, right) <= 0;

      Tuple<dynamic, dynamic> converted = TryConvert(left, right);
      if (converted != null) return converted.Item1 <= converted.Item2;

      return left <= right;
    }

    public static bool Greater(dynamic left, dynamic right)
    {
      if (left is string && right is string)
        return string.Compare(left, right) > 0;

      Tuple<dynamic, dynamic> converted = TryConvert(left, right);
      if (converted != null) return converted.Item1 > converted.Item2;

      return left > right;
    }

    public static bool GreaterOrEquals(dynamic left, dynamic right)
    {
      if (left is string && right is string)
        return string.Compare(left, right) >= 0;

      Tuple<dynamic, dynamic> converted = TryConvert(left, right);
      if (converted != null) return converted.Item1 >= converted.Item2;

      return left >= right;
    }

    public static dynamic Mod(dynamic left, dynamic right)
    {
      Tuple<dynamic, dynamic> converted = TryConvert(left, right);
      if (converted != null) return converted.Item1 % converted.Item2;

      return left % right;
    }

    public static dynamic Div(dynamic left, dynamic right)
    {
      Tuple<dynamic, dynamic> converted = TryConvert(left, right);
      if (converted != null) return converted.Item1 / converted.Item2;
            
      return left / right;
    }

    public static bool And(dynamic left, dynamic right)
    {
      return left & right;
    }

    public static bool ConditionalAnd(dynamic left, dynamic right)
    {
      return left && right;
    }

    public static bool Or(dynamic left, dynamic right)
    {
      return left | right;
    }

    public static bool ConditionalOr(dynamic left, dynamic right)
    {
      return left || right;
    }
  }
}
