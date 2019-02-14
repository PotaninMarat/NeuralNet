using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MatLib;
using NeuralNetwork.ReadyNeuralNetworks;
using MatLib.Interpolation;
using MatLib.Regression;
namespace InterpolaionNeuralNet
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Vector x, y;
        Vector x_, y_;
        TrygonomApproxim approx;
        private void Form1_Load(object sender, EventArgs e)
        {
            #region
            //*
            approx = new TrygonomApproxim(20);
            //*/
            #endregion
            x = Vector.GetVectorIndexes(1024 * 4);
            x /= (double)x.Length;
            y = new Vector(x.Length);

            for (int i = 0; i < x.Length; i++)
            {
                y[i] = f(x[i]);
            }



           // var A = approx.Learn(x, y);
          //  MessageBox.Show(A.ToString());

        }
        double f(double x)
        {
            //x *=0.1;
            double s =// Math.Log(Math.Abs(x))+//Math.Sin(2.0 * 5.0 * Math.PI * x) +
                // Math.Sin(2.0 * 3.2 * Math.PI * x) +
               //  Math.Sin(2.0 * 11.8 * Math.PI * x) +
               //    Math.Sin(2.0 * 18.3 * Math.PI * x) +
                //   Math.Sin(2.0 * 14.65 * Math.PI * x) +
               //    Math.Sin(2.0 * 1.0 * Math.PI * x) +
                   2*x*x-7.0*x+
               //      Math.Sin(2.0 * 3.0 * Math.PI * x) +
              //      Math.Sin(2.0 * 2.0 * Math.PI * x) +
              //         Math.Sin(2.0 * 1.0 * Math.PI * x) +
              //               Math.Sin(2.0 * -5.0 * Math.PI * x) +
             //             -Math.Sin(6.3 * 2.0 * Math.PI * x) +
             //          Math.Abs(x - 100) +
             //          matlib.Statistic.Random.Gauss() * 0.4 +

                //       +Math.Log(x)+
            + 1.0;

            return s;
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int n = 1, m = 2;
            approx.Learn(x.GetInterval(0, n * x.Length / m - 1), y.GetInterval(0, n * x.Length / m - 1));

            ZedGraph.ZedGraphControl zgc = new ZedGraph.ZedGraphControl();
            //var elem = InterpolBase.Taylor(x, y, Vector.GetVectorIndexes(x.Length) / (n * x.Length / m), 15).elements;
            
            var elem = approx.Calculation(x).elements;
            matlib.ZedGraph.DrawZedGraphCurve(zgc, elem, 1, x.Length, Color.Red);
          //  matlib.ZedGraph.DrawZedGraphCurve(zedGraphControl1, approx.Calculation(Vector.GetVectorIndexes(x.Length + u) / (double)(x.Length)).elements, 1, x.Length + u, Color.Red);
          // matlib.ZedGraph.DrawZedGraphCurveAdd(zedGraphControl1, y.elements);
            y.VisualizeAdd("zalupa", zgc);
        }
    }
}
