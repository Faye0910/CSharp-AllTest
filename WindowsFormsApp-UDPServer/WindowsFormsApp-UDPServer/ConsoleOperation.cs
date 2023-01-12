using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp_UDPServer
{
    public partial class Form1 : Form
    {
        public static bool aa = false;
        public bool btnnewversion { set => Invoke(new Action(() => { btnNewVersion.Visible = value; })); }
        string UUID = (string)Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Menza", "uuid", "");
        UdpSocket ConsoleSocket { get; set; }

        void ConsoleInitialize()
        {
            ConsoleSocket = new UdpSocket(2237, 2236);
            ConsoleSocket.NewMessage += ProcessConsoleMessage;
            ConsoleSocket.Open();
        }

         void ProcessConsoleMessage(string message)
        {
            try
            {
                string prefix = string.Format("Reveive From Cashier({0}):\r\n", DateTime.Now.ToString("HH:mm:ss.ffff"));
                Console.WriteLine(prefix + message);

                dynamic receive = JObject.Parse(message);
                string action = receive.action;
                if (action == "505")
                    ConsoleMessage(message);
            }
            catch (Exception ex)
            {
            }
        }

        void ConsoleMessage(string MSG)
        {
            try
            {
                if (MSG.Contains("data"))
                {
                    JObject json = JObject.Parse(MSG);
                    if (json["action"].ToString() == "505")
                    {
                        string msg = "";
                        if (json["data"]["command"].ToString() != "")
                            msg = json["data"]["command"].ToString();
                        if (msg == "2")
                        {
                            btnnewversion = true;
                            string version = json["data"]["version"].ToString();
                            Console.WriteLine("新版本是:  " + version);
                            ConsoleSocket.Send(RecNewversion(2));
                        }
                        if (msg == "3")
                        {
                            ConsoleSocket.Send(RecStop(3));
                            Environment.Exit(0);
                        }
                    }
                }
                else
                {
                    JObject json = JObject.Parse(MSG);
                    if (json["action"].ToString() == "505")
                    {
                        if (json["command"].ToString() =="1" && json["success"].ToString() == "True")
                            Console.WriteLine("有排程");
                        if(json["command"].ToString() == "1" && json["success"].ToString() == "false")
                            Console.WriteLine("無排程");
                    }
                }
                
            }
            catch
            {

            }
            
        }

        byte[] AskNewVersion(int code)
        {
            JObject obj = JObject.FromObject(new
            {
                action = "505",
                client_uuid = "123459786134",
                data = JObject.FromObject(new
                {
                    command = code,
                })
            });

            byte[] buf = Encoding.Default.GetBytes(obj.ToString());
            return buf;
        }

        byte[] RecNewversion(int code)
        {
            JObject obj = JObject.FromObject(new
            {
                success = true,
                action = "505",
                code = 0,
                message = "",
                data = JObject.FromObject(new
                {
                    command = code
                })
            });

            byte[] buf = Encoding.Default.GetBytes(obj.ToString());

            return buf;
        }

        byte[] RecStop(int code)
        {
            byte[] buf = null;
            JObject obj = JObject.FromObject(new
            {
                success = true,
                action = "505",
                command = code,
                code = 0,
                message = ""
            });

            buf = Encoding.Default.GetBytes(obj.ToString());

            return buf;
        }

        byte[] sureUpdate()
        {
            byte[] buf = null;
            try
            {
                JObject obj = JObject.FromObject(new
                {
                    action = "505",
                    client_uuid = UUID,
                    data = JObject.FromObject(new
                    {
                        command = 4,
                    })
                });

                buf = Encoding.Default.GetBytes(obj.ToString());
            }
            catch (Exception ex)
            {
                
            }
            return buf;
        }

        void SendSureUpdate()
        {
            ConsoleSocket.Send(sureUpdate());
        }
    }
}
