using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Runtime.Promotion;
using Scripting.SSharp;
using Scripting.SSharp.Execution.Compilers.Dom;
using Scripting.SSharp.Execution;
using Scripting.SSharp.Execution.VM;
using Scripting.SSharp.UnitTests.Execution;

namespace UnitTests.Execution
{
  /// <summary>
  /// Summary description for AstDomCompilerTests
  /// </summary>
  public class AstDomCompilerTests : IDisposable
  {
    public AstDomCompilerTests()
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
    public void AstDomCompiler_CompileSimple()
    {
      IScriptContext context = new ScriptContext();

      CodeProgram domTree = AstDomCompiler.Compile(Script.Compile("x=1;", null, false).Ast);
      CodeDomCompiler.Compile(domTree).Execute(context);

      Assert.Equal(1, context.GetItem("x", true));
      Assert.Equal(1, context.Result);
    }

    [Fact]
    public void AstDomCompiler_CompileBinaryOperator()
    {
      IScriptContext context = new ScriptContext();

      CodeProgram domTree = AstDomCompiler.Compile(Script.Compile("x=1+1;", null, false).Ast);
      CodeDomCompiler.Compile(domTree).Execute(context);

      Assert.Equal(2, context.GetItem("x", true));
      Assert.Equal(2, context.Result);
    }

    [Fact]
    public void AstDomCompiler_CompileBinaryOperator1()
    {
      IScriptContext context = new ScriptContext();

      CodeProgram domTree = AstDomCompiler.Compile(Script.Compile("x=1+1+1;", null, false).Ast);
      CodeDomCompiler.Compile(domTree).Execute(context);

      Assert.Equal(3, context.GetItem("x", true));
      Assert.Equal(3, context.Result);
    }

    [Fact]
    public void AstDomCompiler_CompileBinaryOperator2()
    {
      IScriptContext context = new ScriptContext();

      CodeProgram domTree = AstDomCompiler.Compile(Script.Compile("x=1+2*3;", null, false).Ast);
      CodeDomCompiler.Compile(domTree).Execute(context);

      Assert.Equal(7, context.GetItem("x", true));
      Assert.Equal(7, context.Result);

      domTree = AstDomCompiler.Compile(Script.Compile("x=1*2+3;", null, false).Ast);
      CodeDomCompiler.Compile(domTree).Execute(context);

      Assert.Equal(5, context.GetItem("x", true));
      Assert.Equal(5, context.Result);

      domTree = AstDomCompiler.Compile(Script.Compile("x=1+2/2+3*2;", null, false).Ast);
      CodeDomCompiler
        .Compile(domTree)
        .Execute(context);

      Assert.Equal(8, context.GetItem("x", true));
      Assert.Equal(8, context.Result);
    }

    [Fact]
    public void AstDomCompiler_CompileBinaryOperator3()
    {
      IScriptContext context = new ScriptContext();

      CodeProgram domTree = AstDomCompiler.Compile(Script.Compile("x=(1+(1+3))*2-1;", null, false).Ast);
      CodeDomCompiler.Compile(domTree).Execute(context);

      Assert.Equal(9, context.GetItem("x", true));
      Assert.Equal(9, context.Result);

      domTree = AstDomCompiler.Compile(Script.Compile("x=(3+2)-1;", null, false).Ast);
      CodeDomCompiler.Compile(domTree).Execute(context);

      Assert.Equal(4, context.GetItem("x", true));
      Assert.Equal(4, context.Result);

      domTree = AstDomCompiler.Compile(Script.Compile("x=(String)-1;", null, false).Ast);
      CodeDomCompiler.Compile(domTree).Execute(context);

      Assert.Equal("-1", context.GetItem("x", true));
    }

