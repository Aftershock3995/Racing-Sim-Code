using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RacingSimPedals
{
    public class PedalDrawingForm : Form
    {
        public int pedal1Position { get; set; }
        private Color lineColor = ColorTranslator.FromHtml("#f94c07"); 

        public PedalDrawingForm()
        {
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 250; 
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (Pen linePen = new Pen(lineColor))
            {
                float pedalX = (float)pedal1Position * Width / 100;

                graphics.DrawLine(linePen, pedalX, 0, pedalX, Height);
            }
        }
    }
}
