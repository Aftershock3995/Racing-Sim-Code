using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using MathNet.Numerics;
using MathNet.Numerics.Interpolation;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

namespace RacingSimPedals
{
    public static class Program
    {
        private static GraphControl graphControl;
        private static ComboBox? comPortComboBox;
        private static Button? startButton;

        private static Button? saveButton;
        private static ComboBox? saveComboBox;

        private static TextBox? renameTextBox;
        private static Button? renameButton;

        private static Button? addPointButton;
        private static Button? removePointButton;

        private static GraphControl pedal1Graph;
        private static GraphControl pedal2Graph;
        private static GraphControl pedal3Graph;

        private static string saveFolderPath = "SaveFiles";
        private static List<string> savedConfigurations;

        private static List<PointF>[] graph;

        private static SerialPort? serialPort;
        private static TextBox? dataTextBox;

        private static int pedal1Position;
        private static int pedal2Position;
        private static int pedal3Position;

        private static List<PointF>? pedal1ResponseCurve;
        private static List<PointF>? pedal2ResponseCurve;
        private static List<PointF>? pedal3ResponseCurve;

        private static string savesFilePath = "saves.json";

        private static Form mainWindow;
        private static ColorGradient colorGradient;

        [STAThread]
        public static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Full App Color
            string hexCode1 = "#373737";
            Color color1 = ColorTranslator.FromHtml(hexCode1);

            Color startColor = ColorTranslator.FromHtml(hexCode1);
            Color endColor = Color.Black;
            colorGradient = new ColorGradient(startColor, endColor);

            System.Windows.Forms.Timer colorTimer = new System.Windows.Forms.Timer();
            colorTimer.Interval = 150;
            colorTimer.Tick += ColorTimer_Tick;
            colorTimer.Start();

            // Setting text color
            string hexCode2 = "#949494";
            Color color2 = ColorTranslator.FromHtml(hexCode2);

            // Border color for buttons
            string hexCode3 = "#f94c07";
            Color color3 = ColorTranslator.FromHtml(hexCode3);

            // Dropdown/text box
            string hexCode4 = "#404040";
            Color color4 = ColorTranslator.FromHtml(hexCode4);

            mainWindow = new Form();
            mainWindow.Size = new Size(655, 325);
            mainWindow.BackColor = (color1);

            comPortComboBox = new ComboBox();
            comPortComboBox.Location = new Point(10, 10);
            comPortComboBox.Width = 200;
            comPortComboBox.Text = "Choose a COM port";
            comPortComboBox.ForeColor = (color2);
            comPortComboBox.BackColor = (color1);

            startButton = new Button();
            startButton.Location = new Point(220, 10);
            startButton.Text = "Start";
            startButton.ForeColor = (color2);
            startButton.FlatStyle = FlatStyle.Flat;
            startButton.FlatAppearance.BorderColor = (color3);
            startButton.FlatAppearance.BorderSize = 1;

            comPortComboBox.Items.AddRange(SerialPort.GetPortNames());
            startButton.Click += StartButton_Click;

            pedal1Graph = new GraphControl();
            pedal1Graph.Location = new Point(10, 125);
            pedal1Graph.Size = new Size(200, 100);

            pedal2Graph = new GraphControl();
            pedal2Graph.Location = new Point(220, 125);
            pedal2Graph.Size = new Size(200, 100);

            pedal3Graph = new GraphControl();
            pedal3Graph.Location = new Point(430, 125);
            pedal3Graph.Size = new Size(200, 100);

            Button editButton = new Button();
            editButton.Location = new Point(10, 250);
            editButton.Text = "Edit Graph";
            editButton.Click += EditButton_Click;
            editButton.ForeColor = (color2);
            editButton.FlatStyle = FlatStyle.Flat;
            editButton.FlatAppearance.BorderColor = (color3);
            editButton.FlatAppearance.BorderSize = 1;

            Button resetButton = new Button();
            resetButton.Location = new Point(100, 250);
            resetButton.Text = "Reset Graph";
            resetButton.Click += ResetButton_Click;
            resetButton.ForeColor = (color2);
            resetButton.FlatStyle = FlatStyle.Flat;
            resetButton.FlatAppearance.BorderColor = (color3);
            resetButton.FlatAppearance.BorderSize = 1;

