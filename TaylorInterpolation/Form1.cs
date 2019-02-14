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
using MatLib.Interpolation;
namespace TaylorInterpolation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            var x = new Vector(30);
            var y = new Vector(x.Length);

            var X = new Vector(10);
            x[0] = 1.0;
            x[1] = 4.0;
            for (int i = 2; i < x.Length; i++)
            {
                x[i] = Math.Pow(i + 1.0, i);
            }
            for (int i = 0; i < X.Length; i++)
            {
                X[i] = i + 1;
            }
            for (int i = 0; i < x.Length; i++)
            {
                y[i] = matlib.Function.Func.f("x^0.5", x[i]);
            }
            for (int i = 1; i < 25; i++)
            {
                var result = InterpolBase.Taylor(x, y, new Vector(new double[]{ 200 }), i);
                richTextBox1.Text += "" + Math.Abs(Math.Sqrt(200.0) - result[0]) + "\n";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }
    }
}
