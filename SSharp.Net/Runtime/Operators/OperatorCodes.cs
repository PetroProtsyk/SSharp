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

namespace Scripting.SSharp.Runtime.Operators
{
  internal static class OperatorCodes
  {
    /// <summary>
    /// Adds two values and pushes the result onto the evaluation stack.
    /// </summary>
    public const string Add = "+";

    /// <summary>
    /// Subtracts one value from another and pushes the result onto the evaluation stack.
    /// </summary>
    public const string Sub = "-";

    /// <summary>
    /// Multiplies two values and pushes the result on the evaluation stack.
    /// </summary>
    public const string Mul = "*";




    /// <summary>
    /// "Greater than" relational operator ( >) that returns true if the first operand is greater than the second, false otherwise. 
    /// </summary>
    public const string Gr = ">";

    /// <summary>
    /// "Greater than or equal" relational operator, >= that returns true if the first operand is greater than or equal to the second, false otherwise.
    /// </summary>
    public const string Ge = ">=";

    /// <summary>
    /// The division operator ( /) divides its first operand by its second.
    /// </summary>
    public const string Div = "/";
  }
}
