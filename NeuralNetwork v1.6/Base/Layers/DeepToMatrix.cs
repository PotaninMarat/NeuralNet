using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MatLib;

namespace NeuralNetwork.Base.Layers
{
    class DeepToMatrix : ILayer
    {
        public Tensor4 input { get; set; }

        public Tensor4 output { get; set; }

        public Tensor4 weights { get; set; }

        public Tensor4 drop { get; set; }

        public Tensor4 delts { get; set; }

        public Tensor4 grads { get; set; }
        public ILayer lastLayer { get; set; }
        public ILayer nextLayer { get; set; }
        public DeepToMatrix(ILayer lastLayer)
        {
            this.lastLayer = lastLayer;
            int width = lastLayer.output.width;
            int height = lastLayer.output.height;
            int deep = lastLayer.output.deep;
            int bs = lastLayer.output.bs;

            if (height != 1 && width != 1) throw new Exception("Ошибка.");
            input = new Tensor4(width, height, deep, bs);
            if(width == 1)
                output = new Tensor4(deep, height, 1, bs);
            else output = new Tensor4(width, deep, 1, bs);
        }
        public Tensor4 CalcOutp(Tensor4 inp)
        {
            this.input = inp;
            if (input.width == 1)
                output = new Tensor4(input.deep, input.height, 1, input.bs);
            else output = new Tensor4(input.width, input.deep, 1, input.bs);
            if(input.width == 1)
            {
                for (int d = 0; d < output.bs; d++)
                    for (int y = 0; y < output.height; y++)
                        for (int x = 0; x < output.width; x++)
                            output[d, 0, y, x] = input[d, x, y, 0];
            }
            else
            {
                for (int d = 0; d < output.bs; d++)
                    for (int y = 0; y < output.height; y++)
                        for (int x = 0; x < output.width; x++)
                            output[d, 0, y, x] = input[d, y, x, 0];
            }
            return output;
        }
        public override string ToString()
        {
            return "DeepToMatrix";
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
           // if (input.width == 1)
           //     output = new Tensor4(input.deep, input.height, 1, input.bs);
           // else output = new Tensor4(input.width, input.deep, 1, input.bs);
            if (input.width == 1)
            {
                for (int d = 0; d < input.bs; d++)
                    for (int z = 0; z < input.deep; z++)
                        for (int y = 0; y < input.height; y++)
                            lastDelts[d, z, y, 0] = delts[d, 0, y, z];
            }
            else
            {
                for (int d = 0; d < input.bs; d++)
                    for (int z = 0; z < input.deep; z++)
                            for (int x = 0; x < input.width; x++)
                                lastDelts[d, z, 0, x] = delts[d, 0, z, x];
            }
            /*
            int ctr = 0;
            for (int d = 0; d < input.bs; d++)
                for (int z = 0; z < input.deep; z++)
                    for (int y = 0; y < input.height; y++)
                        for (int x = 0; x < input.width; x++)
                             lastDelts[d, z, y, x] = delts.elements[ctr++];
            //*/
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
