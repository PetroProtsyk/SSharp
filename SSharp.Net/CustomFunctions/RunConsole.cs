using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.CustomFunctions
{
  internal class RunConsole : IInvokable
  {
    public static RunConsole FunctionDefinition = new RunConsole();
    public static string FunctionName = "RunConsole";

    private RunConsole()
    {
    }

    #region IInvokable Members

    public bool CanInvoke()
    {
      return true;
    }

    public object Invoke(IScriptContext context, object[] args)
    {
      string code = 
          @"Console.WriteLine('Please Input Script.NET program. Press Ctrl+Z when finish.');
            s = Console.In.ReadToEnd();
            astNode = Compiler.Parse(s);
            if (astNode != null)
              astNode.Execute(new ScriptContext()).Value;
            else      
              throw new ScriptException('Syntax Error');";

      Script prog = Script.Compile(code);
      return prog.Execute();
    }

    #endregion
  }
}