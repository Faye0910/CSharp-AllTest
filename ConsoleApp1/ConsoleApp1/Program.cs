using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConsoleApp1
{
    public enum ConsoleCMD
    {
        None,
        QueryNewVersion,
        NewVersionDetected,
        CloseATM,
        ReadyToUpdate,
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            SendUDPToConsoleJson(ConsoleCMD.ReadyToUpdate);
            Console.ReadKey();
        }
        static byte[] SendUDPToConsoleJson(ConsoleCMD command)
        {
            byte[] buf = null;
            try
            {
                JObject obj = JObject.FromObject(new
                {
                    action = "505",
                    client_uuid = "12321",
                    data = JObject.FromObject(new
                    {
                        command = command,
                    })
                });
                Console.WriteLine(obj);
                buf = Encoding.Default.GetBytes(obj.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return buf;
        }
    }
}
