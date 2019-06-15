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
  internal interface ISourceStream
  {
    int Position { get; set; }

    char CurrentChar { get; } 
    
    char NextChar { get; }    
    
    bool MatchSymbol(string symbol, bool ignoreCase);

    string Text { get; } 
    
    string GetLexeme();
    
    SourceLocation TokenStart { get; set; }
    
    int TabWidth { get; }
    
    bool EOF();
  }

  internal struct SourceLocation
  {
    public int Position;
    
    public int Line;
    
    public int Column;
  
    public SourceLocation(int position, int line, int column)
    {
      Position = position;
      Line = line;
      Column = column;
    }
  }

  internal struct SourceSpan
  {
    public readonly SourceLocation Start;
    
    public readonly int Length;
    
    public SourceSpan(SourceLocation start, int length)
    {
      Start = start;
      Length = length;
    }

    public int EndPos  
    {
      get { return Start.Position + Length; }
    }
  }

  internal class SourceFile : ISourceStream
  {
    public SourceFile(string text, string fileName, int tabWidth)
    {
      Text = text;
      FileName = fileName;
      TabWidth = tabWidth;
    }

    public SourceFile(string text, string fileName)
      : this(text, fileName, 8)
    {
    }

    #region ISourceFile Members
    public string FileName
    {
      get;
      private set;
    }

    public int TabWidth
    {
      get;
      private set;
    }

    public int Position
    {
      get;
      set;
    }

    public bool EOF()
    {
      return Position >= Text.Length;
    }

    public char CurrentChar
    {
      get
      {
        if (Position >= Text.Length) return '\0';
        return Text[Position];
      }
    }

    public char NextChar
    {
      get
      {
        if (Position + 1 >= Text.Length) return '\0';
        return Text[Position + 1];
      }
    }

    public bool MatchSymbol(string symbol, bool ignoreCase)
    {
      try
      {
        int cmp;
        if (ignoreCase)
        {
          cmp = string.Compare(Text, Position, symbol, 0, symbol.Length, StringComparison.InvariantCultureIgnoreCase);
        }
        else
        {
          cmp = string.Compare(Text, Position, symbol, 0, symbol.Length, StringComparison.InvariantCulture);
        }
        return cmp == 0;
      }
      catch
      {
        return false;
      }
    }

    public string Text
    {
      get;
      private set;
    }

    public string GetLexeme()
    {
      return Text.Substring(TokenStart.Position, Position - TokenStart.Position);
    }

    public SourceLocation TokenStart
    {
      get;
      set;
    }
    #endregion
  }

}
