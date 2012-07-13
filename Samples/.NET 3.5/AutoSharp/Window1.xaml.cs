using System.Windows;
using Scripting.SSharp;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.UIAutomation;

namespace AutoSharp
{
  /// <summary>
  /// Interaction logic for Window1.xaml
  /// </summary>
  public partial class Window1 : Window
  {
    string testcase =
      @"
              // Close existing instances of Notepad
              Kill(""notepad""); 

              // Launch a new Notepad instance and get main window
              window = Launch(""notepad"");
              // Wait 1 second
              Wait(1000);

              // Get main editor region
              edit = FindByClassName(window, ""Edit"");

              // focus main editor
              FocusEditor(edit);

              // Send sample text to the editor region
              SendKeys.SendWait(""Automating Notepad using Windows UI Automation and S#"");
              Wait(3000);

              // Find [File] menu
              mnuFile = FindById(window, ""Item 1"");

              // Expand [File] menu
              Expand(mnuFile);
              Wait(1000);

              // Invoke [Save As] menu item
              InvokeById(window, ""Item 4"");
              Wait(1000);

              // Get [Save As] dialog
              saveAsDialog = FindByName(window, ""Save As"");

              // Get access to [FileName] textbox
              saveAsName = FindById(saveAsDialog, ""1001"");

              // Focus filename editor              
              FocusEditor(saveAsName);
              
              // Write down file name
              SendKeys.SendWait(""D:\\MyTextFile"");
              // Send [Enter] keypress           
              SendKeys.SendWait(""{ENTER}"");
              Wait(1000);                

              // Check whether Overwrite Dialog appeared
              confirmSaveAs = FindByName(saveAsDialog, ""Confirm Save As"");
              if (confirmSaveAs != null)
              {
                  // Click [OK] button
                  InvokeById(confirmSaveAs, ""CommandButton_6"");
                  Wait(1000);
              }

              // Expand [File] menu
              Expand(mnuFile);
              Wait(1000);

              // Click [Exit] item
              InvokeById(window, ""Item 7""); 
        ";

    private IScriptContext context;

    public Window1()
    {
      RuntimeHost.Initialize();

      InitializeComponent();

      context = new ScriptContext();
      context.Import<UIAutomationFunctions>();      
            
      this.editor.Text = testcase;
    }

    private void btnExecute_Click(object sender, RoutedEventArgs e)
    {
      Script.RunCode(testcase, context);
    }
  }
}
