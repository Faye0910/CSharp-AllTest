using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp_LogMerge_LohAnalyse
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int testten = 0;
        int testf = 0;
        int test20 = 0;
        int test50 = 0;
        int test100 = 0;
        int test200 = 0;
        int test500 = 0;
        int test1000 = 0;
        int error80 = 0;
        int error40 = 0;
        int billopen = 0;
        int coinopen = 0;

        int sum = 0;
        int token = 0;
        string Cashtxt = "";
        bool CashIn = false;
        bool CashState = false;
        int total_len = 0;
        string showtxt2str = "";
        string showtxt3str = "";
        int time = 0;

        public string Message { set => Invoke(new Action(() => { label1.Text = value; })); }
        public string BoxShow { set => Invoke(new Action(() => { textBox1.Text = value; })); }
        public string BoxShow2 { set => Invoke(new Action(() => { textBox2.Text = value; })); }
        public string BoxShow3 { set => Invoke(new Action(() => { textBox3.Text = value; })); }
        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            Merge();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Message = "Merging...";
            Task task = new Task(() => { Merge(); });
            task.Start();
        }

        void Merge()
        {
            string output = Environment.CurrentDirectory + "\\Log.txt";
            string path = Environment.CurrentDirectory + "\\2022-07-13";

            Directory.GetFiles(path, "*.txt", SearchOption.AllDirectories);

            List<string> files = new List<string>();

            files.AddRange(Directory.GetFiles(path));
            files.Sort();

            int a = 0;
            total_len=files.Count;
            timer1.Start();
            foreach (string file in files)
            {
                a++;
                string filename = file.Substring(file.LastIndexOf('\\') + 1, 11);
                string logtxt = File.ReadAllText(file);
                DiscriminantMoney(logtxt, filename);
            }

            BoxShow = string.Format("5塊:{0} \r\n10塊:{1} \r\n20塊:{2} \r\n50塊:{3} \r\n" +
                "100塊:{4} \r\n 200塊:{5} \r\n500塊:{6} \r\n1000塊:{7} \r\n總金額 : {8}\r\n " +
                "TotalToken : {9}\r\n總資料量 : {10}\r\nHOPPER_SENSOR : {11}\r\nTOKEN_LEAK : {12}\r\n" +
                "BILL_SERSOR : {13}\r\n COIN_SERSOR : {14}\r\n",
                testf, testten, test20, test50, test100, test200, test500, test1000, sum, token, total_len, error40, error80,
                billopen, coinopen);
            timer1.Stop();
            Console.WriteLine(time + "秒");
            BoxShow2 = showtxt2str;
            BoxShow3 = showtxt3str;
            Message = "New File Created.";
            files.Clear();
            alldel();

        }

        void DiscriminantMoney(string logtxt, string file)
        {
            if (logtxt.Length > 8 && logtxt.Substring(0, 8) == "90 06 12")
                coins_total(logtxt);
            else if (logtxt.Substring(0, 2) == "81")
            {
                if (CashState == true)
                {
                    CashState = false;
                    logtxt = "";
                    return;
                }
                if (logtxt.Length > 7)
                {
                    string[] data = logtxt.Replace("/r/n", "").Split();
                    if (data[3] == "02-")
                        CashState = true;

                }
                CashIn = true;
                Cashtxt += logtxt;
            }
            else if (logtxt.Substring(0, 3) == "02-")
            {
                if (logtxt.Length > 5)
                {
                    string[] data = logtxt.Replace("/r/n", "").Split();
                    if (data[2] == "10" && CashIn)
                        cash_total();
                }

                CashState = true;
                Cashtxt += logtxt;
            }
            else if (logtxt.Substring(0, 2) == "10")
            {
                if (CashState && CashIn)
                {
                    Cashtxt += logtxt;
                    cash_total();
                }

            }
            else if (logtxt.Substring(0, 2) == "5E")
            {


                if (logtxt.Substring(0, 3) == "5E-")
                    return;
                else
                {
                    CashIn = false;
                    CashState = false;
                    Cashtxt = "";

                }

            }
            else if (logtxt.Substring(0, 2) == "0F" || logtxt.Substring(0, 2) == "18" || logtxt.Substring(0, 2) == "0C" || logtxt.Substring(0, 2) == "30" ||
                logtxt.Substring(0, 2) == "3E" || logtxt.Substring(0, 2) == "4E" || logtxt.Substring(0, 2) == "11")
            {
                CashIn = false;
                CashState = false;
                Cashtxt = "";
            }

            if (logtxt.Length > 6)
            {
                if (logtxt.Substring(0, 5) == "F5 8A")
                {
                    byte[] txtToByte = HexToBytes(logtxt);
                    var error = (ErrorCode)txtToByte[11];

                    if (error.HasFlag(ErrorCode.HOPPER_SENSOR) == true)
                        error40++;

                    if (error.HasFlag(ErrorCode.TOKEN_LEAK) == true)
                        error80++;
                }

                if (logtxt.Substring(0, 5) == "F5 8F")
                {
                    byte[] txtToByte = HexToBytes(logtxt);
                    var opening = (Sensor)BitConverter.ToInt16(txtToByte, 2);

                    if (opening.HasFlag(Sensor.BILL_SERSOR) == true)
                    {
                        billopen++;
                        showtxt2str += file +"..."+ "\r\n";
                    }

                    if (opening.HasFlag(Sensor.COIN_SERSOR) == true)
                    {
                        coinopen++;
                        showtxt3str += file + "\r\n";
                    }
                }
            }
        }

        void coins_total(string txt)
        {
            string coin = "";

            coin = txt.Substring(9, 2);

            if (coin == "05" || coin == "07")
            {
                sum += 5;
                token += 1;
                testf++;
            }
            else if (coin == "06" || coin == "08")
            {
                sum += 10;
                token += 2;
                testten++;
            }
            else if (coin == "01" || coin == "02" || coin == "03" || coin == "04")
            {
                //sum += 1;
            }
        }

        void cash_total()
        {
            string money = "";

            //Console.WriteLine(Cashtxt);

            money = Cashtxt.Substring(3, 2);


            if (money == "40")
            {
                sum += 20;
                token += 5;
                test20++;
            }
            else if (money == "41")
            {
                sum += 50;
                token += 13;
                test50++;
            }
            else if (money == "42")
            {
                sum += 100;
                token += 27;
                test100++;
            }
            else if (money == "43")
            {
                sum += 200;
                token += 54;
                test200++;
            }
            else if (money == "44")
            {
                sum += 500;
                token += 135;
                test500++;
            }
            else if (money == "45")
            {
                sum += 1000;
                token += 270;
                test1000++;
            }

            Cashtxt = "";
            CashState = false;
            CashIn = false;
        }
        public enum ErrorCode : byte
        {
            NONE = 0x00,
            POWER = 0x01,
            MANUAL_ALARM = 0x04,
            MEMORY = 0x08,
            TIME = 0x10,
            HOPPER_SENSOR = 0x40,
            TOKEN_LEAK = 0x80,
        }

        public enum Sensor : short
        {
            BILL_SERSOR = 0x0200,
            COIN_SERSOR = 0x0400,
        }

        public static byte[] HexToBytes(string hexString)
        {
            List<byte> byteList = new List<byte>();
            for (int i = 0; i < hexString.Length / 2; i++)
            {
                hexString = hexString.Replace(" ", String.Empty);
                hexString = hexString.Replace("\r\n", String.Empty);
                string a = hexString.Substring(2 * i, 2);
                byteList.Add(Convert.ToByte(a, 16));
            }
            return byteList.ToArray();
        }

        void alldel()
        {
            sum = 0;
            token = 0;
            Cashtxt = "";
            CashIn = false;
            CashState = false;

            testten = 0;
            testf = 0;
            test20 = 0;
            test50 = 0;
            test100 = 0;
            test200 = 0;
            test500 = 0;
            test1000 = 0;

            error40 = 0;
            error80 = 0;

            billopen = 0;
            coinopen = 0;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Console.WriteLine("...");
            time++;
        }
    }
}
