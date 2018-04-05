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
using System.Timers;

namespace Fractal
{
    public partial class Form1 : Form
    {
        private bool isFirstRun = true;
        private int MAX = 256;      // max iterations
        private double SX = -2.025; // start value real
        private double SY = -1.125; // start value imaginary
        private double EX = 0.6;    // end value real
        private double EY = 1.125;  // end value imaginary

        private static int x1, y1, xs, ys, xe, ye;
        private static double xstart, ystart, xende, yende, xzoom, yzoom;
        private static float xy;
        private Image picture;
        private Graphics g1;
        private HSB HSBcol = new HSB();
        private Pen pen;
        private bool isClicked;
        private static bool action, rectangle, finished;
       // int val;

    


        public Form1()
        {
            InitializeComponent();
            init();
            start();
        }

        public void init()
        {
            finished = false;
            x1 = pictureBox1.Width;
            y1 = pictureBox1.Height;
            xy = (float)x1 / (float)y1;
            picture = new Bitmap(x1, y1);
            g1 = Graphics.FromImage(picture);
            finished = true;
            isClicked = false;
           // val = 0;
        }

        public void start()
        {
            action = false;
            rectangle = false;
            initvalues();
            xzoom = (xende - xstart) / (double)x1;
            yzoom = (yende - ystart) / (double)y1;
            mandelbrot();


        }
        private void pictureBox1_Paint_1(object sender, PaintEventArgs e)
        {
            Graphics obj = e.Graphics;
            obj.DrawImage(picture, new Point(0, 0));
            
        }
       

        private void mandelbrot(int num = 0) // calculate all points
        {
            int x, y;
            float h, b, alt = 0.0f;

            action = false;
            //Dispalying message in text box before mandelbrot is ready
            textBox1.Text = "Mandelbrot-Set will be produced - please wait...";
           textBox1.Enabled = false;
            for (x = 0; x < x1; x += 2)
            {
                for (y = 0; y < y1; y++)
                {

                    h = pointcolour(xstart + xzoom * (double)x, ystart + yzoom * (double)y); // color value

                    if (h != alt)
                    {
                        b = 1.0f - h * h; // brightnes
                                          ///djm added

                        HSBcol.fromHSB(h * 255, 0.8f * 255, b * 255, num); //convert hsb to rgb then make a Java Color
                        Color col = Color.FromArgb((int)HSBcol.rChan, (int)HSBcol.gChan, (int)HSBcol.bChan);
                        pen = new Pen(col);

                        alt = h;
                    }

                    g1.DrawLine(pen, x, y, x + 1, y);

                }
            }
            //Displaying message after mandlebrot is ready
            Cursor.Current = Cursors.Cross;
           textBox1.Text = "Mandelbrot-Set ready - please select and zoom with pressed mouse." +
                " Different options are available on 'Menu'";
            textBox1.Enabled = false;

            action = true;
        }

        private float pointcolour(double xwert, double ywert) // color value from 0.0 to 1.0 by iterations
        {
            double r = 0.0, i = 0.0, m = 0.0;
            int j = 0;

            while ((j < MAX) && (m < 4.0))
            {
                j++;
                m = r * r - i * i;
                i = 2.0 * r * i + ywert;
                r = m + xwert;
            }
            return (float)j / (float)MAX;
        }

        private void initvalues() // reset start values
        {
            string path = Directory.GetCurrentDirectory();

            List<double> l = null;
            if (isFirstRun)
            {
                try
                {
                    StreamReader sr = new StreamReader("values.txt");
                    Console.WriteLine((sr == null) + " ");
                    l = new List<double>();

                    double a = 0;
                    while ((a = Convert.ToDouble(sr.ReadLine())) != 0)

                        l.Add(a);
                }

                catch (Exception ex)
                {

                    Console.WriteLine("Exception" + ex);
                }

                try
                {
                    xstart = l[0];
                    ystart = l[1];
                    xende = l[2];
                    yende = l[3];
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    Console.WriteLine("Exception" + ex);
                }
                isFirstRun = false;
            }
            else
            {
                

                xstart = SX;
                ystart = SY;
                xende = EX;
                yende = EY;



                if ((float)((xende - xstart) / (yende - ystart)) != xy)
                    xstart = xende - (yende - ystart) * (double)xy;
            }
        }
        
