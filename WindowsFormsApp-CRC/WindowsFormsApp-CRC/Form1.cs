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

namespace WindowsFormsApp_CRC
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Rec_CRC(8);
        }

        void Rec_CRC(int len)
        {
            int data = 0b10010110;
            int ploy = 0b0010;
            ploy <<= 4;
            Console.WriteLine($"第0次運算結果：" + Convert.ToString(data, 2));
            for (int i = 0; i < len; i++)
            {
                if ((data & 0b10000000) == 0b10000000)
                {
                    data = (data << 1) ^ ploy;
                }
                else
                {
                    data <<= 1;
                }
                Console.WriteLine($"第{i + 1}次運算結果：" + Convert.ToString(data, 2));
            }
            Console.WriteLine($" 最終運算結果：" + Convert.ToString(data, 2));
        }

        void Dowload()
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

    }
}
