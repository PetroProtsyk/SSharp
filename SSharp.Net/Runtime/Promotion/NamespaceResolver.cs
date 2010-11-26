
namespace Scripting.SSharp.Runtime.Promotion
{
  internal static class NamespaceResolver
  {
    internal static Namespace Get(string name)
    {
      return new Namespace(name);
    }
  }
}
