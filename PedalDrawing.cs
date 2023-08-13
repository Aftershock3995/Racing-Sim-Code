using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RacingSimPedals
{
    public class PedalDrawingForm : Form
    {
        public int pedal1Position { get; set; }
        public int pedal2Position { get; set; }
        public int pedal3Position { get; set; }
        public int[] PedalGraphs { get; set; }
        private List<PointF> dataPoints;

        private float minX = 0f;
        private float maxX = 1f;
        private float minY = 0f;
        private float maxY = 1f;

        private static PedalDrawingForm pedalDrawingForm;

        private Form mainForm;
        public Form MainForm
        {
            get => mainForm;
            set => mainForm = value;
        }

        public PedalDrawingForm(List<PointF> newDataPoints)
        {
            dataPoints = newDataPoints;
        }

        public void RefreshGraph(List<PointF> newDataPoints)
        {
            dataPoints = newDataPoints;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (Pen markerPen = new Pen(Color.Red))
            using (Pen linePen = new Pen(Color.Blue))
            {
                float scaleX = Width / (maxX - minX);
                float scaleY = Height / (maxY - minY);

                if (dataPoints.Count > 0)
                {
                    PointF[] curvePoints = new PointF[dataPoints.Count];

                    for (int i = 0; i < dataPoints.Count; i++)
                    {
                        PointF point = dataPoints[i];
                        float x = (point.X - minX) * scaleX;
                        float y = Height - (point.Y - minY) * scaleY;
                        curvePoints[i] = new PointF(x, y);
                    }

                    graphics.DrawCurve(linePen, curvePoints);

                    foreach (PointF point in curvePoints)
                    {
                        graphics.DrawEllipse(markerPen, point.X - 4, point.Y - 4, 8, 8);
                    }

                    float pedalX = (pedal1Position - minX) * scaleX;
                    float pedalY = Height - (pedal1Position - minY) * scaleY;
                    graphics.DrawLine(markerPen, pedalX, pedalY, pedalX, Height);
                }
            }
        }
    }
}
