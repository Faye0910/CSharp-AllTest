using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp_GitVersionNum
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void Insert_Text(string content)
        {
            int count = textBox1.Text.Length;
            string get_box_text = textBox1.Text;
            textBox1.Text = get_box_text.Insert(count, content + "\r\n");
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.ScrollToCaret();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //錯誤的
            string strUrl = @"https://github.com/Faye0910/Test.git";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);
            request.Timeout = 10000;
            request.Method = "GET";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; Windows NT 5.2; Windows NT 6.0; Windows NT 6.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727; MS-RTC LM 8; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET CLR 4.0C; .NET CLR 4.0E)";
            HttpWebResponse webresponse = (HttpWebResponse)request.GetResponse();
            StreamReader streamReader = new StreamReader(webresponse.GetResponseStream(), Encoding.UTF8);
            string retString = streamReader.ReadToEnd();
            webresponse.Close();
            streamReader.Close();
            string Compare = "";
            foreach (Match match in Regex.Matches(retString, @"[0-9]*\.[0-9]*\.[0-9_]*"))
            {
                string Find_Result = match.Value.ToString();
                if (Find_Result == Compare)
                { }
                else
                {
                    Insert_Text("版本號碼：" + Find_Result);
                    Compare = Find_Result;
                }
            }
        }

    }
}
