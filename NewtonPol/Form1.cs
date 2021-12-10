using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;
using System.IO;

namespace NewtonPol
{
    public partial class Form1 : Form
    {
        public const double PI = 3.1415926535897931;
        public Form1()
        {
            InitializeComponent();
        }
        ZedGraphControl zedGrapgControl1 = new ZedGraphControl();
        protected override void OnLoad(EventArgs e)
        {
            zedGrapgControl1.Location = new Point(10, 30);
            zedGrapgControl1.Name = "text";
            zedGrapgControl1.Size = new Size(550, 550);
            Controls.Add(zedGrapgControl1);
            GraphPane my_Pane = zedGrapgControl1.GraphPane;
            my_Pane.Title.Text = "Solution's result";
            my_Pane.XAxis.Title.Text = "My X";
            my_Pane.YAxis.Title.Text = "My Y";

        }
        private void GetSize()
        {
            zedGrapgControl1.Location = new Point(10, 10);
            zedGrapgControl1.Size = new Size(ClientRectangle.Width - 20, ClientRectangle.Height - 20);

        }
        protected override void OnSizeChanged(EventArgs e)
        {
            GetSize();
        }

      
        public static double F(double x)
        {
            return Math.Pow(x, 3) - 3 * Math.Pow(x, 2) - 8 * Math.Abs(x);
        }
        PointPairList listIsh = new PointPairList();
        public static PointPairList listNew = new PointPairList();

        double Newton(double x, int n, double[] x_arr, double[] y_arr)
        {

            double sum = y_arr[0];
            for (int i = 1; i < n; ++i)
            {

                double F = 0;
                for (int j = 0; j <= i; ++j)
                {

                    double den = 1;
                    for (int k = 0; k <= i; ++k)
                        if (k != j)
                            den *= (x_arr[j] - x_arr[k]);
                   
                    F += y_arr[j] / den;
                }

                
                for (int k = 0; k < i; ++k)
                    F *= (x - x_arr[k]);
                sum += F;
            }
            return sum;
        }
        private void ogo(ZedGraphControl Zed_GraphControl)
        {
            double[] X = new double[] { 0, 2, 4, 6, 8, 10, 12, 14, 16, 18 };
            double[] Y = new double[X.Length];
            double x = 3;
            for (int i = 0; i < X.Length; i++)
            {
                Y[i] = F(X[i]);
                listIsh.Add(X[i], Y[i]);
            }
            GraphPane my_Pane = Zed_GraphControl.GraphPane;
            Newton interpolation = new Newton();
            
            double y = interpolation.GetValue(X, Y, x);

            LineItem myCircle = my_Pane.AddCurve("Исх", listIsh, Color.Red, SymbolType.Circle);
            LineItem myCircl2e = my_Pane.AddCurve("Исх2", listNew, Color.Blue, SymbolType.Triangle);
            zedGrapgControl1.AxisChange();
            zedGrapgControl1.Invalidate();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ogo(zedGrapgControl1);
        }
    }
    class Newton
    {
        public double dy(List<double> Y, List<double> X)
        {
            if (Y.Count > 2)
            {
                List<double> Yleft = new List<double>(Y);
                List<double> Xleft = new List<double>(X);
                Xleft.RemoveAt(0);
                Yleft.RemoveAt(0);
                List<double> Yright = new List<double>(Y);
                List<double> Xright = new List<double>(X);
                Xright.RemoveAt(Y.Count - 1);
                Yright.RemoveAt(Y.Count - 1);
                return (dy(Yleft, Xleft) - dy(Yright, Xright)) / (X[X.Count - 1] - X[0]);
            }
            else if (Y.Count == 2)
            {
                return (Y[1] - Y[0]) / (X[1] - X[0]);
            }
            else
            {
                throw new Exception("Not available parameter");
            }
        }

        public double GetValue(double[] X, double[] Y, double x)
        {
            
            double res = Y[0];
            double buf;
            List<double> Xlist;
            List<double> Ylist;
            for (int i = 1; i < Y.Length; i++)
            {
                Xlist = new List<double>();
                Ylist = new List<double>();
                buf = 1;
                for (int j = 0; j <= i; j++)
                {
                    Xlist.Add(X[j]);
                    Ylist.Add(Y[j]);
                    if (j < i)
                        buf *= x - X[j];
                    
                }
                res += dy(Ylist, Xlist) * buf;
                MessageBox.Show(Convert.ToString(res));
                Form1.listNew.Add(X[i], dy(Ylist, Xlist) * buf);
            }

            return res;
        }

        

        public double dy_h(List<double> Y, List<double> X, int number, int index)
        {
            if (number > 1)
            {
                return (dy_h(Y, X, number - 1, index + 1) - dy_h(Y, X, number - 1, index));
            }
            else if (number == 1)
            {
                return (Y[index + 1] - Y[index]);
            }
            else
            {
                throw new Exception("Not available parameter");
            }
        }

        public double GetValue(double[] X, double[] Y, double x, double h)
        {
            double res = Y[0];
            double buf;
            List<double> Xlist = new List<double>(X);
            List<double> Ylist = new List<double>(Y);
            double q = (x - X[0]) / h;
            buf = 1;
            Form1.listNew.Add(X[0], res);
            for (int i = 1; i < Y.Length; i++)
            {
                buf *= (q - i + 1) / i;
                res += dy_h(Ylist, Xlist, i, 0) * buf;
                Form1.listNew.Add(X[i], res);
            }
            return res;
        }
    }
   
}
