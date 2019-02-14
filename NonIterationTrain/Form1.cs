using MatLib;
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
namespace NonIterationTrain
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            NeuralNet net = new NeuralNet(new string[]{
                
                "input:3:1:1:1",

                "direct:1"
            
            });
            Tensor4 x = new Tensor4(3, 1, 1, 4);
            Tensor4 y = new Tensor4(1, 1, 1, 4);
            int c = 0;
            for (int a = 0; a < 2; a++)
            {
                for (int b = 0; b < 2; b++)
                {
                    x[c, 0, 0, 0] = a - 10;
                    x[c, 0, 0, 2] = a*b;
                    x[c, 0, 0, 1] = b + 10;
                    y[c++, 0, 0, 0] = a ^ b;
                }
            }
            net.TrainWithTeach(x, y, 0, 0, 0, NeuralNet.Optimizer.NonIteratorLinear);
            MessageBox.Show("" + net.CalcErrRootMSE(x, y));
        }
    }
}
