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
