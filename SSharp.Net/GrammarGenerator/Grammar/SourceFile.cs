using System;
namespace Scripting.SSharp.Parser
{
  public interface ISourceStream
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

  public struct SourceLocation
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

  public struct SourceSpan
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

  public class SourceFile : ISourceStream
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
        int cmp = 0;
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
