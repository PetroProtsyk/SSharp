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

using System.Collections.Generic;
using Scripting.SSharp.Parser.Ast;

namespace Scripting.SSharp.Parser.FastGrammar
{
  internal struct ParserStackElement
  {
    public readonly AstNode Node;
    public readonly ParserState State;

    public ParserStackElement(AstNode node, ParserState state)
    {
      Node = node;
      State = state;
    }
  }

  internal class ParserStack
  {
    private readonly List<ParserStackElement> _data = new List<ParserStackElement>(256);

    public int Count
    {
      get { return _data.Count; }
    }

    public int Capacity
    {
      get { return _data.Capacity; }
    }

    public ParserStackElement this[int index]
    {
      get { return _data[index]; }
    }

    public ParserStackElement Top
    {
      get { return this[Count - 1]; }
    }

    public void Push(AstNode node, ParserState state)
    {
      _data.Add(new ParserStackElement(node, state));
    }

    public void Pop(int popCount)
    {
      _data.RemoveRange(Count - popCount, popCount);
    }

    public void Reset()
    {
      _data.Clear();
    }
  }
}
