using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MatLib;

namespace NeuralNetwork.Base.Layers
{
    class Unpool1 : ILayer
    {
                public Tensor4 input { get; set; }

        public Tensor4 output { get; set; }

        public Tensor4 weights { get; set; }

        public Tensor4 drop { get; set; }

        public Tensor4 delts { get; set; }

        public Tensor4 grads { get; set; }
        public ILayer lastLayer { get; set; }
        public ILayer nextLayer { get; set; }
        public Unpool1(ILayer lastLayer)
        {
            this.lastLayer = lastLayer;
            int width = lastLayer.output.width;
            int height = lastLayer.output.height;
            int deep = lastLayer.output.deep;
            int bs = lastLayer.output.bs;

            input = new Tensor4(width, height, deep, bs);
            output = new Tensor4(2 * width, 2 * height, deep, bs);
        }
        public Tensor4 CalcOutp(Tensor4 inp)
        {
            this.input = inp;
            output = new Tensor4(output.width, output.height, output.deep, input.bs);
            for (int d = 0; d < output.bs; d++)
                for (int z = 0; z < output.deep; z++)
                    for (int y = 0; y < output.height; y += 2)
                        for (int x = 0; x < output.width; x += 2)
                            output[d, z, y + 1, x + 1] = input[d, z, y / 2, x / 2];
                        
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

        public Tensor4 Train(double norm, double moment)
        {
            return null;
        }
        public override string ToString()
        {
            return "Unpool1";
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
            delts = (output - y);
            return delts;
        }


        public Tensor4 Backward()
        {
            Tensor4 lastDelts = new Tensor4(lastLayer.output.width, lastLayer.output.height, lastLayer.output.deep, lastLayer.output.bs);
            for (int d = 0; d < output.bs; d++)
                for (int z = 0; z < output.deep; z++)
                    for (int y = 0; y < output.height; y += 2)
                        for (int x = 0; x < output.width; x += 2)
                            lastDelts[d, z, y / 2, x / 2] = delts[d, z, y + 1, x + 1];
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
