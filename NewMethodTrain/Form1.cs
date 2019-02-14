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

namespace NewMethodTrain
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        Vector w = new Vector(new double[]{0.4, -0.6});
        double f(Vector x)
        {
            return Math.Sin(w * x);
        }
        double dfdx(Vector x)
        {
            return Math.Cos(x * w);
        }
        double ddfddx(Vector x)
        {
            return -Math.Sin(x * w);
        }
        double E(Vector x, double y)
        {
            return 0.5 * (f(x) - y) * (f(x) - y);
        }
        double dEdw(Vector x, double y, int index)
        {
            return (f(x) - y) * dfdx(x) * x[index];
        }
        double allErr(Vector[]x, double[] y)
        {
            double res = 0.0;
            for (int i = 0; i < x.Length; i++)
            {
                res += E(x[i], y[i]);
            }
            return (double)res / x.Length;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            NeuralNet net = new NeuralNet(new string[] {
                "input:2:1:1:1",

                "direct:5",
                "sigmoid",

                "direct:1",
                "sigmoid"
            });
            Vector[] x = new Vector[4];
            Vector[] y = new Vector[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                x[i] = new Vector(2);
                y[i] = new Vector(1);
            }
            int ctr = 0;
            for (int a = 0; a < 2; a++)
            {
                for (int b = 0; b < 2; b++)
                {
                    x[ctr][0] = a;
                    x[ctr][1] = b;
                    y[ctr++][0] = a ^ b;
                }
            }
            /*
            Tensor4 x = new Tensor4(2, 1, 1, 7);
            Tensor4 y = new Tensor4(1, 1, 1, 7);
            x.ToVector().Vusialize();
            int ctr = 0;
            x[ctr, 0, 0, 0] = 2; x[ctr, 0, 0, 1] = 1; y[ctr++, 0, 0, 0] = 1;
            x[ctr, 0, 0, 0] = 3; x[ctr, 0, 0, 1] = 1.0; y[ctr++, 0, 0, 0] = 0.1;
            x[ctr, 0, 0, 0] = 2; x[ctr, 0, 0, 1] = 2; y[ctr++, 0, 0, 0] = 0.7;
            x[ctr, 0, 0, 0] = 1; x[ctr, 0, 0, 1] = -1.5; y[ctr++, 0, 0, 0] = 0.5;
            x[ctr, 0, 0, 0] = 3; x[ctr, 0, 0, 1] = 2.2; y[ctr++, 0, 0, 0] = 0.4;
            x[ctr, 0, 0, 0] = 2; x[ctr, 0, 0, 1] = 1.5; y[ctr++, 0, 0, 0] = 0.6;
            x[ctr, 0, 0, 0] = 3; x[ctr, 0, 0, 1] = 1.3; y[ctr++, 0, 0, 0] = 0.9;

            //*/
            MessageBox.Show("" + net.CalcErrRootMSE(x, y));
            double n = 0.01;
            for (int ep = 1; ep <= 5000; ep++)
            {
                for (int i = 0; i < x.Length; i++)
                net.TrainWithTeach(x[i].ToTensor4(), y[i].ToTensor4(), n, 0.9, 0.0001, NeuralNet.Optimizer.SGD);

               // n = n / Math.Sqrt(ep);
            }
            MessageBox.Show("" + net.CalcErrRootMSE(x, y));

            //*
            net.Calculation(new Vector(new double[] { 0, 0 }));
            MessageBox.Show("0, 0: " + net.output[0, 0, 0, 0]);

            net.Calculation(new Vector(new double[] { 0, 1 }));
            MessageBox.Show("0, 1: " + net.output[0, 0, 0, 0]);

            net.Calculation(new Vector(new double[] { 1, 0 }));
            MessageBox.Show("1, 0: " + net.output[0, 0, 0, 0]);

            net.Calculation(new Vector(new double[] { 1, 1 }));
            MessageBox.Show("1, 1: " + net.output[0, 0, 0, 0]);
            //*/
            #region
            /*
            Vector[] x = new Vector[4];
            double[] y = new double[x.Length];

            for (int i = 0; i < x.Length; i++)
            {
                x[i] = new Vector(2);  
            }
            int ctr = 0;
            x[ctr][0] = 0; x[ctr][1] = 0; y[ctr++] = 0;
            x[ctr][0] = 1.0; x[ctr][1] = 0; y[ctr++] = 1.0;
            x[ctr][0] = 0; x[ctr][1] = 1.0; y[ctr++] = 1.0;
            x[ctr][0] = 1.0; x[ctr][1] = 1.0; y[ctr++] = 0;
            #region
            /*
            for (int ep = 0; ep < 100000; ep++)
            {
                for (int i = 0; i < x.Length; i++)
                {
                    for (int j = 0; j < w.Length; j++)
                    {
                        var grad = x[i][j] * (f(x[i]) - y[i]) * dfdx(x[i]);
                        if(x[i][j] != 0)
                        w[j] -= 0.1 * grad / ( dfdx(x[i]) * dfdx(x[i]) * x[i][j] + (f(x[i]) - y[i]) * ddfddx(x[i]) * x[i][j] );
                    }
                }
            }
            
            for (int ep = 0; ep < 100000; ep++)
            {
                for (int i = 0; i < x.Length; i++)
                {
                    var Err = E(x[i], y[i]);
                    var dErr1 = dEdw(x[i], y[i], 0);
                    var dErr2 = dEdw(x[i], y[i], 1);

                    if (x[i][0] != 0)
                        w[0] -= 0.1 * Err / dErr1;

                    if (x[i][1] != 0)
                        w[1] -= 0.1 * Err / dErr2;
                }
            }
            
            Vector result = new Vector(x.Length);
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = f(x[i]);
            }
            MessageBox.Show(w.ToString());
            MessageBox.Show(result.ToString());
         
            #endregion
            
            //*
            ctr = 0;
            Vector result;
            while(allErr(x, y) > 0.00000001)
            {
                Matrix A = new Matrix(w.Length, w.Length);
                A[0, 0] = dEdw(x[0], y[0], 0) + dEdw(x[2], y[2], 0);
                A[0, 1] = dEdw(x[0], y[0], 1) + dEdw(x[2], y[2], 1);

                A[1, 0] = dEdw(x[1], y[1], 0) + dEdw(x[3], y[3], 0);
                A[1, 1] = dEdw(x[1], y[1], 1) + dEdw(x[3], y[3], 1);

                Vector b = new Vector(w.Length);
                b[0] = dEdw(x[0], y[0], 0) * w[0] + dEdw(x[0], y[0], 1) * w[1] + dEdw(x[2], y[2], 0) * w[0] + dEdw(x[2], y[2], 1) * w[1] -E(x[0], y[0]) - E(x[2], y[2]);
                b[1] = dEdw(x[1], y[1], 0) * w[0] + dEdw(x[1], y[1], 1) * w[1] + dEdw(x[3], y[3], 0) * w[0] + dEdw(x[3], y[3], 1) * w[1] -E(x[1], y[1]) - E(x[3], y[3]); // b[i] -= E(x[i], y[i]);
                try
                {
                    w = matlib.SolvingSystems.SolvingSLAY(A, b);
                }
                catch { break; }
                /*
                result = new Vector(x.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = f(x[i]);
                }
                MessageBox.Show(w.ToString());
              
                //MessageBox.Show(result.ToString());
                    ctr++;
            }
            result = new Vector(x.Length);
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = f(x[i]);
                result[i] = Math.Round(result[i], 3);
            }
            MessageBox.Show(ctr.ToString());
            MessageBox.Show(w.ToString());
            MessageBox.Show(result.ToString());
            //*/
            /*
            for (int ep = 0; ep < 10000; ep++)
            {
                for (int i = 0; i < x.Length; i++)
                {
                    if (dEdw(x[i], y[i], 1) != 0 && dEdw(x[i], y[i], 0) != 0)
                    {
                        w[0] -= 0.1 * (E(x[i], y[i]) + dEdw(x[i], y[i], 1)) / dEdw(x[i], y[i], 0);
                        w[1] -= 0.1 * (E(x[i], y[i]) + dEdw(x[i], y[i], 0)) / dEdw(x[i], y[i], 1);
                    }
                }
            }
            Vector result = new Vector(x.Length);
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = f(x[i]);
            }
            MessageBox.Show(w.ToString());
            MessageBox.Show(result.ToString());

            //*/
            #endregion
        }
    }
}
