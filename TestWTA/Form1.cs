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
namespace TestWTA
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        WTANetwork wta;
        private void Form1_Load(object sender, EventArgs e)
        {
            int size = 1;
            wta = new WTANetwork(size, 2);
            Vector[] data = new Vector[100];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = new Vector(size);
                data[i].GaussRandom();
            }

            {
                string r1 = "", r2 = "";
                for (int i = 0; i < data.Length; i++)
                {
                    if (wta.Run(data[i]) == 0)
                        r1 += data[i] + "\n";
                    else r2 += data[i] + "\n";
                }
                richTextBox1.Text = r1;
                richTextBox2.Text = r2;
            }

            for (int ep = 1; ep <= 10; ep++)
                for (int i = 0; i < data.Length; i++)
                    wta.Train(data[i]);

            {
                string r1 = "", r2 = "";
                for (int i = 0; i < data.Length; i++)
                {
                    if (wta.Run(data[i]) == 0)
                        r1 += data[i] + "\n";
                    else r2 += data[i] + "\n";
                }
                richTextBox3.Text = r1;
                richTextBox4.Text = r2;
            }
        }
    }
}
