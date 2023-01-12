using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp_BarTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private delegate void SetPos(int ipos, string vinfo);

        private void SetTextMesssage(int ipos, string vinfo)
        {
            if (this.InvokeRequired)
            {
                SetPos setpos = new SetPos(SetTextMesssage);
                this.Invoke(setpos, new object[] { ipos, vinfo });
            }
            else
            {
                label1.Text = ipos.ToString() + "/1000";
                progressBar1.Value = Convert.ToInt32(ipos);
                textBox1.AppendText(vinfo);
            }
        }

        private void SleepT()
        {
            /*for (int i = 0; i < 500; i++)
            {
                System.Threading.Thread.Sleep(10);
                SetTextMesssage(100 * i / 500, i.ToString() + "\r\n");
            }*/

            for(int i=0;i<39;i++)
            {
                SetTextMesssage(i, i.ToString() + "\r\n");
            }    
        }

        private void button1_Click(object sender, EventArgs e)
        {

            progressBar1.Maximum = 38;
            progressBar1.Minimum = 0;
            Thread fThread = new Thread(new ThreadStart(SleepT));
            fThread.Start();
        }
    }
}
