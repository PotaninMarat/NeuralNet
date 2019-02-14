using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuralNetwork;
using MatLib;
namespace NeuralNetwork.ReadyNeuralNetworks
{
    public class DirectNetwork
    {
        NeuralNet net;
        public DirectNetwork(int inpNum, int classNum)
        {
            net = new NeuralNet(new string[] {
                "input:" + inpNum + ":1:1:1",
                "direct:" + (inpNum / 2 + 1),
                "direct:" + classNum
            });
        }
        public int Calculation(Vector input)
        {
            return net.Calculation(input.ToTensor4())[0];
        }
        public void Save(string path)
        {
            net.SaveWeights(path);
        }
        public void Load(string path)
        {
            net.LoadWeights(path);
        }

    }
}
