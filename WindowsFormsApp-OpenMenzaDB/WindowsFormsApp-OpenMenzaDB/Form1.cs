using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp_OpenMenzaDB
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static void OpenBrowserUrl(string url)
         {
             try
             {
                 // 64位注冊表路徑
                 var openKey = @"SOFTWARE\Wow6432Node\Google\Chrome";
                 if (IntPtr.Size == 4)
                 {
                      openKey = @"SOFTWARE\Google\Chrome";
                  }
                  RegistryKey appPath = Registry.LocalMachine.OpenSubKey(openKey);
                  if (appPath != null)
                  {
                      var result = Process.Start("chrome.exe", url);
                 }
             }
              catch
              {
                MessageBox.Show("ERROR");
               }
         }
        private void button1_Click(object sender, EventArgs e)
        {
            OpenBrowserUrl("http://menzatest.laxan.com.tw/device/device");
        }

    }
}