            Button addPointButton = new Button();
            addPointButton.Name = "addPointButton";
            addPointButton.Location = new Point(190, 250);
            addPointButton.Text = "Add Point";
            addPointButton.Click += AddPointButton_Click;
            addPointButton.ForeColor = (color2);
            addPointButton.FlatStyle = FlatStyle.Flat;
            addPointButton.FlatAppearance.BorderColor = (color3);
            addPointButton.FlatAppearance.BorderSize = 1;

            Button removePointButton = new Button();
            removePointButton.Name = "removePointButton";
            removePointButton.Location = new Point(280, 250);
            removePointButton.Text = "Remove Last Point";
            removePointButton.Click += RemovePointButton_Click;
            removePointButton.ForeColor = (color2);
            removePointButton.FlatStyle = FlatStyle.Flat;
            removePointButton.FlatAppearance.BorderColor = (color3);
            removePointButton.FlatAppearance.BorderSize = 1;

            saveButton = new Button();
            saveButton.Location = new Point(300, 10);
            saveButton.Text = "Save";
            saveButton.Click += SaveButton_Click;
            saveButton.ForeColor = (color2);
            saveButton.FlatStyle = FlatStyle.Flat;
            saveButton.FlatAppearance.BorderColor = (color3);
            saveButton.FlatAppearance.BorderSize = 1;

            saveComboBox = new ComboBox();
            saveComboBox.Location = new Point(400, 10);
            saveComboBox.Width = 150;
            saveComboBox.SelectedIndexChanged += SaveComboBox_SelectedIndexChanged;
            saveComboBox.Items.Add("Choose a Save");
            saveComboBox.SelectedIndex = 0;
            saveComboBox.DropDown += (sender, e) => saveComboBox.Items.Remove("Choose a Save");
            saveComboBox.ForeColor = (color2);
            saveComboBox.BackColor = (color1);

            renameTextBox = new TextBox();
            renameTextBox.Location = new Point(400, 40);
            renameTextBox.Width = 150;
            renameTextBox.Text = "Rename Save";
            renameTextBox.Click += RenameTextBox_Click;
            renameTextBox.ForeColor = (color2);
            renameTextBox.BackColor = (color1);

            renameButton = new Button();
            renameButton.Location = new Point(560, 40);
            renameButton.Text = "Rename";
            renameButton.Click += RenameButton_Click;
            renameButton.ForeColor = (color2);
            renameButton.FlatStyle = FlatStyle.Flat;
            renameButton.FlatAppearance.BorderColor = (color3);
            renameButton.FlatAppearance.BorderSize = 1;

            Button deleteButton = new Button();
            deleteButton.Location = new Point(560, 10);
            deleteButton.Text = "Delete";
            deleteButton.Click += DeleteButton_Click;
            deleteButton.ForeColor = (color2);
            deleteButton.FlatStyle = FlatStyle.Flat;
            deleteButton.FlatAppearance.BorderColor = (color3);
            deleteButton.FlatAppearance.BorderSize = 1;

            dataTextBox = new TextBox();
            dataTextBox.Location = new Point(10, 40);
            dataTextBox.Width = 200;
            dataTextBox.Height = 70;
            dataTextBox.Multiline = true;
            dataTextBox.ScrollBars = ScrollBars.Vertical;
            dataTextBox.ReadOnly = true;
            dataTextBox.BackColor = (color1);
            dataTextBox.ForeColor = (color2);

            mainWindow.Controls.Add(dataTextBox);
            mainWindow.Controls.Add(deleteButton);
            mainWindow.Controls.Add(renameTextBox);
            mainWindow.Controls.Add(renameButton);
            mainWindow.Controls.Add(saveButton);
            mainWindow.Controls.Add(saveComboBox);
            mainWindow.Controls.Add(comPortComboBox);
            mainWindow.Controls.Add(startButton);
            mainWindow.Controls.Add(pedal1Graph);
            mainWindow.Controls.Add(pedal2Graph);
            mainWindow.Controls.Add(pedal3Graph);
            mainWindow.Controls.Add(editButton);
            mainWindow.Controls.Add(resetButton);
            mainWindow.Controls.Add(addPointButton);
            mainWindow.Controls.Add(removePointButton);

            pedal1ResponseCurve = new List<PointF>()
    {
        new PointF(0f, 0f),
        new PointF(0.125f, 0.125f),
        new PointF(0.25f, 0.25f),
        new PointF(0.375f, 0.375f),
        new PointF(0.5f, 0.5f)
    };

            pedal2ResponseCurve = new List<PointF>()
    {
        new PointF(0f, 0f),
        new PointF(0.125f, 0.125f),
        new PointF(0.25f, 0.25f),
        new PointF(0.375f, 0.375f),
        new PointF(0.5f, 0.5f)
    };

