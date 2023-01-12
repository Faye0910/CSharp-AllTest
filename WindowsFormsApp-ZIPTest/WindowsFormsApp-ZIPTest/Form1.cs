using Ionic.Zip;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace WindowsFormsApp_ZIPTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)  //壓縮
        {
            /*using (ZipFile zip = new ZipFile())
            {
                zip.Password = "P@ssW0rd";//壓縮的密碼
                zip.AddEntry("123.txt", "111111111111555555555555555555\r\n 4444444"); //要壓縮的目路
                zip.Save("test.zip"); //存在exe那邊
            }
            this.Close();*/

            using (ZipFile zip = new ZipFile())
            {
                zip.Password = "P@ssW0rd";//壓縮的密碼
                zip.AddFile("20220722141245-0e6e259b-2d31-4120-84af-845305d2227e.png");
                zip.Save("123.zip"); //存在exe那邊
            }
        }

        private void button2_Click(object sender, EventArgs e)  //解壓縮
        {
            UnZipFiles("123.zip", "P@ssW0rd");//ATM2.0.3.7_ForUpdate.zip
        }

        private void UnZipFiles(string path, string password)
        {
            ZipFile unzip = ZipFile.Read(path);
            if (password != null && password != string.Empty) unzip.Password = password;
            string unZipPath = path.Replace(".zip", "");

            foreach (ZipEntry e in unzip)
            {
                e.Extract(unZipPath, ExtractExistingFileAction.OverwriteSilently);
            }
        }
        void Menza_Dowload()
        {
            try
            {
                WebClient mywebClient = new WebClient();
                mywebClient.DownloadFile("http://mysite.com/myfile.txt", @"d:\myfile.txt");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Menza_Dowload();
        }
    }
}
