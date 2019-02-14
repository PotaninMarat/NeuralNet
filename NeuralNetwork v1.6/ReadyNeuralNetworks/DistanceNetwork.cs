using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MatLib;
namespace NeuralNetwork.ReadyNeuralNetworks
{
    public class DistanceNetwork
    {
        public Tensor4 input;
        public Vector output;
        public NeuralNet net;
        public DistanceNetwork(int width, int height, int deep, int bs, int classNum)
        {
            input = new Tensor4(width, height, deep, bs);
            output = new Vector(classNum);
            net = new NeuralNet(new string[]{
                "input:" + width + ":" + height + ":" + deep + ":" + bs,
                "convDistance:" + classNum
            });
        }
        int CalculationBase(Tensor4 input)
        {
            this.input = input;
            return net.Calculation(input)[0];
        }
        public int Calculation(Tensor4 input)
        {
            return CalculationBase(input);
        }
        public int Calculation(Matrix input)
        {
            return CalculationBase(input.ToTensor4());
        }
        public int Calculation(Tensor3 input)
        {
            return CalculationBase(input.ToTensor4());
        }
        public void Train(Matrix x, int y, double norm = 0.01, double moment = 0.0)
        {
            net.TrainWithTeach(x.ToTensor4(), new int[] { y }, norm, moment);
        }
    }
}
