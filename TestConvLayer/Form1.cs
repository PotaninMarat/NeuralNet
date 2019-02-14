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
namespace TestConvLayer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Random rand = new Random();
        NeuralNet net;
        Tensor4 inp;
        int inpNum = 250;
        Tensor4[] xTest;
        int[] yTest;
        private void Form1_Load(object sender, EventArgs e)
        {
            inp = new Tensor4(inpNum, 1, 1, 1);
            net = new NeuralNet(new string[]{
                "input:" + inpNum + ":1:1:1",

                "conv:8:1:3",
                "prelu",
                "pool:3:1",

                "conv:7:1:4",
                "prelu",
                "pool:2:1",

                "conv:6:1:5",
                "prelu",
                "pool:2:1",

                "conv:5:1:6",
                "prelu",
                "pool:2:1",


                "softmax:3"
                //"conv:4:1:3"
            });
            MessageBox.Show(net.GetInfoAboutOutputs());

            xTest = new Tensor4[5000];
            yTest = new int[xTest.Length];
            for(int i = 0; i < xTest.Length; i++)
            {
                switch ((int)(rand.NextDouble() * 3.0 + 1))
                {
                    case 1:
                        yTest[i] = 0;
                        xTest[i] = sin();
                        break;
                    case 2:
                        yTest[i] = 1;
                        xTest[i] = rect();
                        break;
                    case 3:
                        yTest[i] = 2;
                        xTest[i] = noise();
                        break;
                    default: throw new Exception();
                }
            }
        }

        private void buttonSin_Click(object sender, EventArgs e)
        {
            net.Calculation(sin());
            matlib.ZedGraph.DrawZedGraphBar(zedGraphControl2, net.output.ToVector(0).elements, 1, 3, Color.Green);
            matlib.ZedGraph.DrawZedGraphBar(zedGraphControl1, net.layers[0].output.ToVector(0).elements, 1, 3, Color.Green);
        }
        Tensor4 sin()
        {
            double[] val = matlib.Function.Func.Sin(inpNum, rand.NextDouble() * 20, rand.NextDouble() * 4, 1).elements;
            Tensor4 res = new Tensor4(val.Length, 1, 1, 1);
            res.elements = val;
            return res;
        }
        Tensor4 rect()
        {
            double[] val = matlib.Function.Func.Rect(inpNum, rand.NextDouble() * 20, rand.NextDouble() * 4, 1).elements;
            Tensor4 res = new Tensor4(val.Length, 1, 1, 1);
            res.elements = val;
            return res;
        }
        Tensor4 noise()
        {
            Tensor4 res = new Tensor4(inpNum, 1, 1, 1);
            res.Random();
            return res;
        }
        private void buttonTrain_Click(object sender, EventArgs e)
        {
            double norm = 0.002;//20e-3; баг!!!
            var trainer = NeuralNet.Optimizer.Adam;
            for(int epoch = 1; epoch <= 1; epoch++)
                for(int iter = 0; iter < 1000; iter++)
                {
                    Tensor4 y = new Tensor4(3, 1, 1, 1);
                    switch((int)(rand.NextDouble() * 3.0 + 1))
                    {
                        case 1:
                            y[0, 0, 0, 0] = 1;
                            net.TrainWithTeach(sin(), y, norm, 0.0, 0.0001, trainer);
                            break;
                        case 2:
                            y[0, 0, 0, 1] = 1;
                            net.TrainWithTeach(rect(), y, norm, 0.0, 0.0001, trainer);
                            break;
                        case 3:
                            y[0, 0, 0, 2] = 1;
                            net.TrainWithTeach(noise(), y, norm, 0.0, 0.0001, trainer);
                            break;
                        default: throw new Exception();
                    }
                }
        }

        private void buttonRect_Click(object sender, EventArgs e)
        {
            net.Calculation(rect());
            matlib.ZedGraph.DrawZedGraphBar(zedGraphControl2, net.output.ToVector(0).elements, 1, 3, Color.Green);
            matlib.ZedGraph.DrawZedGraphBar(zedGraphControl1, net.layers[0].output.ToVector(0).elements, 1, 3, Color.Green);
        }

        private void buttonNoise_Click(object sender, EventArgs e)
        {
            net.Calculation(noise());
            matlib.ZedGraph.DrawZedGraphBar(zedGraphControl2, net.output.ToVector(0).elements, 1, 3, Color.Green);
            matlib.ZedGraph.DrawZedGraphBar(zedGraphControl1, net.layers[0].output.ToVector(0).elements, 1, 3, Color.Green);
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            int numCorrectAnsw = 0;
            for(int i = 0; i < xTest.Length; i++)
                if (net.Calculation(xTest[i], 0, 0)[0] == yTest[i]) numCorrectAnsw++;
            MessageBox.Show("Точность: " + (100.0 * (double)numCorrectAnsw / (double)xTest.Length) + "%");
            
        }
    }
}
