using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatLib;
namespace NeuralNetwork.Base.Layers
{
    public class Input : ILayer
    {
        public Tensor4 input { get; set; }

        public Tensor4 output { get; set; }

        public Tensor4 weights { get; set; }

        public Tensor4 drop { get; set; }

        public Tensor4 delts { get; set; }

        public Tensor4 grads { get; set; }
        public Input(int width, int height, int deep, int bs)
        {
            input = new Tensor4(width, height, deep, bs);
            output = input;
        }
        public Tensor4 CalcOutp(Tensor4 inp)
        {
            this.input = inp;
            output = inp;
            return output;
        }

        public Tensor4 CalcDelts(ILayer lastLayer, ILayer nextLayer)
        {
            throw new NotImplementedException();
        }

        public Tensor4 CalcGrads(double lambda)
        {
            return null;
            //throw new NotImplementedException();
        }

        public Tensor4 Train(double norm, double moment)
        {
            return null;
            //throw new NotImplementedException();
        }

        public void RandomWeights()
        {
            //throw new NotImplementedException();
        }
        public void SetWeights(Tensor4 weights)
        {
            //throw new NotImplementedException();
        }
        public override string ToString()
        {
            return "Input";
        }

        public Tensor4 CalcDelts(Tensor4 y, ILayer lastLayer)
        {
            throw new NotImplementedException();
        }

        public ILayer nextLayer
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Tensor4 Backward()
        {
            return null;
            //throw new NotImplementedException();
        }


        public ILayer lastLayer
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public Vector[] WeightsNeurons()
        {
            return null;
        }
        public void SetWeightsNeurons(Tensor3[] weights)
        {

        }
    }
}
