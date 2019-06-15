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
using System.Collections.Generic;

namespace Scripting.SSharp.Execution.Compilers
{
  /// <summary>
  /// Base compiler
  /// </summary>
  internal abstract class BaseCompiler<ElementType, ContextType, ResultType, CompilerInterfaceType> 
    where CompilerInterfaceType : ICompiler<ElementType,ContextType, ResultType>
  {
    public static void Register(Type compilerType)
    {
      var compilerAttribute = Attribute.GetCustomAttribute(compilerType, typeof(CompilerTypeAttribute)) as CompilerTypeAttribute;
      if (compilerAttribute == null) throw new NotSupportedException("CompilerTypeAttribute is not assigned");
      if (!typeof(ElementType).IsAssignableFrom(compilerAttribute.NodeType)) throw new NotSupportedException();
      
      Compilers.Add(compilerAttribute.NodeType, (CompilerInterfaceType)Activator.CreateInstance(compilerType));
    }
        
    public static void Register<TCompiler>() where TCompiler : class, CompilerInterfaceType, new()
    {
      var compilerAttribute = Attribute.GetCustomAttribute(typeof(TCompiler), typeof(CompilerTypeAttribute)) as CompilerTypeAttribute;
      if (compilerAttribute == null) throw new NotSupportedException("CompilerTypeAttribute is not assigned");
      if (!typeof(ElementType).IsAssignableFrom(compilerAttribute.NodeType)) throw new NotSupportedException();

      Compilers.Add(compilerAttribute.NodeType, new TCompiler());
    }
      
    // TODO: (Denis Vuyka) This function is not safe, wire dictionary access with TryGetValue
    protected internal static ResultType Compile(ElementType element, ContextType result)
    {
      return Compilers[element.GetType()].Compile(element, result);
    }

    protected internal static T Compile<T>(ElementType element, ContextType result) where T : ResultType
    {
      return (T)Compile(element, result);
    }

    // TODO: (Denis Vuyka) this is not thread-safe
    protected static readonly Dictionary<Type, CompilerInterfaceType> Compilers = new Dictionary<Type, CompilerInterfaceType>();
  }
}
