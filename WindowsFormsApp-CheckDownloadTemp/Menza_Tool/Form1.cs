/*
 * Menza_Tool should be execute first
 * author:Faye Lin
 * Time: 2022/08/10
 */

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Menza_Tool
{
    public partial class Form1 : Form
    {
        string Sourcedir = @"C:\Monitor\gittest";
        string downloadtemp= @"S:\Profile\Download\Monitor\";
        progressForm progress = new progressForm();
        SetupIniIP ini = new SetupIniIP();
        bool Have_File=false;
        string filename_ini = "";

        public Form1()
        {
            InitializeComponent();
            start();
        }

        /// <summary>
        /// 程式開始須執行
        /// </summary>
        void start()
        {
            Check_file.Start();

            WindowState = FormWindowState.Minimized;
            notifyIcon1.Icon = SystemIcons.Asterisk;
            notifyIcon1.Icon = new Icon(@"Floder.ico");
        }
        
        /// <summary>
        /// 檢查是否有有檔案
        /// </summary>
        private void Check_file_Tick(object sender, EventArgs e)
        {
            if (Directory.Exists(Sourcedir))
            { 
                if (Directory.GetDirectories(Sourcedir).Length != 0 || Directory.GetFiles(Sourcedir).Length != 0)
                    Have_File=true;
            }
        }

        /// <summary>
        /// 複製檔案
        /// </summary>
        private void CopyDirectory(string SourcePath, string DestinationPath, bool overwriteexisting)
        {
            try
            {
                if (Directory.Exists(SourcePath))
                {
                    if (Directory.Exists(DestinationPath) == false)
                        Directory.CreateDirectory(DestinationPath);

                    progress.set(0, Directory.GetFiles(SourcePath).Length);
                    foreach (string fls in Directory.GetFiles(SourcePath))
                    {
                        FileInfo flinfo = new FileInfo(fls);
                        flinfo.CopyTo(DestinationPath+"\\" + flinfo.Name, true);
                        progress.Addprogess();
                    }
                    foreach (string drs in Directory.GetDirectories(SourcePath))
                    {
                        DirectoryInfo drinfo = new DirectoryInfo(drs);
                        CopyDirectory(drs, DestinationPath + "\\" + drinfo.Name, true);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void openMonitorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(Environment.CurrentDirectory+@"\Menza_Main.exe");
            startInfo.UseShellExecute = false;
            startInfo.Arguments = "MonitorStart";
            Process.Start(startInfo);
        }

        /// <summary>
        /// For Test -要更新時的操作
        /// </summary>
        private void updatetestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Have_File)
            {
                Check_file.Stop();
                progress.Show();
                CopyDirectory(Sourcedir, downloadtemp, true);
                progress.Close();

                var result = MessageBox.Show("檔案升級完成即將重新啟動", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                    MessageBox.Show("執行關機function");
                Check_file.Start();
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
        /// 取得INI檔案內的IP
        /// </summary>
        void IPsetup()
        {
            try
            {
                if (File.Exists(Application.StartupPath + "\\" + filename_ini))
                    ini.IniReadValue("CONFIG", "HOST", filename_ini);
            }
            catch (Exception)
            {
                
            }
        }
    }
}
