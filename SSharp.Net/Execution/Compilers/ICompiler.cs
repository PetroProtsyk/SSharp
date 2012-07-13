using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripting.SSharp.Execution.Compilers
{
  public interface ICompiler<FromType, ContextType, ResultType>
  {
    ResultType Compile(FromType syntaxNode, ContextType prog);
  }
}
