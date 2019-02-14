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
namespace TestKernel
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Bitmap pictX, pictY;
        NeuralNet net;
        private void Form1_Load(object sender, EventArgs e)
        {
            pictX = new Bitmap(@"C:\Users\Marat\Documents\Visual Studio 2013\Projects\MonochImg_in_colorfulImg\MonochImg_in_colorfulImg\bin\Debug\train sample\x3.png");
            pictY = new Bitmap(@"C:\Users\Marat\Documents\Visual Studio 2013\Projects\MonochImg_in_colorfulImg\MonochImg_in_colorfulImg\bin\Debug\train sample\y3.png");
            pictX = new Bitmap(pictX, 20, 20);
            pictureBox1.Image = pictX;
            net = new NeuralNet(new string[] {
                "input:" + pictX.Width + ":" + pictX.Height + ":1:1",

                "kernel",
                "direct:225",
                "vectorToTensor3:15:15:1",
                //"gauss:-0,005"
            });
            pictY = new Bitmap(pictY, net.output.width, net.output.height);
        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            net.Calculation(matlib.Convertor.Picture.BitmapInMatrix(pictX).ToTensor4() / 255.0);
            pictureBox2.Image = matlib.Convertor.Picture.MatrixInBitmap(net.output.ToMatrix(0, 0) * 255.0);
        }

        private void buttonTrain_Click(object sender, EventArgs e)
        {
            Tensor4 x = matlib.Convertor.Picture.BitmapInMatrix(pictX).ToTensor4() / 255.0;
            Tensor4 y = matlib.Convertor.Picture.BitmapInMatrix(pictY).ToTensor4() / 255.0;
            //pictureBox2.Image = matlib.Convertor.MatrixInBitmap(y.ToMatrix(0, 0) * 255);
            for (int i = 0; i < 200; i++)
                net.TrainWithTeach(x, y, 0.002, 0, 0.0001, NeuralNet.Optimizer.Adam);

            MessageBox.Show("Error: " + net.CalcErrRootMSE(new Tensor4[] { x }, new Tensor4[] { y }));
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pictX = new Bitmap(openFileDialog1.FileName);
                pictX = new Bitmap(pictX, net.layers[0].input.width, net.layers[0].input.height);
                pictureBox1.Image = pictX;
            }
        }
    }
}
