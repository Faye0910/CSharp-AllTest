using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApp_PushLog
{
    public partial class Form1 : Form
    {
        bool TodayDo = false;
        SetupIniIP ini = new SetupIniIP();
        string filename_ini = @"conf\setup.ini";

        string LastDoneDay = "";
        public Form1()
        {
            InitializeComponent();
            IPSetup();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string time = DateTime.Now.ToString("HH");
            int t= Convert.ToInt32(time);
            string day = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

            if (day == LastDoneDay)
                TodayDo = true;

            if (t < 8)
                TodayDo = false;

            if (t>8 && TodayDo==false)
            {
                Thread thread = new Thread(new ThreadStart(Merge));
                thread.Start();
            }
        }

        /// <summary>
        /// 設定ini檔
        /// </summary>
        public class SetupIniIP
        {
            public string path;
            [DllImport("kernel32", CharSet = CharSet.Unicode)]
            private static extern long WritePrivateProfileString(string section,
            string key, string val, string filePath);
            [DllImport("kernel32", CharSet = CharSet.Unicode)]
            private static extern int GetPrivateProfileString(string section,
            string key, string def, StringBuilder retVal,
            int size, string filePath);
            public void IniWriteValue(string Section, string Key, string Value, string inipath)
            {
                WritePrivateProfileString(Section, Key, Value, Application.StartupPath + "\\" + inipath);
            }
            public string IniReadValue(string Section, string Key, string inipath)
            {
                StringBuilder temp = new StringBuilder(255);
                int i = GetPrivateProfileString(Section, Key, "", temp, 255, Application.StartupPath + "\\" + inipath);
                return temp.ToString();
            }
        }

        /// <summary>
        /// 取得INI檔案內的IP 以及用於clone
        /// </summary>
        void IPSetup()
        {
            try
            {
                if (File.Exists(Application.StartupPath + "\\" + filename_ini))
                {
                    LastDoneDay=ini.IniReadValue("FILE", "WKDIR", filename_ini);

                    string d = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

                    if (d == LastDoneDay)
                        TodayDo = true;
                    else
                    {
                        Thread thread = new Thread(new ThreadStart(Merge));
                        thread.Start();
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// log合併
        /// </summary>
        void Merge()
        {
            TodayDo = true;

            string sourceFolder = @"S:\Profile\Download\Log";
            string output = "";
            string dir = "";

            if (!Directory.Exists(sourceFolder))
            {
                Console.WriteLine($"Source Folder Not Exist!");
                return;
            }

            List<string> files = new List<string>(Directory.GetDirectories(sourceFolder));
            files.Sort();
            string text = "#";
            foreach (string file in files)
            {
                List<string> fi = new List<string>(Directory.GetDirectories(file));
                fi.Sort();
                int len = file.Length - (file.LastIndexOf('\\') + 1);
                string Name=file.Substring(file.LastIndexOf('\\')+1,len);
                foreach (string f in fi)
                {
                    List<string> ff = new List<string>(Directory.GetFiles(f));
                    ff.Sort();
                    int Daylen = f.Length - (f.LastIndexOf('\\') + 1);
                    string day = f.Substring(f.LastIndexOf('\\') + 1, Daylen);
                    DateTime startdate = Convert.ToDateTime(LastDoneDay);
                    DateTime enddate = Convert.ToDateTime(day);
                    if ((enddate - startdate).Days <= 0)
                        continue;
                    foreach (string t in ff)
                    {
                        text +=
                        t.Substring(t.LastIndexOf('\\') + 1)
                        .PadRight(30, '-').PadLeft(40, '-')
                        + "\r\n";
                        text += File.ReadAllText(t);
                    }
                    dir = Environment.CurrentDirectory + "\\" + "Log" + "\\" + day;
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    output = dir+ "\\" + Name + "Log.txt";
                    using (StreamWriter sw = new StreamWriter(output, false, Encoding.UTF8))
                    {
                        sw.WriteLine(text);
                    }
                    ini.IniWriteValue("FILE", "WKDIR", day, filename_ini);
                }
            }
            
            files.Clear();
            Console.WriteLine("New File Created.");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}