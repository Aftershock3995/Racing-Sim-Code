using Newtonsoft.Json;
using RacingSimPedals;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO.Ports;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

public class GraphControl : Control
{
    private List<PointF> dataPoints;
    private Pen linePen;
    private Pen markerPen;
    private Pen markerPen2;
    private int selectedPointIndex;

    private bool isDragging;
    private bool pedal1Graph;
    private bool pedal2Graph;
    private bool pedal3Graph;
    public int pedal1Position { get; set; }
    public int pedal2Position { get; set; }
    public int pedal3Position { get; set; }
    public int[] PedalGraphs { get; set; }

    private const int NumGridLines = 10;
    private const float GridSpacing = 0.1f;


    private float minX = 0f;
    private float maxX = 1f;
    private float minY = 0f; 
    private float maxY = 1f;

    public GraphControl()
    {

        // no clue?
        string hexCode1 = "#e84e0f";

        // Dot Color
        string hexCode2 = "#f94c07";

        // Line Color
        string hexCode3 = "#232424";

        Color color1 = ColorTranslator.FromHtml(hexCode1);
        Color color2 = ColorTranslator.FromHtml(hexCode2);
        Color color3 = ColorTranslator.FromHtml(hexCode3);
        dataPoints = new List<PointF>();
        linePen = new Pen(color3, 2f);
        markerPen = new Pen(color2, 2.5f);
        markerPen2 = new Pen(color1, 2f);
        selectedPointIndex = -1;
        isDragging = false;
        pedal1Graph = false;
        pedal2Graph = false;
        pedal3Graph = false;

        SetStyle(ControlStyles.Selectable, true);
        SetStyle(ControlStyles.UserMouse, true);
        SetStyle(ControlStyles.StandardClick, true);
        SetStyle(ControlStyles.StandardDoubleClick, true);
        SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        DoubleBuffered = true;

        MouseDown += GraphControl_MouseDown;
        MouseMove += GraphControl_MouseMove;
        MouseUp += GraphControl_MouseUp;
        GotFocus += GraphControl_GotFocus;
        LostFocus += GraphControl_LostFocus;
    }

    public void RefreshGraph(List<PointF> newDataPoints)
    {
        dataPoints = newDataPoints;
        Invalidate();
    }

    public int Pedal1GraphActive
    {
        get { return pedal1Graph ? 1 : 0; }
        set
        {
            bool isActive = (value != 0);

            if (pedal1Graph != isActive)
            {
                pedal1Graph = isActive;
                if (!isActive && selectedPointIndex != -1)
                {
                    selectedPointIndex = -1;
                    isDragging = false;
                }
                Invalidate();
            }
        }
    }

    public int Pedal2GraphActive
    {
        get { return pedal2Graph ? 1 : 0; }
        set
        {
            bool isActive = (value != 0);

            if (pedal2Graph != isActive)
            {
                pedal2Graph = isActive;
                if (!isActive && selectedPointIndex != -1)
                {
                    selectedPointIndex = -1;
                    isDragging = false;
                }
                Invalidate();
            }
        }
    }

    public int Pedal3GraphActive
    {
        get { return pedal3Graph ? 1 : 0; }
        set
        {
            bool isActive = (value != 0);

            if (pedal3Graph != isActive)
            {
                pedal3Graph = isActive;
                if (!isActive && selectedPointIndex != -1)
                {
                    selectedPointIndex = -1;
                    isDragging = false;
                }
                Invalidate();
            }
        }
    }

    float desiredY = 200;

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        Graphics graphics = e.Graphics;
        DrawBackgroundGrid(graphics);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

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

            if (pedal1Graph && Pedal1GraphActive == 1)
            {
                    float newFloat = PedalDrawingForm.desiredX1;
                    float pedal1Position = (newFloat / scaleX) + minX;
                    float pedalX = (pedal1Position - minX) * scaleX;
                    float pedalY = Height - desiredY;
                    graphics.DrawLine(markerPen2, pedalX, pedalY, pedalX, Height);
                Invalidate();
            }

