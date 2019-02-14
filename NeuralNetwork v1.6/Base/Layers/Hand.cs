using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MatLib;

namespace NeuralNetwork.Base.Layers
{
    class Hand : ILayer
    {
                public Tensor4 input { get; set; }

        public Tensor4 output { get; set; }

        public Tensor4 weights { get; set; }

        public Tensor4 drop { get; set; }

        public Tensor4 delts { get; set; }

        public Tensor4 grads { get; set; }
        public ILayer lastLayer { get; set; }
        public ILayer nextLayer { get; set; }
        public Hand(ILayer lastLayer)
        {
            this.lastLayer = lastLayer;
            int width = lastLayer.output.width;
            int height = lastLayer.output.height;
            int deep = lastLayer.output.deep;
            int bs = lastLayer.output.bs;

            input = new Tensor4(width, height, deep, bs);
            if (input.height * input.deep != 1) throw new Exception();
            output = new Tensor4(width + 1, height, deep, bs);
        }
        public Tensor4 CalcOutp(Tensor4 inp)
        {
            this.input = inp;
            output = new Tensor4(inp.width + 1, inp.height, inp.deep, inp.bs);

            for (int d = 0; d < inp.bs; d++)
                for (int x = 0; x < inp.width; x++)
                    output[d, 0, 0, x] = inp[d, 0, 0, x];

            //for (int d = 0; d < inp.bs; d++)
            //            output[d, 0, 0, output.width - 1] = 1.0;

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
            return "Polarization";
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
            delts = (output - y);
            return delts;
        }


        public Tensor4 Backward()
        {
            Tensor4 lastDelts = new Tensor4(input.width, 1, 1, delts.bs);
            for (int d = 0; d < lastDelts.bs; d++)
                for (int x = 0; x < lastDelts.width; x++)
                    lastDelts[d, 0, 0, x] = delts[d, 0, 0, x];
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
