using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Runtime.Promotion;
using Scripting.SSharp;
using Scripting.SSharp.Parser.Ast;

namespace UnitTests
{
  /// <summary>
  /// Summary description for MetaFeatures
  /// </summary>
  [TestClass]
  public class MetaFeatures
  {

    [TestInitialize]
    public void Setup()
    {
      RuntimeHost.Initialize();
      EventBroker.ClearAllSubscriptions();
    }

    [TestCleanup]
    public void TearDown()
    {
      RuntimeHost.CleanUp();
      EventBroker.ClearAllSubscriptions();
    }

    [TestMethod]
    public void EvalFunctionNew()
    {
      object rez = Script.RunCode("eval('1+1');");

      Assert.AreEqual(2, rez);
    }

    [TestMethod]
    public void MetaOperatorNew()
    {
      object rez = Script.RunCode("<! 1+1; !>;");

      ///Should be ScriptProg
      Assert.IsInstanceOfType(rez, typeof(ScriptAst));

      ScriptAst prog = (ScriptAst)rez;
      object rez1 = prog.Execute(new ScriptContext());

      Assert.AreEqual(2, rez1);
    }

    [TestMethod]
    public void BaseCode()
    {
      Script script = Script.Compile("1+1; a=0; c = <! 1+1; !>; b = 12;");
      script.Execute();

      ScriptAst metaNode = (ScriptAst)script.Context.GetItem("c", true);
      string code = script.Code(metaNode);

      Assert.AreEqual("<! 1+1; !>", code);
    }

    [TestMethod]
    public void BaseTree()
    {
      Script script = Script.Compile("c = <! 1+1; !>;");
      string rez = script.SyntaxTree;

      Assert.IsInstanceOfType(rez, typeof(string));
    }

    [TestMethod]
    public void MetaOperator()
    {
      object resultVal = Script.RunCode(@"a=<! x = 2; !>; a();");
      
      Assert.AreEqual(2, resultVal);
    }

    [TestMethod]
    public void AppendOperator()
    {
      object resultVal = Script.RunCode(@"AppendAst( <! x = 1; !> ); a=<! x = 2; !>; a();");

      Assert.AreEqual((int)1, resultVal);
    }

    [TestMethod]
    public void EvalFunction()
    {
      object resultVal = Script.RunCode(@"eval('a=2;');");
      
      Assert.AreEqual(2, resultVal);
    }

  }
}
