using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Runtime.Promotion;
using Scripting.SSharp;
using Scripting.SSharp.Execution.Compilers.Dom;

namespace ScriptNET.Execution.UnitTests
{
  /// <summary>
  /// Summary description for UnitTest1
  /// </summary>
  public class DomTests : IDisposable
  {
    public DomTests()
    {
      RuntimeHost.Initialize();
    }

    public void Dispose()
    {
      RuntimeHost.CleanUp();
    }
   
    [Fact]
    public void CodeVariableReferenceTest()
    {
      object testObject = new object();

      IScriptContext context = new ScriptContext();
      context.SetItem("a", testObject);

      CodeProgram prog = new CodeProgram();
      prog.Statements.Add(
        new CodeExpressionStatement(
          new CodeVariableReference("a")));

      CodeDomCompiler.Compile(prog).Execute(context);

      Assert.Equal(testObject, context.Result);
    }

    [Fact]
    public void CodeValueReferenceTest()
    {
      IScriptContext context = new ScriptContext();
      CodeProgram prog1 = new CodeProgram();
      prog1.Statements.Add(
        new CodeExpressionStatement(
          new CodeAssignExpression("b",
            new CodeValueReference("Test B"))));

      CodeDomCompiler.Compile(prog1).Execute(context);

      Assert.Equal("Test B", context.GetItem("b", true));
    }
  }
}
