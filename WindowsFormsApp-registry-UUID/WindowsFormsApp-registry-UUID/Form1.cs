using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp_registry_UUID
{
    public partial class Form1 : Form
    {
        RegistryKey RegistryInfo = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);

        public Form1()
        {
            InitializeComponent();
        }

        private void registrybtn_Click(object sender, EventArgs e)
        {
            string UUIDPath = (string)Registry.GetValue(@"HKEY_CURRENT_USER\\SOFTWARE\\Menza", "uuid", null);
            if (UUIDPath != null)
            {
                RegistryInfo.CreateSubKey("Software", true);
                RegistryInfo.CreateSubKey("Menza", true);
                string value = Convert.ToString(Guid.NewGuid().ToString());
                RegistryInfo.SetValue("uuid", value);
                Console.WriteLine("SET:   " + value);
            }
            else
            {

                Console.WriteLine("GET:   "+Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Menza", "uuid", "its not found"));
                RegistryInfo.Close();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            RegistryInfo.Close();
        }
    }
}