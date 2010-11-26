using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Runtime.Promotion;
using Scripting.SSharp;
using Scripting.SSharp.Execution.Compilers.Dom;

namespace ScriptNET.Execution.UnitTests
{
  /// <summary>
  /// Summary description for UnitTest1
  /// </summary>
  [TestClass]
  public class DomTests
  {
    [TestInitialize]
    public void Setup()
    {
      RuntimeHost.Initialize();
    }

    [TestCleanup]
    public void TearDown()
    {
      RuntimeHost.CleanUp();
    }
   
    [TestMethod]
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

      Assert.AreEqual(testObject, context.Result);
    }

    [TestMethod]
    public void CodeValueReferenceTest()
    {
      IScriptContext context = new ScriptContext();
      CodeProgram prog1 = new CodeProgram();
      prog1.Statements.Add(
        new CodeExpressionStatement(
          new CodeAssignExpression("b",
            new CodeValueReference("Test B"))));

      CodeDomCompiler.Compile(prog1).Execute(context);

      Assert.AreEqual("Test B", context.GetItem("b", true));
    }
  }
}
