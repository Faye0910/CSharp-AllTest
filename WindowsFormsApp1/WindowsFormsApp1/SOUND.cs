using System;
using System.Media;
using System.Runtime.InteropServices;

namespace WindowsFormsApp1
{
    /// <summary>
    /// 音效類別
    /// </summary>
    internal class SOUND
    {
        private static SoundPlayer sound = new SoundPlayer();

        /// <summary>
        /// 分貝
        /// </summary>
        public static string db = "0";

        //音檔名稱
        private static string power_on = "Power On";
        private static string token_forward = "Token Forward";
        private static string token_clear = "Token Clear";
        private static string token_invert = "Token Invert";
        private static string tp70 = "TP70";
        private static string coin = "Coin";
        private static string token_in = "Token In";
        private static string m1_in = "M1 In";
        private static string qr_in = "Qr In";
        private static string m1_bad = "M1 BAD";
        private static string nde_in = "Nde In";
        private static string ndr_in = "Ndr In";

        
        /// <summary>
        /// 播放指定頻率
        /// </summary>
        /// <param name="dwFreq">頻率</param>
        /// <param name="dwDuration">時間(ms)</param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern int Beep(int dwFreq, int dwDuration);

        /// <summary>
        /// i/o板啟動
        /// </summary>
        public static void PowerOn()
        {
            Play(power_on);
        }
        /// <summary>
        /// 吐代幣
        /// </summary>
        public static void TokenForward()
        {
            Play(token_forward, true);
        }
        /// <summary>
        /// 清Hopper
        /// </summary>
        public static void TokenClear()
        {
            Play(token_clear);
        }
        /// <summary>
        /// Hopper馬達反轉--
        /// </summary>
        public static void TokenInvert()
        {
            Play(token_invert);
        }
        /// <summary>
        /// 吃紙鈔
        /// </summary>
        public static void TP70()
        {
            Play(tp70);
        }
        /// <summary>
        /// 投錢幣
        /// </summary>
        public static void Coin()
        {
            Play(coin);
        }
        /// <summary>
        /// 投代幣
        /// </summary>
        public static void TokenIn()
        {
            Play(token_in);
        }
        /// <summary>
        /// 卡片掃描
        /// </summary>
        public static void M1In()
        {
            Play(m1_in);
        }
        /// <summary>
        /// QrCord掃描--
        /// </summary>
        public static void QrIn()
        {
            Play(qr_in);
        }
        /// <summary>
        /// 卡片掃描失敗
        /// </summary>
        public static void M1Bad()
        {
            Play(m1_bad);
        }
        /// <summary>
        /// 
        /// </summary>
        public static void NdeIn()
        {
            Play(nde_in);
        }
        /// <summary>
        /// 尚未實裝
        /// </summary>
        public static void NdrIn()
        {
            Play(ndr_in);
        }

        /// <summary>
        /// 停止播放
        /// </summary>
        public static void Stop()
        {
            sound.Stop();
        }

        /// <summary>
        /// 播放wav
        /// </summary>
        /// <param name="name">檔案名</param>
        /// <param name="loop">是否循環</param>
        private static void Play(string name, bool loop = false)
        {
            //指定路徑
            string location = String.Format("C:\\Users\\Faye.Lin\\source\\repos\\for test\\WindowsFormsApp1\\WindowsFormsApp1\\Power On.wav", name, db);
           
            try
            {
                sound.SoundLocation = location;
                sound.LoadAsync();

                if (!loop)
                {
                    sound.Play();
                }
                else
                {
                    sound.PlayLooping();
                }
            }
            catch 
            { 

            }
        }
    }

}
