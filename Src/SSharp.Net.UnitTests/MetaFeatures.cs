using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Runtime.Promotion;
using Scripting.SSharp;
using Scripting.SSharp.Parser.Ast;

namespace UnitTests
{
  /// <summary>
  /// Summary description for MetaFeatures
  /// </summary>
  public class MetaFeatures : IDisposable
  {
    public MetaFeatures()
    {
      RuntimeHost.Initialize();
      EventBroker.ClearAllSubscriptions();
    }

    public void Dispose()
    {
      RuntimeHost.CleanUp();
      EventBroker.ClearAllSubscriptions();
    }

    [Fact]
    public void EvalFunctionNew()
    {
      object rez = Script.RunCode("eval('1+1');");

      Assert.Equal(2, rez);
    }

    [Fact]
    public void MetaOperatorNew()
    {
      object rez = Script.RunCode("<! 1+1; !>;");

      Assert.IsType<ScriptProg>(rez);

      ScriptAst prog = (ScriptAst)rez;
      object rez1 = prog.Execute(new ScriptContext());

      Assert.Equal(2, rez1);
    }

    [Fact]
    public void BaseCode()
    {
      Script script = Script.Compile("1+1; a=0; c = <! 1+1; !>; b = 12;");
      script.Execute();

      ScriptAst metaNode = (ScriptAst)script.Context.GetItem("c", true);
      string code = script.Code(metaNode);

      Assert.Equal("<! 1+1; !>", code);
    }

    [Fact]
    public void BaseTree()
    {
      Script script = Script.Compile("c = <! 1+1; !>;");
      string rez = script.SyntaxTree;

      Assert.IsType<string>(rez);
    }

    [Fact]
    public void MetaOperator()
    {
      object resultVal = Script.RunCode(@"a=<! x = 2; !>; a();");
      
      Assert.Equal(2, resultVal);
    }

    [Fact]
    public void AppendOperator()
    {
      object resultVal = Script.RunCode(@"AppendAst( <! x = 1; !> ); a=<! x = 2; !>; a();");

      Assert.Equal((int)1, resultVal);
    }

    [Fact]
    public void EvalFunction()
    {
      object resultVal = Script.RunCode(@"eval('a=2;');");
      
      Assert.Equal(2, resultVal);
    }

  }
}
