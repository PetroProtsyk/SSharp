using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.UIAutomation
{
  public sealed class UIAutomationFunctions : FunctionTable
  {
    public UIAutomationFunctions()
    {
      this
       .AddFunction<KillFunction>("Kill")
       .AddFunction<LaunchFunction>("Launch")
       .AddFunction<WaitFunction>("Wait")
       .AddFunction<FindByClassNameFunction>("FindByClassName")
       .AddFunction<FindByIdFunction>("FindById")
       .AddFunction<FindByNameFunction>("FindByName")
       .AddFunction<ExpandFunction>("Expand")
       .AddFunction<InvokeByIdFunction>("InvokeById")
       .AddFunction<FocusEditorFunction>("FocusEditor")
       ;
    }
  }
}
