using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp_AnyDesk
{
    public partial class Form1 : Form
    {
        private DataReceivedEventHandler p_OutputDataReceived;
        private DataReceivedEventHandler p_ErrorDataReceived;
        object Installsouce = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AnyDeskCommand("--set-password");//echo <my_password> AnyDesk-bbac4dcb.exe --set-password
        }

        void AnyDeskCommand(string command)
        {
            using (Process p = new Process())
            {
                // set start info
                string txt = Installsouce.ToString();
                string dir = txt.Substring(1, txt.Length - 2) + "\\ ";
                string name = txt.Substring(txt.LastIndexOf('\\') + 1, txt.Length - txt.LastIndexOf('\\') - 2);
                p.StartInfo = new ProcessStartInfo(@"C:\Windows\System32\cmd.exe")//getid.bat
                {
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    WorkingDirectory = dir,
                };
                // event handlers for output & error
                p.OutputDataReceived += p_OutputDataReceived;
                p.ErrorDataReceived += p_ErrorDataReceived;

                // start process
                p.Start();
                // send command to its input
                p.StandardInput.Write(name + ".exe " + command + p.StandardInput.NewLine);
                Console.WriteLine(name + ".exe " + command);
                //wait
                p.CloseMainWindow();
                p.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (checkAnyDeskInstall() == true)
                MessageBox.Show("有安裝 AnyDesk ");
            else
                MessageBox.Show("沒有安裝 AnyDesk ");

            if (CheckAnyDeskRun() == true)
                MessageBox.Show("有執行 AnyDesk ");
            else
                MessageBox.Show("沒有執行 AnyDesk ");
        }

        /// <summary>
        /// 確認是否有安裝AnyDesk
        /// </summary>   
        /// <returns>true: 有安裝, false:沒有安裝</returns>
        private bool checkAnyDeskInstall()
        {
            Microsoft.Win32.RegistryKey uninstallNode = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            foreach (string subKeyName in uninstallNode.GetSubKeyNames())
            {
                Microsoft.Win32.RegistryKey subKey = uninstallNode.OpenSubKey(subKeyName);
                object displayName = subKey.GetValue("DisplayName");
                if (displayName != null)
                {
                    if (displayName.ToString().Contains("AnyDesk"))
                    {
                        Installsouce = subKey.GetValue("InstallLocation");
                        return true;
                    }
                }
            }
            return false;
        }

        private bool CheckAnyDeskRun()
        {
            Process[] processes = Process.GetProcessesByName("AnyDesk-bbac4dcb");

            if (processes.Length == 0)
            {
                Console.WriteLine("Not running");
                return false;
            }
            else
            {
                Console.WriteLine("Running");
                return true;
            }
        }

        public void WriteBATFile(string fileContent)
        {
            string filePath = "S:\\Profile\\Desktop\\testChange.bat";
            if (!File.Exists(filePath))
            {
                FileStream fs1 = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs1);
                sw.WriteLine(fileContent);
                sw.Close();
                fs1.Close();
            }
            else
            {
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Write);
                StreamWriter sr = new StreamWriter(fs);
                sr.WriteLine(fileContent);
                sr.Close();
                fs.Close();
            }
        }
    } 
}
