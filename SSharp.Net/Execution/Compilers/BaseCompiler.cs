using System;
using System.Collections.Generic;

namespace Scripting.SSharp.Execution.Compilers
{
  /// <summary>
  /// Base compiler
  /// </summary>
  public abstract class BaseCompiler<ElementType, ContextType, ResultType, CompilerInterfaceType> 
    where CompilerInterfaceType : ICompiler<ElementType,ContextType, ResultType>
  {
    public static void Register(Type compilerType)
    {
      CompilerTypeAttribute compilerAttribute = Attribute.GetCustomAttribute(compilerType, typeof(CompilerTypeAttribute)) as CompilerTypeAttribute;
      if (compilerAttribute == null) throw new NotSupportedException("CompilerTypeAttribute is not assigned");
      if (!typeof(ElementType).IsAssignableFrom(compilerAttribute.NodeType)) throw new NotSupportedException();
      
      compilers.Add(compilerAttribute.NodeType, (CompilerInterfaceType)Activator.CreateInstance(compilerType));
    }
        
    public static void Register<TCompiler>() where TCompiler : class, CompilerInterfaceType, new()
    {
      CompilerTypeAttribute compilerAttribute = Attribute.GetCustomAttribute(typeof(TCompiler), typeof(CompilerTypeAttribute)) as CompilerTypeAttribute;
      if (compilerAttribute == null) throw new NotSupportedException("CompilerTypeAttribute is not assigned");
      if (!typeof(ElementType).IsAssignableFrom(compilerAttribute.NodeType)) throw new NotSupportedException();

      compilers.Add(compilerAttribute.NodeType, new TCompiler());
    }
      
    // TODO: (Denis Vuyka) This function is not safe, wire dictionary access with TryGetValue
    protected internal static ResultType Compile(ElementType element, ContextType result)
    {
      return compilers[element.GetType()].Compile(element, result);
    }

    protected internal static T Compile<T>(ElementType element, ContextType result) where T : ResultType
    {
      return (T)Compile(element, result);
    }

    // TODO: (Denis Vuyka) this is not thread-safe
    protected static readonly Dictionary<Type, CompilerInterfaceType> compilers = new Dictionary<Type, CompilerInterfaceType>();
  }
}
