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
using System.Globalization;
using Scripting.SSharp.Parser.Ast;

namespace Scripting.SSharp.Parser
{
  internal class NumberLiteral : RegexBasedTerminal
  {
    #region Fields
    public const string pattern = "(0[xX][0-9a-fA-F]+)|([-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?(ul|u|l|d|f|m)?)";
    //"[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
    private static List<string> firsts = new List<string>();
    #endregion

    #region Construction
    public NumberLiteral()
      : base("number", pattern)
    {
      firsts.Add("0x");
      firsts.AddRange(new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" });
    }
    #endregion

    #region Methods
    protected override TokenAst CreateToken(CompilerContext context, ISourceStream source)
    {
      TokenAst token = base.CreateToken(context, source);
      token.Value = ConvertNumber(token.Text);
      return token;
    }

    public override IList<string> GetFirsts()
    {
      return firsts;
    }

    private object ConvertNumber(string text)
    {
      if (text.Contains(".") || text.Contains("e") || text.EndsWith("d") || text.EndsWith("f") || text.EndsWith("m"))
        return ToDouble(text);

      if (text.StartsWith("0x"))
        return Int32.Parse(text.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

      return ToInteger(text);
      //throw new NotSupportedException("Number is not in correct format:" + text);
    }

    private object ToInteger(string text)
    {
      if (text.EndsWith("ul"))
        return Convert.ToUInt64(text.TrimEnd('u', 'l'), CultureInfo.InvariantCulture);

      if (text.EndsWith("u"))
        return Convert.ToUInt32(text.TrimEnd('u'), CultureInfo.InvariantCulture);

      if (text.EndsWith("l"))
        return Convert.ToInt64(text.TrimEnd('l'), CultureInfo.InvariantCulture);

      return Convert.ToInt32(text, CultureInfo.InvariantCulture);
    }

    private static object ToDouble(string text)
    {
      if (text.EndsWith("f"))
        return Convert.ToSingle(text.TrimEnd('f'), CultureInfo.InvariantCulture);
      
      if (text.EndsWith("m"))
        return Convert.ToDecimal(text.TrimEnd('m'), CultureInfo.InvariantCulture);

      if (text.EndsWith("d"))
        return Convert.ToDouble(text.TrimEnd('d'), CultureInfo.InvariantCulture);

      return Convert.ToDouble(text, CultureInfo.InvariantCulture);
    }
    #endregion
  }
}
