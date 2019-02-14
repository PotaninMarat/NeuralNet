using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NeuralNetwork;
using MatLib;
namespace TestDirectLayer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Tensor4 inp;
        NeuralNet net;
        double norm = 0.05, moment = 0.9;
        List<double> eps = new List<double>();
        private void textBoxX1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                inp[0, 0, 0, 0] = double.Parse(textBoxX1.Text);
                net.Calculation(inp);
                labelResult.Text = "" + Math.Round(net.output[0, 0, 0, 0], 6);
            }
            catch { }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            inp = new Tensor4(2, 1, 1, 1);
            net = new NeuralNet(new string[]{
                "input:" + inp.width + ":" + inp.height + ":" + inp.deep + ":" + inp.bs,

                //"conv:3:1:2",
                //"relu",
                //"direct:1",
                //"sin",
                //"kernel",
                "direct:1",
                //"sin"
            });
        }

        private void textBoxX2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                inp[0, 0, 0, 1] = double.Parse(textBoxX2.Text);
                net.Calculation(inp);
                labelResult.Text = "" + Math.Round(net.output[0, 0, 0, 0], 6);
            }
            catch { }
        }

        private void textBoxNorm_TextChanged(object sender, EventArgs e)
        {
            try
            {
                norm = double.Parse(textBoxNorm.Text);
            }
            catch { }
        }

        private void textBoxMoment_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonTrain_Click(object sender, EventArgs e)
        {
            Tensor4[] x = new Tensor4[4];
            Tensor4[] y = new Tensor4[x.Length];
            for(int i = 0; i < x.Length; i++)
            {
                x[i] = new Tensor4(inp.width, inp.height, inp.deep, 1);
                y[i] = new Tensor4(1, 1, 1, 1);
            }
            //*
            //x[0][0, 0, 0, 0] = 5; x[0][0, 0, 0, 1] = 8; y[0][0, 0, 0, 0] = 13;
            //x[0][1, 0, 0, 0] = 1; x[0][1, 0, 0, 1] = 1; y[0][1, 0, 0, 0] = 0;

            //x[1][0, 0, 0, 0] = 3; x[1][0, 0, 0, 1] = -1; y[1][0, 0, 0, 0] = 2;
            //x[1][1, 0, 0, 0] = 1; x[1][1, 0, 0, 1] = 1; y[1][1, 0, 0, 0] = 0;

            //x[2][0, 0, 0, 0] = 12; x[2][0, 0, 0, 1] = -11; y[2][0, 0, 0, 0] = 1;
            //x[2][1, 0, 0, 0] = 1; x[2][1, 0, 0, 1] = 1; y[2][1, 0, 0, 0] = 0;

            x[0][0, 0, 0, 0] = 0; x[0][0, 0, 0, 1] = 1; y[0][0, 0, 0, 0] = 1;
            x[1][0, 0, 0, 0] = 0; x[1][0, 0, 0, 1] = 0; y[1][0, 0, 0, 0] = 0;

            x[2][0, 0, 0, 0] = 1; x[2][0, 0, 0, 1] = 0; y[2][0, 0, 0, 0] = 1;
            x[3][0, 0, 0, 0] = 1; x[3][0, 0, 0, 1] = 1; y[3][0, 0, 0, 0] = 0;

            //x[5][0, 0, 0, 0] = 0; x[5][0, 0, 0, 1] = 1; y[5][0, 0, 0, 0] = 1;
            //x[5][1, 0, 0, 0] = 1; x[5][1, 0, 0, 1] = 0; y[5][1, 0, 0, 0] = 1;
            //*/
            /*
            int ctr1 = 0;
            for(int a = 0; a < 2; a++)
                for(int b = 0; b < 2; b++)
                {
                    if (ctr1 == x.Length) { a = 2; break; }
                    x[ctr1][0, 0, 0, 0] = a;
                    x[ctr1][0, 0, 0, 1] = b;
                    y[ctr1++][0, 0, 0, 0] = a ^ b;
                }
            //*/
            eps.Add(calcErr(x, y));
            for (int epoch = 1; epoch <= 10; epoch++)
            {
                for (int iter = 0; iter < x.Length; iter++)
                    net.TrainWithTeach(x[iter], y[iter], norm, moment, 0.0001);
                eps.Add(Math.Round(calcErr(x, y), 4));
            }
            matlib.ZedGraph.DrawZedGraphCurve(zedGraphControl1, eps.ToArray(), 1, eps.Count, Color.Green);
            //MessageBox.Show("" + (eps[0] - eps[eps.Count - 1]));
        }
        double calcErr(Tensor4[] x, Tensor4[] y)
        {
            double err = 0.0;
            for (int i = 0; i < x.Length; i++)
            {
                net.Calculation(x[i]);
                err += (Math.Pow(y[i][0, 0, 0, 0] - net.output[0, 0, 0, 0], 2.0));
            }
            err /= x.Length;
            return err;
        }
    }
}
