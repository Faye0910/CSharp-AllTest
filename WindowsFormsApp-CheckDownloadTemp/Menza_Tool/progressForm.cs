using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Menza_Tool
{
    public partial class progressForm : Form
    {
        public progressForm()
        {
            InitializeComponent();
        }

        public void set(int min, int max)
        {
            progressBar1.Minimum = min;
            progressBar1.Maximum = max;
        }

        public void Addprogess()
        {
            progressBar1.Value++;
            int percent = (int)(((double)progressBar1.Value / (double)progressBar1.Maximum) * 100);
            progressBar1.Refresh();
            progressBar1.CreateGraphics().DrawString(percent.ToString() + "%", new Font("Arial", (float)8.5, FontStyle.Regular), Brushes.Black, new PointF(progressBar1.Width / 2 - 10, progressBar1.Height / 2 - 7));
            Thread.Sleep(10);
        }
    }
}
