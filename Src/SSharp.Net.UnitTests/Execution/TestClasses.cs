using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripting.SSharp.UnitTests.Execution
{
  public class VM_Test1
  {
    public int Level { get; set; }

    public VM_Test1(int level)
    {
      Level = level;
    }

    public VM_Test1()
      : this(0)
    {
    }

    public VM_Test1 GetNextLevel()
    {
      return new VM_Test1(Level + 1);
    }

    public VM_Test1 Next
    {
      get
      {
        return GetNextLevel();
      }
    }
  }

  public class VM_Test2<T> where T : class
  {
    public T Level { get; set; }

    public VM_Test2(T level)
    {
      Level = level;
    }

    public VM_Test2()
      : this(null)
    {
    }

    public VM_Test2<T> GetNextLevel(T level)
    {
      return new VM_Test2<T>(level);
    }
  }
}
