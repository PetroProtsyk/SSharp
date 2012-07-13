using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Scripting.SSharp.Runtime;
using Scripting.SSharp;

namespace Debug.CF
{
  public partial class Form1 : Form
  {
    private bool initialized = false;

    public Form1()
    {
      InitializeComponent();
    }

    private void menuItem2_Click(object sender, EventArgs e)
    {
      Application.Exit();
    }

    private void menuItem1_Click(object sender, EventArgs e)
    {
      if (!initialized)
      {
        textBox2.Text = "Initializing...";
        Application.DoEvents();

        RuntimeHost.Initialize();
        initialized = true;
      }

      try
      {
        object result = Script.RunCode(textBox1.Text);
        if (result != null)
        {
          textBox2.Text = "Result: " + result.ToString();
        }
        else
        {
          textBox2.Text = "Result: Null";
        }
      }
      catch(Exception ex)
      {
        textBox2.Text = "Exception: " + ex.Message;
      }
    }
  }
}