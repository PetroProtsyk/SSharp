namespace Scripting.SSharp.Execution.Compilers
{
  internal interface ICompiler<FromType, ContextType, ResultType>
  {
    ResultType Compile(FromType syntaxNode, ContextType prog);
  }
}