        public void destroy() // delete all instances 
        {
            if (finished)
            {

                picture = null;
                g1 = null;
                // garbage collection
            }
        }
        public void update()
        {
            try
            {
                Graphics g = pictureBox1.CreateGraphics();
                g.DrawImage(picture, 0, 0);
                if (rectangle)
                {

                    Pen mypen = new Pen(Color.White, 1);
                    if (xs < xe)
                    {
                        if (ys < ye) g.DrawRectangle(mypen, xs, ys, (xe - xs), (ye - ys));
                        else g.DrawRectangle(mypen, xs, ye, (xe - xs), (ys - ye));
                    }
                    else
                    {
                        if (ys < ye) g.DrawRectangle(mypen, xe, ys, (xs - xe), (ye - ys));
                        else g.DrawRectangle(mypen, xe, ye, (xs - xe), (ys - ye));
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
            private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            isClicked = true;
            if (action)
            {
                xs = e.X;
                ys = e.Y;
            }
        }

            private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
            {
            if (isClicked) {
                if (action)
                {
                    xe = e.X;
                    ye = e.Y;
                    rectangle = true;
                    update();
                }  
            }
        }

        private void startColorCyclingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void stopToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private int timerInt = 1;
        private void timer1_Tick_1(object sender, EventArgs e)
        {
            timerInt++;

            if (timerInt >= 8)
            {
                timerInt = 1;
            }
            else
            {
                mandelbrot(timerInt);
                update();
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
            {
            int z, w;
            if (action)
            {
                xe = e.X;
                ye = e.Y;
                if (xs > xe)
                {
                    z = xs;
                    xs = xe;
                    xe = z;
                }
                if (ys > ye)
                {
                    z = ys;
                    ys = ye;
                    ye = z;
                }
                w = (xe - xs);
                z = (ye - ys);
                if ((w < 2) && (z < 2)) initvalues();
                else
                {
                    if (((float)w > (float)z * xy)) ye = (int)((float)ys + (float)w / xy);
                    else xe = (int)((float)xs + (float)z * xy);
                    xende = xstart + xzoom * (double)xe;
                    yende = ystart + yzoom * (double)ye;
                    xstart += xzoom * (double)xs;
                    ystart += yzoom * (double)ys;
                }
                xzoom = (xende - xstart) / (double)x1;
                yzoom = (yende - ystart) / (double)y1;

                mandelbrot();
                
                rectangle = false;
                isClicked = false;
                update();
            }

            try
            {
                StreamWriter sx = new StreamWriter("values.txt");
                sx.WriteLine(xstart);
                sx.WriteLine(ystart);
                sx.WriteLine(xende);
                sx.WriteLine(yende);
                sx.Close();

            }
            catch (Exception ex)
            {
                Console.Write("Exception" + ex);
            }
        }
        public void getInfo()
        {
            MessageBox.Show("Mandelbrot By:" + Environment.NewLine +
                "Deepesh K.C." + Environment.NewLine +
                "The British College" + Environment.NewLine +
                "Email: deepeshkc@live.com" + Environment.NewLine +
                "Contact No: +9779843202087" + Environment.NewLine,
                "About"
                );
        }
        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawImage(pictureBox1.Image, 0, 0);
        }

        

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            start();
            mandelbrot();
            update();
         

        }

        private void cloneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 clone = new Form1();
            clone.Show();
            isFirstRun = false;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog f = new SaveFileDialog();
            f.Filter = "JPG(*.JPG) | *.JPG";
            if (f.ShowDialog() == DialogResult.OK)
            {
                picture.Save(f.FileName);
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            {
                DialogResult dialogResult = MessageBox.Show("Do You want to exit ?", "Exit", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    this.Close();
                }
                else if (dialogResult == DialogResult.No)
                {

                }
            }
        }

        private void changeColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Random rd = new Random();

            mandelbrot(rd.Next(1, 7));
            update();
           
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getInfo();
        }

    }
        }


    