            if (pedal2Graph && Pedal2GraphActive == 1)
            {
                    float newFloat = PedalDrawingForm.desiredX2;
                    float pedal2Position = (newFloat / scaleX) + minX;
                    float pedalX = (pedal2Position - minX) * scaleX;
                    float pedalY = Height - desiredY;
                    graphics.DrawLine(markerPen2, pedalX, pedalY, pedalX, Height);
                Invalidate();
            }

            if (pedal3Graph && Pedal3GraphActive == 1)
            {
                    float newFloat = PedalDrawingForm.desiredX3;
                    float pedal3Position = (newFloat / scaleX) + minX;
                    float pedalX = (pedal3Position - minX) * scaleX;
                    float pedalY = Height - desiredY;
                    graphics.DrawLine(markerPen2, pedalX, pedalY, pedalX, Height);
                Invalidate();
            }

            // Border Graph Color
            string hexCode4 = "#f94c07";
            Color color4 = ColorTranslator.FromHtml(hexCode4);

            if (pedal1Graph && Pedal1GraphActive == 1)
            {
                DrawBorder(graphics, color4, 1f);
            }

            if (pedal2Graph && Pedal2GraphActive == 1)
            {
                DrawBorder(graphics, color4, 1f);
            }

            if (pedal3Graph && Pedal3GraphActive == 1)
            {
                DrawBorder(graphics, color4, 1f);
            }
        }
    }

    private void DrawBackgroundGrid(Graphics graphics)
    {
        float graphWidth = Width - 1;
        float graphHeight = Height - 1;

        float gridSpacingX = graphWidth * GridSpacing;
        float gridSpacingY = graphHeight * GridSpacing;

        // Grid Color
        string hexCode2 = "#5f5f5f";
        Color color2 = ColorTranslator.FromHtml(hexCode2);
        Pen gridPen = new(color2, 1f);

        // Graph Backround Color
        string hexCode = "#525252";
        Color color = ColorTranslator.FromHtml(hexCode);
        graphics.Clear(color);



        for (int i = 0; i <= NumGridLines; i++)
        {
            float x = i * gridSpacingX;
            graphics.DrawLine(gridPen, x, 0, x, graphHeight);
        }

        for (int i = 0; i <= NumGridLines; i++)
        {
            float y = i * gridSpacingY;
            graphics.DrawLine(gridPen, 0, y, graphWidth, y);
        }

        // Grid Color
        string hexCode3 = "#5f5f5f";
        Color color3 = ColorTranslator.FromHtml(hexCode3);
        Brush squareBrush = new SolidBrush(color3);

        float squareSize = 3f;

        for (int i = 0; i <= NumGridLines; i++)
        {
            for (int j = 0; j <= NumGridLines; j++)
            {
                float x = i * gridSpacingX;
                float y = j * gridSpacingY;
                graphics.FillRectangle(squareBrush, x - squareSize / 2, y - squareSize / 2, squareSize, squareSize);
            }
        }
    }

    private void DrawGraph(Graphics graphics)
    {
        int graphWidth = ClientSize.Width;
        int graphHeight = ClientSize.Height;

        PointF[] curvePoints = new PointF[dataPoints.Count];

        for (int i = 0; i < dataPoints.Count; i++)
        {
            curvePoints[i] = PointToGraphCoordinates(dataPoints[i], graphWidth, graphHeight);
        }

        if (dataPoints.Count > 1)
        {
            graphics.DrawCurve(linePen, curvePoints);
        }

        foreach (PointF graphPoint in curvePoints)
        {
            graphics.DrawEllipse(markerPen, graphPoint.X - 3, graphPoint.Y - 3, 6, 6);
        }
    }

    private PointF PointToGraphCoordinates(PointF point, float graphWidth, float graphHeight)
    {
        float x = point.X * graphWidth;
        float y = (1f - point.Y) * graphHeight;
        return new PointF(x, y);
    }


    private float CalculateDistance(PointF point1, PointF point2)
    {
        float deltaX = point2.X - point1.X;
        float deltaY = point2.Y - point1.Y;
        return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }

    private int FindClosestPointIndex(PointF point, float graphWidth, float graphHeight)
    {
        float thresholdDistance = 8f;

        for (int i = 0; i < dataPoints.Count; i++)
        {
            PointF graphPoint = PointToGraphCoordinates(dataPoints[i], graphWidth, graphHeight);
            float distance = CalculateDistance(point, graphPoint);

            if (distance < thresholdDistance)
                return i;
        }

        return -1;
    }

    private void DrawBorder(Graphics graphics, Color color, float penWidth)
    {
        using (Pen borderPen = new(color, penWidth))
        {
            graphics.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);
        }
    }

    private void GraphControl_MouseDown(object sender, MouseEventArgs e)
    {
        float graphWidth = Width - 1;
        float graphHeight = Height - 1;

        selectedPointIndex = FindClosestPointIndex(e.Location, graphWidth, graphHeight);

        if (selectedPointIndex != -1)
        {
            isDragging = true;
            Capture = true;

            if (pedal1Graph && Pedal1GraphActive == 1 && selectedPointIndex == 0)
            {
                DrawBorder(CreateGraphics(), Color.LightGray, 2.5f);
            }
            else if (pedal2Graph && Pedal2GraphActive == 1 && selectedPointIndex == 1)
            {
                DrawBorder(CreateGraphics(), Color.LightGray, 2.5f);
            }
            else if (pedal3Graph && Pedal3GraphActive == 1 && selectedPointIndex == 2)
            {
                DrawBorder(CreateGraphics(), Color.LightGray, 2.5f);
            }
        }
    }

    private void GraphControl_MouseMove(object sender, MouseEventArgs e)
    {
        if (isDragging && selectedPointIndex != -1)
        {
            float graphWidth = Width - 1;
            float graphHeight = Height - 1;

            PointF normalizedPoint = GraphToNormalizedCoordinates(e.Location, graphWidth, graphHeight);

            if (normalizedPoint.X >= minX + 0.02f && normalizedPoint.X <= maxX - 0.02f &&
                normalizedPoint.Y >= minY + 0.03f && normalizedPoint.Y <= maxY - 0.03f)
            {
                dataPoints[selectedPointIndex] = normalizedPoint;
                Invalidate();
            }
        }
    }

    private void GraphControl_MouseUp(object sender, MouseEventArgs e)
    {
        isDragging = false;
        Capture = false;
    }

    private void GraphControl_GotFocus(object sender, EventArgs e)
    {
        pedal1Graph = true;
        pedal2Graph = true;
        pedal3Graph = true;
        Invalidate();
    }

    private void GraphControl_LostFocus(object sender, EventArgs e)
    {
        pedal1Graph = false;
        pedal2Graph = false;
        pedal3Graph = false;
        Invalidate();
    }

    private PointF GraphToNormalizedCoordinates(PointF point, float graphWidth, float graphHeight)
    {
        float x = point.X / graphWidth;
        float y = 1f - (point.Y / graphHeight);
        return new PointF(x, y);
    }

    public void SetGridBounds(float minX, float maxX, float minY, float maxY)
    {
        this.minX = minX;
        this.maxX = maxX;
        this.minY = minY;
        this.maxY = maxY;
    }

    public void AddDataPoint(PointF point)
    {
        float graphWidth = Width - 1;
        float graphHeight = Height - 1;

        PointF normalizedPoint = GraphToNormalizedCoordinates(point, graphWidth, graphHeight);

        if (normalizedPoint.X >= minX && normalizedPoint.X <= maxX &&
            normalizedPoint.Y >= minY && normalizedPoint.Y <= maxY)
        {
            dataPoints.Add(normalizedPoint);
            Invalidate();
        }
    }
}