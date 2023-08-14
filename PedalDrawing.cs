using System;
using System.Drawing;
using System.Windows.Forms;

namespace RacingSimPedals
{
    public class PedalDrawingForm : Form
    {
        public int pedal1Position { get; set; }

        public PedalDrawingForm()
        {
            // Start the timer to update the pedal position
            Timer timer = new Timer();
            timer.Interval = 100; // Set your preferred interval
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Refresh the form to update the drawing
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (Pen linePen = new Pen(Color.Blue))
            {
                // Calculate the position of the pedal on the x-axis
                float pedalX = (float)pedal1Position * Width / 100;

                // Draw the vertical line
                graphics.DrawLine(linePen, pedalX, 0, pedalX, Height);
            }
        }
    }
}
