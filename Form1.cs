using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AdvancedSoftwareEngineeringPart1
{
    public partial class MainForm : Form
    {
        public readonly CommandParser commandParser;
        public readonly List<string> programCommands = new List<string>();

        public MainForm()
        {
            InitializeComponent();
            commandParser = new CommandParser(panel1.CreateGraphics(), panel1.Size);
        }


        public void button4_Click(object sender, EventArgs e)
        {
            try
            {
                SyntaxCheck();
                MessageBox.Show("Syntax check passed successfully.", "Syntax Check", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Syntax check failed: {ex.Message}", "Syntax Check", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public void SyntaxCheck()
        {
            foreach (string command in programCommands)
            {
                commandParser.SyntaxCheck(command);
            }
        }

        public void UpdateProgramTextBox()
        {
            ProgramTextBox.Text = string.Join(Environment.NewLine, programCommands);
        }


        public void button3_Click(object sender, EventArgs e)
        {
            string command = CommandTextBox.Text;
            programCommands.Add(command);
            UpdateProgramTextBox();
            CommandTextBox.Clear();
        }
        public void ProgramTextBox_TextChanged(object sender, EventArgs e)
        {
            // Not implemented for this example
        }

        public void RunButton_Click_1(object sender, EventArgs e)
        {
            try
            {
                programCommands.Clear();
                programCommands.AddRange(ProgramTextBox.Lines);
                commandParser.ExecuteProgram(programCommands);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                programCommands.Clear();
                programCommands.AddRange(File.ReadAllLines(openFileDialog.FileName));
                UpdateProgramTextBox();
            }
        }

        public void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllLines(saveFileDialog.FileName, programCommands);
            }
        }


    }

    public class CommandParser
    {
        public readonly Graphics graphics;
        public readonly Pen pen;
        public readonly Size panelSize;
        public bool fillEnabled = false;

        /// <summary>
        // public global::TestProjectPart1.FakeGraphics FakeGraphics { get; }
        /// </summary>
        public Size Size { get; }

        public CommandParser(Graphics graphics, Size panelSize)
        {
            this.graphics = graphics;
            this.panelSize = panelSize;
            pen = new Pen(Color.Black); // Default pen color
        }


       

        public void ExecuteProgram(List<string> commands)
        {
            foreach (string command in commands)
            {
                ExecuteCommand(command);
            }
        }

        public void ExecuteCommand(string command)
        {
            string[] parts = command.Split(' ');
            string cmd = parts[0].ToLower();

            switch (cmd)
            {
                case "position":
                    if (parts.Length < 3)
                        throw new ArgumentException("Invalid number of parameters for position command");
                    int x = int.Parse(parts[1]);
                    int y = int.Parse(parts[2]);
                    graphics.ResetTransform();
                    graphics.TranslateTransform(x, y);
                    break;

                case "pen":
                    if (parts.Length < 2)
                        throw new ArgumentException("Invalid number of parameters for pen command");
                    SetPenColor(parts[1]);
                    break;

                case "draw":
                    if (parts.Length < 3)
                        throw new ArgumentException("Invalid number of parameters for draw command");
                    int endX = int.Parse(parts[1]);
                    int endY = int.Parse(parts[2]);
                    if (fillEnabled)
                        graphics.FillRectangle(pen.Brush, endX, endY, 1, 1);
                    else
                        graphics.DrawLine(pen, 0, 0, endX, endY);
                    break;

                case "clear":
                    graphics.Clear(Color.White);
                    break;

                case "reset":
                    graphics.ResetTransform();
                    break;

                case "rectangle":
                    if (parts.Length < 3)
                        throw new ArgumentException("Invalid number of parameters for rectangle command");
                    int width = int.Parse(parts[1]);
                    int height = int.Parse(parts[2]);
                    if (fillEnabled)
                        graphics.FillRectangle(pen.Brush, 0, 0, width, height);
                    else
                        graphics.DrawRectangle(pen, 0, 0, width, height);
                    break;

                case "circle":
                    if (parts.Length < 2)
                        throw new ArgumentException("Invalid number of parameters for circle command");
                    int radius = int.Parse(parts[1]);
                    int diameter = radius * 2;
                    if (fillEnabled)
                        graphics.FillEllipse(pen.Brush, 0, 0, diameter, diameter);
                    else
                        graphics.DrawEllipse(pen, 0, 0, diameter, diameter);
                    break;

                case "triangle":
                    if (parts.Length < 7)
                        throw new ArgumentException("Invalid number of parameters for triangle command");
                    Point[] trianglePoints = new Point[3];
                    trianglePoints[0] = new Point(int.Parse(parts[1]), int.Parse(parts[2]));
                    trianglePoints[1] = new Point(int.Parse(parts[3]), int.Parse(parts[4]));
                    trianglePoints[2] = new Point(int.Parse(parts[5]), int.Parse(parts[6]));
                    if (fillEnabled)
                        graphics.FillPolygon(pen.Brush, trianglePoints);
                    else
                        graphics.DrawPolygon(pen, trianglePoints);
                    break;

                case "fill":
                    if (parts.Length < 2)
                        throw new ArgumentException("Invalid number of parameters for fill command");
                    if (parts[1].ToLower() == "on")
                        fillEnabled = true;
                    else if (parts[1].ToLower() == "off")
                        fillEnabled = false;
                    break;

                default:
                    throw new ArgumentException($"Invalid command: {cmd}");
            }
        }

        public void SyntaxCheck(string command)
        {
            // Check for valid commands
            if (!Regex.IsMatch(command, @"^(position|pen|draw|clear|reset|rectangle|circle|triangle|fill)\s+(\w+\s*)*$", RegexOptions.IgnoreCase))
                throw new ArgumentException($"Invalid command syntax: {command}");

            string[] parts = command.Split(' ');
            string cmd = parts[0].ToLower();

            // Check for valid parameters
            switch (cmd)
            {
                case "position":
                    if (parts.Length < 3 || !int.TryParse(parts[1], out _) || !int.TryParse(parts[2], out _))
                        throw new ArgumentException($"Invalid parameters for position command: {command}");
                    break;

                case "pen":
                    if (parts.Length < 2 || !IsValidColor(parts[1]))
                        throw new ArgumentException($"Invalid parameters for pen command: {command}");
                    break;

                case "draw":
                    if (parts.Length < 3 || !int.TryParse(parts[1], out _) || !int.TryParse(parts[2], out _))
                        throw new ArgumentException($"Invalid parameters for draw command: {command}");
                    break;

                case "rectangle":
                    if (parts.Length < 3 || !int.TryParse(parts[1], out _) || !int.TryParse(parts[2], out _))
                        throw new ArgumentException($"Invalid parameters for rectangle command: {command}");
                    break;

                case "circle":
                    if (parts.Length < 2 || !int.TryParse(parts[1], out _))
                        throw new ArgumentException($"Invalid parameters for circle command: {command}");
                    break;

                case "fill":
                    if (parts.Length < 2 || (parts[1].ToLower() != "on" && parts[1].ToLower() != "off"))
                        throw new ArgumentException($"Invalid parameters for fill command: {command}");
                    break;

                case "triangle":
                    if (parts.Length < 7 || !int.TryParse(parts[1], out _) || !int.TryParse(parts[2], out _)
                        || !int.TryParse(parts[3], out _) || !int.TryParse(parts[4], out _) || !int.TryParse(parts[5], out _)
                        || !int.TryParse(parts[6], out _))
                        throw new ArgumentException($"Invalid parameters for triangle command: {command}");
                    break;

                default:
                    break;
            }
        }

        public void SetPenColor(string color)
        {
            switch (color.ToLower())
            {
                case "red":
                    pen.Color = Color.Red;
                    break;
                case "green":
                    pen.Color = Color.Green;
                    break;
                case "blue":
                    pen.Color = Color.Blue;
                    break;
                case "black":
                    pen.Color = Color.Black;
                    break;
                default:
                    throw new ArgumentException($"Invalid pen color: {color}");
            }
        }

        public bool IsValidColor(string color)
        {
            switch (color.ToLower())
            {
                case "red":
                case "green":
                case "blue":
                case "black":
                    return true;
                default:
                    return false;
            }
        }
    }
}
