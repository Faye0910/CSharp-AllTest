using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp_UDPServer
{
    public partial class Form1 : Form
    {
        public string BoxSend { set => Invoke(new Action(() => { textBox1.Text = value; })); }
        public string BoxRec { set => Invoke(new Action(() => { textBox2.Text = value; })); }
        
        public Form1()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// 啟動sendthread
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.AppendText("Start Send..");
            Thread sendThread = new Thread(new ThreadStart(sendthread));
            sendThread.IsBackground = true;
            sendThread.Start();
            
        }
        /// <summary>
        /// 廣播傳送訊息
        /// </summary>
        void sendthread()
        {
            UdpClient udpsend = new UdpClient(new IPEndPoint(IPAddress.Any, 888));

            JObject obj = JObject.FromObject(new
            {
                action = "5001",
                client_uuid = "e25a352-339c-4d4c-a50b-889f93db7e0e",
                data = JObject.FromObject(new
                {
                    code = 1,
                    id="TWAA000"
                })
            });


            byte[] buf = Encoding.Default.GetBytes("This is UDP test");
            IPEndPoint ipendpoint = new IPEndPoint(IPAddress.Broadcast, 80);
            while (true)
            {
                udpsend.Send(buf, buf.Length, ipendpoint);
                BoxSend= buf.ToString();
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 接收訊息
        /// </summary>
        void recthread()
        {
            UdpClient udpClient=new UdpClient(new IPEndPoint(IPAddress.Any, 2237));
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 2237);
            while(true)
            {
                if (udpClient.Client == null)
                    return;
                byte[] buf =udpClient.Receive(ref endPoint);
                string MSG =Encoding.Default.GetString(buf);
                

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.AppendText("Start Rec..");
            Thread recThread = new Thread(new ThreadStart(recthread));
            recThread.IsBackground = true;
            recThread.Start();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            SendSureUpdate();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            byte[] a = AskNewVersion(1);
            ConsoleSocket.Send(a);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ConsoleInitialize();
            //timer1.Start();

            var obj = (JArray)JsonConvert.DeserializeObject(File.ReadAllText(Environment.CurrentDirectory + @"\" + "schedules.txt"));
            Console.WriteLine(obj[0]["schedule_id"].ToString());
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Console.WriteLine(aa);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(aa==true)
                aa=false;
            else
                aa=true;
                
        }
    }
}
