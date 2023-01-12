using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace WindowsFormsApp_ForMerge
{
    public partial class Form1 : Form
    {
        public string label3txt { set => Invoke(new Action(() => { label3.Text = value; })); }
        public bool btnEnable { set => Invoke(new Action(() => { button1.Enabled = value; })); }
        public int progressBar1Maximum { set => Invoke(new Action(() => { progressBar1.Maximum = value; })); }
       public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            progressBar1.Value = 0;
            progressBar1.Minimum = 0;
            Thread thread =new Thread(Merge);
            thread.Start();
        }

        void Merge()
        {
            string sourceFolder = textBox1.Text;
            List<string> file = new List<string>(Directory.GetFiles(sourceFolder));
            progressBar1Maximum= file.Count;
            file.Sort();

            string day =sourceFolder.Substring((sourceFolder.LastIndexOf(@"\")) + 1);
            string source = sourceFolder.Substring(0, sourceFolder.Length - day.Length - 1);
            string name = source.Substring(source.LastIndexOf(@"\") + 1);

            string output = Environment.CurrentDirectory + "\\" +day +"_"+name +"Log.txt";
            Console.WriteLine(output);

            if (File.Exists(output))
                File.Delete(output);
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(output))
            {
                foreach (string t in file)
                {
                    Thread thread=new Thread(AddProgressBar);
                    thread.Start();
                    Console.WriteLine(progressBar1.Value);
                    sw.WriteLine(t.Substring(t.LastIndexOf('\\') + 1) + "\t" + (File.ReadAllText(t)).Replace("\r\n", " "));
                }
            }

            //MessageBox.Show("合併完成");
            file.Clear();
            btnEnable = true;
        }

        void AddProgressBar()
        {
            Invoke(new Action(() =>
            {
                progressBar1.Value++;
                label4.Text=(progressBar1.Value).ToString()+"/"+"共"+progressBar1.Maximum;
            }));
            
        }
    }
}