    [Fact]
    public void AstDomCompiler_CompileBinaryOperator4()
    {
      IScriptContext context = new ScriptContext();

      CodeProgram domTree = AstDomCompiler.Compile(Script.Compile("1>2;", null, false).Ast);
      CodeDomCompiler.Compile(domTree).Execute(context);

      Assert.False((bool)context.Result);

      domTree = AstDomCompiler.Compile(Script.Compile("1<2;", null, false).Ast);
      CodeDomCompiler.Compile(domTree).Execute(context);

      Assert.True((bool)context.Result);

      domTree = AstDomCompiler.Compile(Script.Compile("2!=3;", null, false).Ast);
      CodeDomCompiler.Compile(domTree).Execute(context);

      Assert.True((bool)context.Result);

      domTree = AstDomCompiler.Compile(Script.Compile("5 == 5;", null, false).Ast);
      CodeDomCompiler.Compile(domTree).Execute(context);

      Assert.True((bool)context.Result);

      domTree = AstDomCompiler.Compile(Script.Compile("5>=5;", null, false).Ast);
      CodeDomCompiler.Compile(domTree).Execute(context);

      Assert.True((bool)context.Result);

      domTree = AstDomCompiler.Compile(Script.Compile("5<=4;", null, false).Ast);
      CodeDomCompiler.Compile(domTree).Execute(context);

      Assert.False((bool)context.Result);
    }

    [Fact]
    public void AstDomCompiler_Return()
    {
      IScriptContext context = new ScriptContext();

      CodeProgram domTree = AstDomCompiler.Compile(Script.Compile("return 1+1; 2+2;", null, false).Ast);
      CodeDomCompiler.Compile(domTree).Execute(context);

      Assert.Equal(2, context.Result);
    }

    [Fact]
    public void AstDomCompiler_IfElse()
    {
      IScriptContext context = new ScriptContext();

      CodeProgram domTree = AstDomCompiler.Compile(Script.Compile("x = 1; if (x>1) return 2; else return 3;", null, false).Ast);
      CodeDomCompiler.Compile(domTree).Execute(context);

      Assert.Equal(3, context.Result);

      domTree = AstDomCompiler.Compile(Script.Compile("x = 2; if (x>1) return 2; else return 3;", null, false).Ast);
      CodeDomCompiler.Compile(domTree).Execute(context);

      Assert.Equal(2, context.Result);

      domTree = AstDomCompiler.Compile(Script.Compile("x = 2; if (x>1) return 5;", null, false).Ast);
      CodeDomCompiler.Compile(domTree).Execute(context);

      Assert.Equal(5, context.Result);

      domTree = AstDomCompiler.Compile(Script.Compile("x = 1; if (x>1) return 5; return 10;", null, false).Ast);
      CodeDomCompiler.Compile(domTree).Execute(context);

      Assert.Equal(10, context.Result);

    }

