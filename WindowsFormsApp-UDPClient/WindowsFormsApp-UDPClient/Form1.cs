using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }
        bool flag = true;
        Thread thread;
        UdpClient udp;

        private void button2_Click_1(object sender, EventArgs e)
        {
            flag = false;
            if (thread.ThreadState == ThreadState.Running)
                thread.Abort();
            udp.Close();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            textBox1.AppendText("Start Send..");
            Thread sendThread = new Thread(new ThreadStart(sendthread));
            sendThread.IsBackground = true;
            sendThread.Start();
        }

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
                    id = "TWAA000"
                })
            });


            byte[] buf = Encoding.Default.GetBytes(obj.ToString());
            IPEndPoint ipendpoint = new IPEndPoint(IPAddress.Broadcast, 80);
            while (true)
            {
                udpsend.Send(buf, buf.Length, ipendpoint);
                Console.WriteLine( buf.ToString());
                Thread.Sleep(1000);
            }
        }

    }
}