using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using CoreAudioApi;

namespace WindowsFormsApp1
{
    public partial class Volume : Form
    {
        MMDevice device;
        List<Button> btnlist;
        public Volume()
        {
            InitializeComponent();
            
            btnlist = new List<Button> { Volume40, Volume60, Volume80, Volume100 };
            MMDeviceEnumerator devEnum = new MMDeviceEnumerator();
            device = devEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            
        }


        private void CloseFormbtn_Click(object sender, EventArgs e)
        {
            SOUND.Stop();
            btnstyle(null);
            Form1.volume.Close();
        }

        private void Volume40_Click(object sender, EventArgs e)
        {
            SOUND.Stop();
            
            SetVolume = 40;
            btnstyle(Volume40);
            SOUND.PowerOn();
        }

        private void Volume60_Click(object sender, EventArgs e)
        {
            SOUND.Stop();
            SetVolume = 60;
            btnstyle(Volume60);
            SOUND.PowerOn();
        }

        private void Volume80_Click(object sender, EventArgs e)
        {
            SOUND.Stop();
            SetVolume = 80;
            btnstyle(Volume80);
            SOUND.PowerOn();
        }

        private void Volume100_Click(object sender, EventArgs e)
        {
            SOUND.Stop();
            SetVolume =100;
            btnstyle(Volume100);
            SOUND.PowerOn();
        }

        #region 修改系統音量


        void btnstyle(Button button)
        {
            foreach(Button btn in btnlist)
            {
                if(btn ==button)
                {
                    btn.UseVisualStyleBackColor = false;
                    btn.BackColor = Color.FromArgb(14, 110, 184);
                    btn.ForeColor = Color.White;
                }
                else
                {
                    btn.BackColor = Color.FromKnownColor(KnownColor.ControlLight);
                    btn.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
                }
            }
        }

        #endregion
       /// <summary>
        /// 獲取當前音量
        /// </summary>
        public int CurrentVolume
        {
            get => Convert.ToInt32(device.AudioEndpointVolume.MasterVolumeLevelScalar * 100.0f);
        }

        public int SetVolume
        {
            get => CurrentVolume;
            set
            {
                if (value < 0) device.AudioEndpointVolume.MasterVolumeLevelScalar = 0.0f;
                else if (value > 100) device.AudioEndpointVolume.MasterVolumeLevelScalar = 100.0f;
                else device.AudioEndpointVolume.MasterVolumeLevelScalar = value / 100.0f;
            }
        }

        private void Volume_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Console.WriteLine("現在音量" + CurrentVolume);
        }
    }
}
