using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RacingSimPedals
{
    public class PedalDrawingForm : Form
    {
        private float minX = 0f;
        private float maxX = 1f;
        private float minY = 0f;
        private float maxY = 1f;

        private Pen markerPen;

        private Color lineColor = ColorTranslator.FromHtml("#f94c07");

        public PedalDrawingForm()
        {
            markerPen = new Pen(lineColor, 2.5f);

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

            float scaleX = Width / (maxX - minX);
            float scaleY = Height / (maxY - minY);
            float xPosition = 50f;
            float pedalX = (xPosition - minX) * scaleX;
            float pedalY = Height;
            graphics.DrawLine(markerPen, pedalX, pedalY, pedalX, Height - 50);
        }
    }
}
