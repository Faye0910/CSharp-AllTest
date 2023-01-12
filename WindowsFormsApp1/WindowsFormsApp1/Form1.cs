using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using VisioForge.Libs.NAudio.CoreAudioApi;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public static Volume volume = new Volume();
        string UUID = (string) Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Menza", "uuid", "");
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 資料夾名稱串接排序
        /// </summary>
        /// <param name="OrderPath">  Order的資料夾  </param>
        List<string> SortOrder(string OrderPath)
        {
            List<string> o = new List<string>();
            string datenow = DateTime.Now.ToString("yyyy-MM-dd");
            string[] datetime = datenow.Split('-');

            List<string> files = new List<string>(Directory.GetDirectories(OrderPath));
            
            files.Sort((x,y)=>-x.CompareTo(y));
            foreach (string year in files)
            {
                List<string> file = new List<string>(Directory.GetDirectories(year));
                file.Sort((x, y) => -x.CompareTo(y));
                foreach (string month in file)
                {
                    
                    List<string> _file = new List<string>(Directory.GetDirectories(month));
                    _file.Sort((x, y) => -x.CompareTo(y));
                    foreach (string day in _file)
                    {
                        List<string> orders = new List<string>(Directory.GetFiles(day));
                        orders.Sort((x, y) => x.CompareTo(y));
                        foreach (string order in orders)
                        {
                            //string getMergedate = year.Substring((year.LastIndexOf('\\') + 1), 4) + "-" + month.Substring((month.LastIndexOf('\\') + 1), 2) + "-" + day.Substring((day.LastIndexOf('\\') + 1), 2);
                            o.Add(order);//新增到List裡面
                            Console.WriteLine(order);//order.Substring((order.LastIndexOf('\\') + 1), order.Length-(order.LastIndexOf('\\') + 1))
                        }
                    }
                }
            }

            return o;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SortOrder(@"C:\Users\Faye.Lin\source\ATM Clone\Order");

        }
        private void button1_Click(object sender, EventArgs e)
        {//測試解json --版更的
            string jsonText = "[{'part_type_id':'19','version':'2.0.3.8','account_day':'2022-10-14','url':''}," +
                                "{'part_type_id':'16','version':'1.2.3.0','account_day':'2022-10-14','url':''}]";
            var mJObj = JArray.Parse(jsonText);

            JObject obj = JObject.FromObject(new
            {
                @event = "request:api",
                action = "1116",
                success=true,
                code=0,
                Message="",
                data = JObject.FromObject(new
                {
                    schedules =mJObj

                    
                })
            });

            if(obj["data"]["schedules"].Count()>0)
                Console.WriteLine(obj["data"]["schedules"].Count());

            if (obj["data"]["schedules"].Count() > 0)
            {
                for (int len=0;len<obj["data"]["schedules"].Count();len++)
                {
                    string updateDay = obj["data"]["schedules"][len]["account_day"].ToString();
                    string verison = obj["data"]["schedules"][len]["version"].ToString();
                    string url = obj["data"]["schedules"][len]["url"].ToString();

                    Console.WriteLine(len + "  : " + updateDay + "  " + verison + "  " + url);

                    if (updateDay == DateTime.Now.ToString("yyyy-MM-dd"))
                    {
                        //byte[] bytes = SendVersionjson(2, version);
                        //_udpSocket.Send(bytes);
                        Console.WriteLine("Today");
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //volume.SetVolume = 80;
            volume.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {

            AskATMUpdate(UUID);
        }

        public static void AskATMUpdate(string uuid)
        {
            JObject obj = JObject.FromObject(new //回應Cashier ATM訊問版本
            {
                success = true,
                action = "505",
                command = 1,
                code = 0,
                message = ""
            });


            byte[] buf = Encoding.Default.GetBytes(obj.ToString());
            Console.WriteLine(obj.ToString());
        }

    }
}