            pedal3ResponseCurve = new List<PointF>()
    {
        new PointF(0f, 0f),
        new PointF(0.125f, 0.125f),
        new PointF(0.25f, 0.25f),
        new PointF(0.375f, 0.375f),
        new PointF(0.5f, 0.5f)
    };


            if (!Directory.Exists(saveFolderPath))
            {
                Directory.CreateDirectory(saveFolderPath);
            }


            LoadSavedConfigurations();

            Application.Run(mainWindow);
        }

        private static void ColorTimer_Tick(object sender, EventArgs e)
        {
            Color currentColor = colorGradient.GetColor();

            mainWindow.BackColor = currentColor;

            if (colorGradient.IsTransitionComplete())
            {
                colorGradient.Reverse();

                Color tempColor = colorGradient.StartColor;
                colorGradient.StartColor = colorGradient.EndColor;
                colorGradient.EndColor = tempColor;
            }
        }

        public class ColorGradient
        {
            private Color initialStartColor; 
            private Color initialEndColor;  
            private Color startColor;
            private Color endColor;
            private int steps;
            private int currentStep;
            private bool reverse;

            public bool IsTransitionComplete()
            {
                if (reverse)
                {
                    return currentStep == 0;
                }
                else
                {
                    return currentStep == steps - 1;
                }
            }

            public Color StartColor
            {
                get { return startColor; }
                set { startColor = value; }
            }

            public Color EndColor
            {
                get { return endColor; }
                set { endColor = value; }
            }

            public ColorGradient(Color startColor, Color endColor, int steps = 100)
            {
                this.initialStartColor = startColor;
                this.initialEndColor = endColor; 
                this.startColor = startColor;
                this.endColor = endColor;
                this.steps = steps;
                this.currentStep = 0;
                this.reverse = false;
            }

            public void Reverse()
            {
                reverse = !reverse;
                currentStep = steps - currentStep - 1;
            }

            private void Swap<T>(ref T a, ref T b)
            {
                T temp = a;
                a = b;
                b = temp;
            }

            public Color GetColor()
            {
                float ratio = (float)currentStep / (steps - 1);

                if (reverse)
                    ratio = 1 - ratio;

                int red = (int)(initialStartColor.R * (1 - ratio) + initialEndColor.R * ratio);
                int green = (int)(initialStartColor.G * (1 - ratio) + initialEndColor.G * ratio);
                int blue = (int)(initialStartColor.B * (1 - ratio) + initialEndColor.B * ratio);

                currentStep = (currentStep + 1) % steps;

                if (currentStep == 0)
                    Swap(ref initialStartColor, ref initialEndColor);

                return Color.FromArgb(red, green, blue);
            }
        }

        private static void RenameTextBox_Click(object sender, EventArgs e)
        {
            if (renameTextBox.Text == "Rename Save")
            {
                renameTextBox.Text = string.Empty;
            }
        }

