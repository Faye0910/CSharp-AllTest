using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp_GitClone
{
    public partial class Form1 : Form
    {
        string Url = "https://github.com/Faye-Laxan/ATM-Formal.git";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string branchName = "2.0.3.6";
            Repository.Clone(Url, Environment.CurrentDirectory+"\\ATM", new CloneOptions { BranchName = branchName });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string branchName = "Test";
            Repository.Clone(Url, Environment.CurrentDirectory + "\\ATM2", new CloneOptions { BranchName = branchName });
        }
    }
}
