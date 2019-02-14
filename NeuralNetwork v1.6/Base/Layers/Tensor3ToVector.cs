using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatLib;
namespace NeuralNetwork.Base.Layers
{
    class Tensor3ToVector : ILayer
    {
        public Tensor4 input { get; set; }

        public Tensor4 output { get; set; }

        public Tensor4 weights { get; set; }

        public Tensor4 drop { get; set; }

        public Tensor4 delts { get; set; }

        public Tensor4 grads { get; set; }
        public ILayer lastLayer { get; set; }
        public ILayer nextLayer { get; set; }
        public Tensor3ToVector(ILayer lastLayer)
        {
            this.lastLayer = lastLayer;
            int width = lastLayer.output.width;
            int height = lastLayer.output.height;
            int deep = lastLayer.output.deep;
            int bs = lastLayer.output.bs;

            input = new Tensor4(width, height, deep, bs);
            output = new Tensor4(width * height * deep, 1, 1, bs);
        }
        public Tensor4 CalcOutp(Tensor4 inp)
        {
            this.input = inp;
            int mul = output.height * output.width;
            for (int d = 0; d < output.bs; d++)
                for (int z = 0; z < output.deep; z++)
                    for (int y = 0; y < output.height; y++)
                        for (int x = 0; x < output.width; x++)
                            output[d, 0, 0, z * mul + y * output.width + x] = input[d, z, y, x];
            return output;
        }

        public Tensor4 CalcDelts(ILayer lastLayer, ILayer nextLayer)
        {
            this.lastLayer = lastLayer;
            this.nextLayer = nextLayer;
            delts = nextLayer.Backward();
            return delts;
        }

        public Tensor4 CalcGrads(double lambda)
        {
            return null;
        }
        public override string ToString()
        {
            return "Tensor3ToVector";
        }
        public Tensor4 Train(double norm, double moment)
        {
            return null;
        }

        public void RandomWeights()
        {
        }

        public void SetWeights(Tensor4 weights)
        {
            //throw new NotImplementedException();
        }


        public Tensor4 CalcDelts(Tensor4 y, ILayer lastLayer)
        {
            this.lastLayer = lastLayer;
            delts = output - y;
            return delts;
        }


        public Tensor4 Backward()
        {
            Tensor4 lastDelts = new Tensor4(input.width, input.height, input.deep, input.bs);
            int mul = input.height * input.width;
            for (int d = 0; d < input.bs; d++)
                for (int z = 0; z < input.deep; z++)
                    for (int y = 0; y < input.height; y++)
                        for (int x = 0; x < input.width; x++)
                            lastDelts[d, z, y, x] = delts[d, 0, 0, z * mul + y * input.width + x];
            return lastDelts;
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