    [Fact]
    public void AstDomCompiler_SwitchCase()
    {
      IScriptContext context = new ScriptContext();

      CodeProgram domTree = AstDomCompiler.Compile(Script.Compile(@"
        a = 2;
        switch(a){
          case 1: return 1;
          case 2: return 3;
          case 3: return 5;
          default: return 4;
        }
      ", null, false).Ast);
      CodeDomCompiler.Compile(domTree).Execute(context);

      Assert.Equal(3, context.Result);
    }

    [Fact]
    public void AstDomCompiler_SwitchDefault()
    {
      IScriptContext context = new ScriptContext();

      CodeProgram domTree = AstDomCompiler.Compile(Script.Compile(@"
        a = 15;
        switch(a){
          case 1: return 1;
          case 2: return 3;
          case 3: return 5;
          default: return 4;
        }
      ", null, false).Ast);
      CodeDomCompiler.Compile(domTree).Execute(context);

      Assert.Equal(4, context.Result);
    }

    [Fact]
    public void AstDomCompiler_While()
    {
      IScriptContext context = new ScriptContext();

      CodeProgram domTree = AstDomCompiler.Compile(Script.Compile("x = 1; while (x<10) x=x+1;", null, false).Ast);
      CodeDomCompiler.Compile(domTree).Execute(context);

      Assert.Equal(10, context.GetItem("x", true));
      Assert.False((bool)context.Result);

      domTree = AstDomCompiler.Compile(Script.Compile("x = 1; while (x<10) x=x+1; return 3;", null, false).Ast);
      CodeDomCompiler.Compile(domTree).Execute(context);

      Assert.Equal(3, context.Result);

      domTree = AstDomCompiler.Compile(Script.Compile("x = 2; while (x<2) return 2; return 5;", null, false).Ast);
      CodeDomCompiler.Compile(domTree).Execute(context);

      Assert.Equal(5, context.Result);

      domTree = AstDomCompiler.Compile(Script.Compile("x = 1; while (x<2) return 2; return 5;", null, false).Ast);
      CodeDomCompiler.Compile(domTree).Execute(context);

      Assert.Equal(2, context.Result);

    }

    [Fact]
    public void AstDomCompiler_Block()
    {
      IScriptContext context = new ScriptContext();

      CodeProgram domTree = AstDomCompiler.Compile(Script.Compile("{x = 1; y=2;}", null, false).Ast);
      CodeDomCompiler.Compile(domTree).Execute(context);

      Assert.Equal(1, context.GetItem("x", true));
      Assert.Equal(2, context.GetItem("y", true));

      domTree = AstDomCompiler.Compile(Script.Compile("x = 10; y = 0; while (x>0) {x=x-1; y = y+1;}", null, false).Ast);
      CodeDomCompiler.Compile(domTree).Execute(context);

      Assert.Equal(0, context.GetItem("x", true));
      Assert.Equal(10, context.GetItem("y", true));
    }

    [Fact]
    public void AstDomCompiler_For()
    {
      IScriptContext context = new ScriptContext();

      CodeProgram domTree = AstDomCompiler.Compile(Script.Compile("sum=0;for(i=0; i<10; i=i+1) sum = sum + i;", null, false).Ast);
      CodeDomCompiler.Compile(domTree).Execute(context);

      Assert.Equal(45, context.GetItem("sum", true));
    }

    [Fact]
    public void AstDomCompiler_ForEach()
    {
      IScriptContext context = new ScriptContext();
      context.SetItem("a", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

      CodeProgram domTree = AstDomCompiler.Compile(Script.Compile("sum=0; foreach(i in a) sum = sum + i;", null, false).Ast);
      ExecutableMachine vm = CodeDomCompiler.Compile(domTree);
      vm.Execute(context);

      Assert.Equal(45, context.GetItem("sum", true));
    }

    [Fact]
    public void AstDomCompiler_PlusPlus_MinusMinus()
    {
      IScriptContext context = new ScriptContext();

      CodeProgram domTree = AstDomCompiler.Compile(Script.Compile("sum=0;for(i=0; i<10; i++) sum = sum + i;", null, false).Ast);
      CodeDomCompiler.Compile(domTree).Execute(context);

      Assert.Equal(45, context.GetItem("sum", true));

      domTree = AstDomCompiler.Compile(Script.Compile("sum=0;for(i=9; i>=0; i--) sum = sum + i;", null, false).Ast);
      CodeDomCompiler.Compile(domTree).Execute(context);

      Assert.Equal(45, context.GetItem("sum", true));
    }

    [Fact]
    public void AstDomCompiler_InvokeMember()
    {
      int[] a = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
      int[] b = new int[9];

      IScriptContext context = new ScriptContext();
      context.SetItem("a", a);
      context.SetItem("b", b);
      
      CodeProgram domTree = AstDomCompiler.Compile(Script.Compile("return a.CopyTo(b, 0);", null, false).Ast);
      ExecutableMachine vm = CodeDomCompiler.Compile(domTree);
      vm.Execute(context);

      Assert.Equal(a[4], b[4]);
    }

    [Fact]
    public void AstDomCompiler_InvokeMember1()
    {
      VM_Test1 vt1 = new VM_Test1();
      
      IScriptContext context = new ScriptContext();
      context.SetItem("v", vt1);

      CodeProgram domTree = AstDomCompiler.Compile(Script.Compile("return v.GetNextLevel().GetNextLevel();", null, false).Ast);
      ExecutableMachine vm = CodeDomCompiler.Compile(domTree);
      vm.Execute(context);

      Assert.Equal(2, ((VM_Test1)context.Result).Level);
    }

    [Fact]
    public void AstDomCompiler_InvokeMember3()
    {
      VM_Test1 vt1 = new VM_Test1();

      IScriptContext context = new ScriptContext();
      context.SetItem("v", vt1);

      CodeProgram domTree = AstDomCompiler.Compile(Script.Compile("return v.Next.Next;", null, false).Ast);
      ExecutableMachine vm = CodeDomCompiler.Compile(domTree);
      vm.Execute(context);

      Assert.Equal(2, ((VM_Test1)context.Result).Level);
    }
  }
}
