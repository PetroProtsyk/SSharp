using System.Windows.Controls;
using Scripting.SSharp;
using Scripting.SSharp.Runtime;

namespace Debug.Silverlight
{
  public partial class MainPage : UserControl
  {
    public MainPage()
    {
      InitializeComponent();

      RuntimeHost.Initialize();

      string code =
        @"a = [17,-2, 0,-3, 5, 3,1, 2, 55];

         for (i=0; i < a.Length; i=i+1)
          for (j=i+1; j <  a.Length; j=j+1)
           if (a[i] > a[j] )
           {
             temp = a[i]; 
             a[i] = a[j];
             a[j] = temp;
           }

          s = 'Results:';
          for (i=0; i < a.Length; i++)
           if (i!=0)
             s = s + ',' + a[i];
           else
             s += a[i];";

      Script s = Script.Compile(code);
      object rez = s.Execute();

      textBox1.Text = rez.ToString();
    }
  }
}