        private static void StartButton_Click(object sender, EventArgs e)
        {
            string selectedPort = comPortComboBox.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectedPort))
            {
                try
                {
                    serialPort = new SerialPort(selectedPort, 9600);
                    serialPort.DataReceived += SerialPort_DataReceived;
                    serialPort.Open();

                    comPortComboBox.Enabled = false;
                    startButton.Enabled = false;

                    Console.WriteLine("Serial port opened. Press any key to exit.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        public static void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            GraphControl graphControl = new GraphControl();

            SerialPort sp = (SerialPort)sender;
            string data = sp.ReadLine();

            if (!string.IsNullOrEmpty(data))
            {
                // Use the Invoke method to update the dataTextBox from the main thread
                dataTextBox.Invoke(new Action(() =>
                {
                    dataTextBox.AppendText("Received: " + data + Environment.NewLine);
                }));

                if (data.Contains("Pedal 1 Position:"))
                {
                    int position = int.Parse(data.Split(':')[1].Trim());
                    Console.WriteLine("Pedal 1 Position: " + position);

                    pedal1Position = position;

                    // Send the parsed position back over the serial port
                    string response = "Pedal 1 Position: " + position;
                    sp.WriteLine(response);

                    // Display the sent data in the dataTextBox
                    dataTextBox.Invoke(new Action(() =>
                    {
                        dataTextBox.AppendText("Sent: " + response + Environment.NewLine);
                    }));
                }
                else if (data.Contains("Pedal 2 Position:"))
                {
                    int position = int.Parse(data.Split(':')[1].Trim());
                    Console.WriteLine("Pedal 2 Position: " + position);
                    pedal2Position = position;

                    // Send the parsed position back over the serial port
                    string response = "Pedal 2 Position: " + position;
                    sp.WriteLine(response);

                    // Display the sent data in the dataTextBox
                    dataTextBox.Invoke(new Action(() =>
                    {
                        dataTextBox.AppendText("Sent: " + response + Environment.NewLine);
                    }));
                }
                else if (data.Contains("Pedal 3 Position:"))
                {
                    int position = int.Parse(data.Split(':')[1].Trim());
                    Console.WriteLine("Pedal 3 Position: " + position);
                    pedal3Position = position;

                    // Send the parsed position back over the serial port
                    string response = "Pedal 3 Position: " + position;
                    sp.WriteLine(response);

                    // Display the sent data in the dataTextBox
                    dataTextBox.Invoke(new Action(() =>
                    {
                        dataTextBox.AppendText("Sent: " + response + Environment.NewLine);
                    }));
                }
            }
        }

        private static void EditButton_Click(object sender, EventArgs e)
        {
            // Define the response curves for pedal 1, pedal 2, and pedal 3
            List<PointF> pedal1ResponseCurve = new List<PointF>()
    {
        new PointF(0f, 0f),
        new PointF(0.125f, 0.125f),
        new PointF(0.25f, 0.25f),
        new PointF(0.375f, 0.375f),
        new PointF(0.5f, 0.5f)
    };

            List<PointF> pedal2ResponseCurve = new List<PointF>()
    {
        new PointF(0f, 0f),
        new PointF(0.125f, 0.125f),
        new PointF(0.25f, 0.25f),
        new PointF(0.375f, 0.375f),
        new PointF(0.5f, 0.5f)
    };

            List<PointF> pedal3ResponseCurve = new List<PointF>()
    {
        new PointF(0f, 0f),
        new PointF(0.125f, 0.125f),
        new PointF(0.25f, 0.25f),
        new PointF(0.375f, 0.375f),
        new PointF(0.5f, 0.5f)
    };

            // Refresh the graphs with the new response curves
            pedal1Graph.RefreshGraph(pedal1ResponseCurve);
            pedal2Graph.RefreshGraph(pedal2ResponseCurve);
            pedal3Graph.RefreshGraph(pedal3ResponseCurve);

            // Send the response curves over the serial port
            string pedal1Response = ConvertResponseCurveToString(pedal1ResponseCurve);
            string pedal2Response = ConvertResponseCurveToString(pedal2ResponseCurve);
            string pedal3Response = ConvertResponseCurveToString(pedal3ResponseCurve);

            // Append the sent data to the dataTextBox
            dataTextBox.Invoke(new Action(() =>
            {
                dataTextBox.AppendText("Sent P1 Response: " + pedal1Response + Environment.NewLine);
                dataTextBox.AppendText("Sent P2 Response: " + pedal2Response + Environment.NewLine);
                dataTextBox.AppendText("Sent P3 Response: " + pedal3Response + Environment.NewLine);
            }));

            // Update the pedal positions in the graph control
            float pedal1Position = 0.25f;
            float pedal2Position = 0.35f;
            float pedal3Position = 0.4f;

            // Append the pedal positions to the dataTextBox
            dataTextBox.Invoke(new Action(() =>
            {
                dataTextBox.AppendText("Pedal 1 Position: " + pedal1Position + Environment.NewLine);
                dataTextBox.AppendText("Pedal 2 Position: " + pedal2Position + Environment.NewLine);
                dataTextBox.AppendText("Pedal 3 Position: " + pedal3Position + Environment.NewLine);
            }));
        }

        private static string ConvertResponseCurveToString(List<PointF> curve)
        {
            // Generate the smooth curve using the provided points
            List<PointF> smoothCurve = GenerateSmoothCurve(curve);

            // Convert the smooth curve to string representation
            StringBuilder sb = new StringBuilder();
            foreach (PointF point in smoothCurve)
            {
                sb.AppendFormat("({0:F3}, {1:F3}) ", point.X, point.Y);
            }

            return sb.ToString();
        }

        private static List<PointF> GenerateSmoothCurve(List<PointF> curve)
        {
            // Create cubic spline interpolation object
            double[] x = curve.Select(p => (double)p.X).ToArray();
            double[] y = curve.Select(p => (double)p.Y).ToArray();
            CubicSpline spline = CubicSpline.InterpolateNatural(x, y);

            // Generate the smooth curve using cubic spline interpolation
            List<PointF> smoothCurve = new List<PointF>();
            float minX = curve.Min(p => p.X);
            float maxX = curve.Max(p => p.X);
            float interval = (maxX - minX) / 100; // Adjust the interval for desired curve smoothness

            for (float xValue = minX; xValue <= maxX; xValue += interval)
            {
                float yValue = (float)spline.Interpolate(xValue);
                smoothCurve.Add(new PointF(xValue, yValue));
            }

            return smoothCurve;
        }

        private static void ResetButton_Click(object sender, EventArgs e)
        {

            pedal1ResponseCurve = new List<PointF>()
            {
                new PointF(0f, 0f),
                new PointF(0.125f, 0.125f),
                new PointF(0.25f, 0.25f),
                new PointF(0.375f, 0.375f),
                new PointF(0.5f, 0.5f)
            };

            pedal2ResponseCurve = new List<PointF>()
            {
                new PointF(0f, 0f),
                new PointF(0.125f, 0.125f),
                new PointF(0.25f, 0.25f),
                new PointF(0.375f, 0.375f),
                new PointF(0.5f, 0.5f)
            };

            pedal3ResponseCurve = new List<PointF>()
            {
                new PointF(0f, 0f),
                new PointF(0.125f, 0.125f),
                new PointF(0.25f, 0.25f),
                new PointF(0.375f, 0.375f),
                new PointF(0.5f, 0.5f)
            };


            pedal1Graph.RefreshGraph(pedal1ResponseCurve);
            pedal2Graph.RefreshGraph(pedal2ResponseCurve);
            pedal3Graph.RefreshGraph(pedal3ResponseCurve);
        }

        private static void AddPointButton_Click(object sender, EventArgs e)
        {
            if (pedal1Graph.Pedal1GraphActive == 1 && pedal2ResponseCurve != null)
            {
                float newX1 = 0.75f;
                float newY1 = 0.75f;
                pedal2ResponseCurve.Add(new PointF(newX1, newY1));
                pedal2Graph.RefreshGraph(pedal2ResponseCurve);
            }
            else if (pedal2Graph.Pedal2GraphActive == 1 && pedal1ResponseCurve != null)
            {
                float newX2 = 0.75f;
                float newY2 = 0.75f;
                pedal1ResponseCurve.Add(new PointF(newX2, newY2));
                pedal1Graph.RefreshGraph(pedal1ResponseCurve);
            }
            else if (pedal3Graph.Pedal3GraphActive == 1 && pedal3ResponseCurve != null)
            {
                float newX3 = 0.75f;
                float newY3 = 0.75f;
                pedal3ResponseCurve.Add(new PointF(newX3, newY3));
                pedal3Graph.RefreshGraph(pedal3ResponseCurve);
            }

            // Check the current active graph and prevent it from being deactivated
            if (pedal1Graph.Pedal1GraphActive == 1)
                pedal1Graph.Pedal1GraphActive = 1;
            else if (pedal2Graph.Pedal2GraphActive == 1)
                pedal2Graph.Pedal2GraphActive = 1;
            else if (pedal3Graph.Pedal3GraphActive == 1)
                pedal3Graph.Pedal3GraphActive = 1;
        }

        private static void RemovePointButton_Click(object sender, EventArgs e)
        {

            if (pedal1Graph.Pedal1GraphActive == 1 && pedal1ResponseCurve != null)
            {

                if (pedal1ResponseCurve.Count > 1)
                {
                    pedal2ResponseCurve.RemoveAt(pedal2ResponseCurve.Count - 1);
                    pedal2Graph.RefreshGraph(pedal2ResponseCurve);
                }
            }
            else if (pedal2Graph.Pedal2GraphActive == 1 && pedal2ResponseCurve != null)
            {

                if (pedal2ResponseCurve.Count > 1)
                {
                    pedal1ResponseCurve.RemoveAt(pedal1ResponseCurve.Count - 1);
                    pedal1Graph.RefreshGraph(pedal1ResponseCurve);
                }
            }
            else if (pedal3Graph.Pedal3GraphActive == 1 && pedal3ResponseCurve != null)
            {

                if (pedal3ResponseCurve.Count > 1)
                {
                    pedal3ResponseCurve.RemoveAt(pedal3ResponseCurve.Count - 1);
                    pedal3Graph.RefreshGraph(pedal3ResponseCurve);
                }
            }


            pedal1Graph.Pedal1GraphActive = 1;
            pedal2Graph.Pedal2GraphActive = 1;
            pedal3Graph.Pedal3GraphActive = 1;
        }

        private static void SaveButton_Click(object sender, EventArgs e)
        {

            string saveName = "Save " + (saveComboBox.Items.Count + 1);
            SaveConfiguration(saveName);
        }

        private static void SaveConfiguration(string saveName)
        {


            string json = GetResponseCurvesAsJson();
            string saveFilePath = Path.Combine(saveFolderPath, saveName + ".json");
            File.WriteAllText(saveFilePath, json);


            saveComboBox.Items.Add(saveName);
            saveComboBox.SelectedIndex = saveComboBox.Items.Count - 1;


            savedConfigurations.Add(saveName);
        }

        private static string GetResponseCurvesAsJson()
        {

            var responseCurves = new Dictionary<string, List<PointF>>();
            responseCurves["Pedal1"] = pedal1ResponseCurve;
            responseCurves["Pedal2"] = pedal2ResponseCurve;
            responseCurves["Pedal3"] = pedal3ResponseCurve;


            string json = JsonConvert.SerializeObject(responseCurves);

            return json;
        }

        private static void SaveComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            string selectedSaveName = saveComboBox.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectedSaveName))
            {
                LoadConfiguration(selectedSaveName);
            }
        }

        private static void LoadConfiguration(string saveName)
        {
            string saveFilePath = Path.Combine(saveFolderPath, saveName + ".json");
            if (File.Exists(saveFilePath))
            {
                string json = File.ReadAllText(saveFilePath);
                var responseCurves = JsonConvert.DeserializeObject<Dictionary<string, List<PointF>>>(json);
                if (responseCurves != null)
                {
                    if (responseCurves.TryGetValue("Pedal1", out var pedal1Curve))
                    {
                        pedal1ResponseCurve = pedal1Curve;
                        pedal1Graph.RefreshGraph(pedal1ResponseCurve);
                    }
                    if (responseCurves.TryGetValue("Pedal2", out var pedal2Curve))
                    {
                        pedal2ResponseCurve = pedal2Curve;
                        pedal2Graph.RefreshGraph(pedal2ResponseCurve);
                    }
                    if (responseCurves.TryGetValue("Pedal3", out var pedal3Curve))
                    {
                        pedal3ResponseCurve = pedal3Curve;
                        pedal3Graph.RefreshGraph(pedal3ResponseCurve);
                    }
                }
            }
        }

        private static void RenameButton_Click(object sender, EventArgs e)
        {
            string selectedSaveName = saveComboBox.SelectedItem as string;
            string newSaveName = renameTextBox.Text.Trim();

            if (!string.IsNullOrEmpty(selectedSaveName) && !string.IsNullOrEmpty(newSaveName))
            {
                if (selectedSaveName != newSaveName)
                {

                    string oldFilePath = Path.Combine(saveFolderPath, selectedSaveName + ".json");
                    string newFilePath = Path.Combine(saveFolderPath, newSaveName + ".json");
                    File.Move(oldFilePath, newFilePath);


                    int selectedIndex = saveComboBox.SelectedIndex;
                    saveComboBox.Items[selectedIndex] = newSaveName;
                }
            }
        }

        private static void DeletePictureBox_Click(object sender, EventArgs e)
        {
            string selectedSaveName = saveComboBox.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectedSaveName))
            {

                string filePath = Path.Combine(saveFolderPath, selectedSaveName + ".json");
                File.Delete(filePath);


                saveComboBox.Items.Remove(selectedSaveName);
                saveComboBox.SelectedIndex = -1;


                renameTextBox.Text = string.Empty;
            }
        }

        private static void DeleteButton_Click(object sender, EventArgs e)
        {
            if (saveComboBox.SelectedItem != null)
            {
                string selectedSave = saveComboBox.SelectedItem.ToString();


                string saveFilePath = Path.Combine(saveFolderPath, selectedSave + ".json");
                File.Delete(saveFilePath);

                // Remove the save name from the drop-down menu
                saveComboBox.Items.Remove(selectedSave);


                saveComboBox.SelectedIndex = -1;
            }
        }

        private static void LoadSavedConfigurations()
        {
            string[] saveFiles = Directory.GetFiles(saveFolderPath, "*.json");


            savedConfigurations = new List<string>();

            foreach (string saveFile in saveFiles)
            {
                string saveName = Path.GetFileNameWithoutExtension(saveFile);


                saveComboBox.Items.Add(saveName);
                savedConfigurations.Add(saveName);
            }
        }

    }
}