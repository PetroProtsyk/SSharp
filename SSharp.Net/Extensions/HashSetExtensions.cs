using System.Collections.Generic;

namespace Scripting.SSharp
{
  internal static class HashSetExtensions
  {
    public static void AddRange<T>(this HashSet<T> hashset, IEnumerable<T> values)
    {
      foreach (T value in values)
        hashset.Add(value);
    }
  }
}
