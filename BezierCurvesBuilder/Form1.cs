using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BezierCurvesBuilder
{
    public partial class Form1 : Form
    {

        private BezierPath figure;

        public Form1()
        {
            figure = new BezierPath();
            time.Tick += new EventHandler(Animate);
            InitializeComponent();
        }

        private int selectedPoint = -1; //unselected

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                for (int i = 0; i < figure.controlPointFs.Count; i++)
                {
                    if (IsMouseOverThePoint(new Point(e.X, e.Y), figure.controlPointFs[i]) && selectedPoint < 0)
                    {
                        selectedPoint = i;
                        break;
                    }
                }
                if (selectedPoint >= 0 && checkBox1.Checked)
                {
                    figure.controlPointFs[selectedPoint] = new PointF(e.X, e.Y);
                    pictureBox1.Invalidate();
                }
            }
        }

        private bool IsMouseOverThePoint(Point mouse, PointF point)
        {
            return (Math.Abs(mouse.X - point.X) <= 5 &&
                    Math.Abs(mouse.Y - point.Y) <= 5);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            selectedPoint = -1;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (figure.IsEmpty())
            {
                figure.AddControlPoint(new PointF(50, 50));
                figure.AddControlPoint(new PointF(100, 20));
                figure.AddControlPoint(new PointF(150, 50));
                pictureBox1.Invalidate();
            }
            else
            {
                PointF last = figure.controlPointFs[figure.controlPointFs.Count - 1];
                figure.AddControlPoint(new PointF(last.X + 40, 20));
                figure.AddControlPoint(new PointF(last.X + 80, 50));
            }
            pictureBox1.Invalidate();
        }


        private Pen pen = new Pen(Color.MidnightBlue, 4);
        private Pen pen2 = new Pen(Color.Red, 2);
        private Pen pen3 = new Pen(Color.Red, 5);

        private void DrawGrid(PaintEventArgs e)
        {
            Font font = new Font("Microsoft Sans Serif", 8);
            Brush brush = new SolidBrush(Color.Black);
            for (int i = 0; i < pictureBox1.Width; i += 50)
            {
                e.Graphics.DrawLine(new Pen(Color.Black,0.5f), i,0,i,pictureBox1.Height);
                e.Graphics.DrawString((i).ToString(),font,brush,i,0);
            }
            for (int i = 0; i < pictureBox1.Height; i += 50)
            {
                e.Graphics.DrawLine(new Pen(Color.Black, 0.5f), 0,i, pictureBox1.Width,i);
                e.Graphics.DrawString((i).ToString(), font, brush, 0,i);
            }

        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            DrawGrid(e);
            var t = figure.GetDrawingPointFs();
            for (int i = 1; i < t.Count; i++)
            {
                e.Graphics.DrawLine(pen, t[i - 1], t[i]);
            }

            t = figure.controlPointFs;
            if (t.Any() && checkBox1.Checked)
            {
                e.Graphics.DrawEllipse(pen3, t[0].X, t[0].Y, 5, 5);
                for (int i = 1; i < t.Count; i++)
                {
                    e.Graphics.DrawEllipse(pen3, t[i].X, t[i].Y, 5, 5);
                    e.Graphics.DrawLine(pen2, t[i - 1], t[i]);
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            pictureBox1.Invalidate();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            string path;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog1.FileName, "");
                foreach (var point in figure.controlPointFs)
                {
                    File.AppendAllText(saveFileDialog1.FileName,
                        point.X + Environment.NewLine + point.Y + Environment.NewLine);
                }
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked)
            {
                pictureBox1.Image = new Bitmap("./Shark3.jpg");

            }
            else
            {
                pictureBox1.Image = null;

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                List<PointF> vertex = new List<PointF>();
                StreamReader str = new StreamReader(openFileDialog1.FileName);
                while (!str.EndOfStream)
                {
                    var x = int.Parse(str.ReadLine());
                    var y = int.Parse(str.ReadLine());
                    vertex.Add(new PointF(x, y));
                }
                figure.controlPointFs = vertex;
                pictureBox1.Invalidate();
            }
        }

        List<PointF> anim1 = new List<PointF>();
        List<PointF> anim2 = new List<PointF>();

        private int currentFrame = 0;
        private int maxFrame = 100;
        private bool goesUp = true;
        Timer time = new Timer();
        private void button4_Click(object sender, EventArgs e)
        {

            if (time.Enabled)
            {
                time.Stop();
                return;
            }

            anim1.Clear();
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            List<PointF> vertex = new List<PointF>();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                StreamReader str = new StreamReader(openFileDialog1.FileName);
                while (!str.EndOfStream)
                {
                    var x = int.Parse(str.ReadLine());
                    var y = int.Parse(str.ReadLine());
                    anim1.Add(new PointF(x, y));
                }
                //figure.controlPointFs = vertex;
                //pictureBox1.Invalidate();


            }
            else
            {
                return;
            }
            anim2.Clear();
            OpenFileDialog openFileDialog2 = new OpenFileDialog();
            openFileDialog2.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog2.FilterIndex = 2;
            openFileDialog2.RestoreDirectory = true;
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {

                StreamReader str = new StreamReader(openFileDialog2.FileName);
                while (!str.EndOfStream)
                {
                    var x = int.Parse(str.ReadLine());
                    var y = int.Parse(str.ReadLine());
                    anim2.Add(new PointF(x, y));
                }
                //figure.controlPointFs = vertex;
                //pictureBox1.Invalidate();

                
               
                time.Interval = 7;
                figure.controlPointFs = new PointF[anim1.Count].ToList();//To fill the main list
                time.Start();

            }
            else
            {
                return;
            }
        }

        public void Animate(Object myObject, EventArgs myEventArgs)
        {
            if (currentFrame == 0)
                goesUp = true;
            if (currentFrame == maxFrame)
                goesUp = false;

            for (int i = 0; i < anim1.Count; i++)
            {
                figure.controlPointFs[i] = new PointF(anim2[i].X - (anim2[i].X - anim1[i].X) * currentFrame / maxFrame,
                     anim2[i].Y - (anim2[i].Y - anim1[i].Y) * currentFrame / maxFrame);               
            }

            if (goesUp)
            {
                currentFrame++;
            }
            else
            {
                currentFrame--;
            }
            pictureBox1.Invalidate();

        }

        private void button5_Click(object sender, EventArgs e)
        {
            bool rotation = true;
            var m = (float)numericUpDown1.Value;
            var n = (float)numericUpDown2.Value;
            var angle = (float)numericUpDown3.Value;
            angle *= (float)Math.PI / 180.0F;
            if (angle == 0) rotation= false;
            for (int i = 0; i < figure.controlPointFs.Count; i++)
            {
                if(rotation)
                figure.controlPointFs[i] = new PointF(figure.controlPointFs[i].X - m, figure.controlPointFs[i].Y - n);
                if (rotation)
                    figure.controlPointFs[i] = new PointF(Convert.ToSingle(figure.controlPointFs[i].X *
                Math.Cos(angle) - figure.controlPointFs[i].Y *
                Math.Sin(angle))
    ,
                Convert.ToSingle(figure.controlPointFs[i].X *
                Math.Sin(angle) + figure.controlPointFs[i].Y *
                Math.Cos(angle)))
                ;
                figure.controlPointFs[i] = new PointF(figure.controlPointFs[i].X + m, figure.controlPointFs[i].Y + n);
            }
            pictureBox1.Invalidate();
        }
    }

}
