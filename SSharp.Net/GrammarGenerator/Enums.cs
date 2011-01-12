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

namespace Scripting.SSharp.Parser
{
  internal enum TokenCategory
  {
    Content,
    Outline,
    Comment,
    Error,
  }

  [Flags]
  internal enum TokenMatchMode
  {
    ByValue = 1,
    ByType = 2,
    ByValueThenByType = ByValue | ByType,
  }

  internal enum Associativity
  {
    Left,
    Right,
    Neutral
  }

  internal enum ParserActionType
  {
    Shift,
    Reduce,
    Operator
  }

  internal enum TermOptions
  {
    None = 0,
    IsOperator = 0x01,
    IsGrammarSymbol = 0x02,
    IsPunctuation = 0x20,
    IsList = 0x80,
    IsStarList = 0x100,
    IsNonGrammar = 0x0200,
  }
}
