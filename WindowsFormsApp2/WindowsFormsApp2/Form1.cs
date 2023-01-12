using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            clone();
        }

        private static void clone()
        {
            string wkDir = @"C:\Monitor\gittest";
            string url = "https://github.com/Faye0910/test.git";
            CloneOptions co = new CloneOptions
            {
                CredentialsProvider = (x, y, z) => new UsernamePasswordCredentials { Username = "Faye0910", Password = "Z;m30910" }
            };
            //Repository.Init(wkDir);
            Repository.Clone(url, wkDir, co);

        }
    }
}
