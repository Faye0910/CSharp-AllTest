using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp_jsonTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            JObject obj2 = new JObject();
            JObject obj3 = new JObject();
            JArray list = new JArray();
            obj2.Add("C:", "111GB");
            obj3.Add("D:", "5GB");

            list.Add(obj2);
            list.Add(obj3);

            JObject obj = JObject.FromObject(new
            {
                command = "1",
                disk = list
            });


            string str = "{'event': 'response:api','action':'1118','success': true,'code': 0,'message': '','data': {'schedule': {'schedule_id': '1', 'part_type_id': '28','version': '2.0.3.8','account_day': '2022-10-14','target_path': 'c://atm', 'url':'zip url','crc':'', }}}";
            string str2 = "{'event': 'response:api','action':'1118','success': true,'code': 0,'message': '','data': {'schedule':null }}";

            var Data =JObject.Parse(str);

            var Data2 = JObject.Parse(Data["data"].ToString());
            if (Data2["schedule"].ToString() == "")
                Console.WriteLine("....");
            else
            {
                var Data3 = JObject.Parse(Data2["schedule"].ToString());

                Console.WriteLine(Data3);
            }
            
            /*

            JObject obj = JObject.FromObject(new
            {
                action = "5001",
                client_uuid = "sgahjdka-sdadsad5464",
                data = JObject.FromObject(new
                {
                    new_version = "12.34.5.6",
                    created_at = "xxxx-xx-xx xx:xx:xx"
                })
            });

            var result = JsonConvert.SerializeObject(obj);
            Console.WriteLine(result);*/
        }
        public new string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
