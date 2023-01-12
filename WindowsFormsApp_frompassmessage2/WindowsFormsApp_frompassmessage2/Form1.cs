using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp_frompassmessage2
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd,int Msg,int wParam,int lParam);

        public string BoxRec { set => Invoke(new Action(() => { textBox1.Text = value; })); }
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 開啟接收Windows訊息(包含接收後傳送)以及UDP的訊息
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.AppendText("Start Rec..");
            Thread recThread = new Thread(new ThreadStart(Rec));
            recThread.IsBackground = true;
            recThread.Start();
        }

        /// <summary>
        /// 接收Windows訊號
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            IntPtr WINDOW_HANDLER = FindWindow(null, "ABC");//不可設為全域
            switch (m.Msg)
            {
                case 0x0100:
                    textBox2.AppendText("收到" + "0x0100"+"\r\n");
                    SendMessage(WINDOW_HANDLER, 0x0100, 0, 0);
                    break;
                case 0x0300:
                    //MessageBox.Show("收到關閉ATM");
                    Environment.Exit(0);
                    break;
                case 0x0600:
                    MessageBox.Show("收到重啟Windows");
                    SendMessage(WINDOW_HANDLER, 0x0700, 0, 0);
                    break;

                default:
                    break;
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// 接收UDP訊息
        /// </summary>
        void Rec()
        {
            UdpClient udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 2236));
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 2237);
            while (true)
            {
                if (udpClient.Client == null)
                    return;
                byte[] buf = udpClient.Receive(ref endPoint);
                string msg = Encoding.Default.GetString(buf);
                BoxRec = msg;
            }
        }
    }
}
