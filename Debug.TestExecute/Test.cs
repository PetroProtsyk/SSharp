using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Debug.TestExecute
{
  public class Test
  {
    public static void IsTrue(bool value)
    {
      if (!value) throw new TestException("Condition failed");
    }

    public static void AreEqual(object v1, object v2)
    {
      if (!object.Equals(v1, v2)) throw new TestException("Equality condition failed");
    }
  }

  public class TestException : Exception
  {
    public TestException(string message)
      : base(message)
    {
    }
  }
}
