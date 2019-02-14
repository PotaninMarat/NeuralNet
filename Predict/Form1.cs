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
using NeuralNetwork;
using System.Diagnostics;

namespace Predict
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Vector y;
        Vector interval = new Vector(1);
        int dist = 100;
        int pad = 0;
        private void buttonPredict_Click(object sender, EventArgs e)
        {
            List<double> res = new List<double>();

            //net.Calculation(new Vector(new double[] { y[0] }).ToTensor4());
           // res.Add(net.output.elements[0]);


            /*
            for (int i = 0; i < y.Length - interval.Length - dist; i+= interval.Length)
            {
                for (int j = 0; j < interval.Length; j++)
                {
                    interval[j] = y[i + j];
                }
                net.Calculation(interval.ToTensor4());
                res.Add(net.output[0, 0, 0, 0]);
            }
            //*/
            matlib.ZedGraph.DrawZedGraphCurve(zedGraphControl1, y.elements, 1, y.Length, Color.Green);
            matlib.ZedGraph.DrawZedGraphCurve(zedGraphControl2, res.ToArray(), 1, res.Count, Color.Green);
        }

        private void buttonTrain_Click(object sender, EventArgs e)
        {
            StopTrain = !StopTrain;
            //if (!backgroundWorker1.IsBusy)
            //    backgroundWorker1.RunWorkerAsync();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
           
        }
        int All, True;
        List<double> predict = new List<double>();
        List<double> ideal = new List<double>();
        double f(double x)
        {
            x += pad;
            return Math.Sin((pad + x) / 15.0) * Math.Log10(1.0 + (pad + x) / 10.0) * Math.Cos((pad + x) / 25.0) + matlib.random.NextDouble() *

            0                         
            //matlib.Function.Optimize.MinimizationObjFunc.MethodConjGrad()
            ;
        }
        bool StopTrain;
        private void timer1_Tick(object sender, EventArgs e)
        {
            All++;
            int numPoints = 150;
            y = new Vector(numPoints + dist); for (int i = 0; i < y.Length; i++) y[i] = f(i);

            pad++;

            matlib.ZedGraph.DrawZedGraphCurve(zedGraphControl1, y.elements, 1, y.Length, Color.DarkRed);

            Vector x = new Vector(y.GetInterval(0, numPoints - 1));

            Vector yx = new Vector(x.Length);
            for (int i = 0; i < x.Length; i++) {
			   yx[i] = i;
			}
           // double RealValue = matlib.Function.Interpolation.Taylor(yx, x, new Vector(new double[] { x.Length + (double)dist }), 4)[0];
                //matlib.Function.Interpolation.Lagrange(yx, x, new Vector(new double[] { x.Length + (double)dist }), 5)[0];
            int Ideal = (((y[y.Length - 1] - x.elements[x.Length - 1] >= 0) ? 1 : 0));
            //int Real = (((RealValue - x.elements[x.Length - 1] >= 0) ? 1 : 0));


           // this.predict.Add(Real);
            ideal.Add(Ideal);

           // label1.BeginInvoke((MethodInvoker)(()=>{label1.Text = "" + RealValue;}));
            if (ideal.Count > y.Length)
            {
                ideal.RemoveAt(0);
                this.predict.RemoveAt(0);
            }
                //real =
             matlib.ZedGraph.DrawZedGraphClear(zedGraphControl2);
             matlib.ZedGraph.DrawZedGraphCurveAdd(zedGraphControl2, this.predict.ToArray(), 1, this.predict.Count, Color.DarkRed);
             matlib.ZedGraph.DrawZedGraphCurveAdd(zedGraphControl2, ideal.ToArray(), 1, ideal.Count, Color.Green);



        }
    }
}
