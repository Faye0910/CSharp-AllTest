using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Upload
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                WebClient client = new WebClient();
                string myFile = @"C:\Users\Faye.Lin\source\repos\for test\Upload\Upload\bin\Debug\2022-10-10MenzaConsolerecordLog.zip";
                client.Credentials = CredentialCache.DefaultCredentials;
                byte[] responseArray = client.UploadFile(@"http://192.168.1.52/upload/upload-vsc-v2.php", "POST", myFile);
                Console.WriteLine("\nResponse Received. The contents of the file uploaded are:\n{0}",System.Text.Encoding.UTF8.GetString(responseArray));
                client.Dispose();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }

            var uuid = Guid.NewGuid().ToString();
            Console.WriteLine(uuid);
        }
    }
}
