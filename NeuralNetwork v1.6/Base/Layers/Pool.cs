using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatLib;
namespace NeuralNetwork.Base.Layers
{
    class Pool : ILayer
    {
       public Tensor4 input { get; set; }

        public Tensor4 output { get; set; }

        public Tensor4 weights { get; set; }

        public Tensor4 drop { get; set; }

        public Tensor4 delts { get; set; }

        public Tensor4 grads { get; set; }
        public ILayer lastLayer { get; set; }
        public ILayer nextLayer { get; set; }
        int filtW, filtH;
        public Pool(ILayer lastLayer, int filtW, int filtH)
        {
            this.lastLayer = lastLayer;
            int width = lastLayer.output.width;
            int height = lastLayer.output.height;
            int deep = lastLayer.output.deep;
            int bs = lastLayer.output.bs;

            this.filtW = filtW;
            this.filtH = filtH;
            input = new Tensor4(width, height, deep, bs);
            output = new Tensor4(width / filtW + ((width % filtW == 0) ? 0 : 1), height / filtH + ((height % filtH == 0) ? 0 : 1), deep, bs);
        }
        public override string ToString()
        {
            return "Pool";
        }
        public Tensor4 CalcOutp(Tensor4 inp)
        {
            this.input = inp;
            output = new Tensor4(input.width / filtW + ((input.width % filtW == 0) ? 0 : 1), input.height / filtH + ((input.height % filtH == 0) ? 0 : 1), input.deep, input.bs);
           // output = new Tensor4(output.width, output.height, output.deep, input.bs);
            for (int d = 0; d < inp.bs; d++)
                for (int z = 0; z < inp.deep; z++)
                    for (int y = 0; y < inp.height; y += filtH)
                        for (int x = 0; x < inp.width; x += filtW)
                        {
                            double max = input[d, z, y, x];
                            for (int dy = 0; dy < filtH && (y + dy) < input.height; dy++)
                                for (int dx = 0; dx < filtW && (x + dx) < input.width; dx++)
                                    if (max < input[d, z, y + dy, x + dx])
                                        max = input[d, z, y + dy, x + dx];
                            output[d, z, y / filtH, x / filtW] = max;
                        }
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
            for (int d = 0; d < input.bs; d++)
                for (int z = 0; z < input.deep; z++)
                    for (int y = 0; y < input.height; y += filtH)
                        for (int x = 0; x < input.width; x += filtW)
                        {
                            int Xmax = x, Ymax = y;
                            for (int dy = 0; dy < filtH && (y + dy) < input.height; dy++)
                                for (int dx = 0; dx < filtW && (x + dx) < input.width; dx++)
                                    if (input[d, z, Ymax, Xmax] < input[d, z, y + dy, x + dx])
                                    {
                                        Ymax = y + dy;
                                        Xmax = x + dx;
                                    }
                            lastDelts[d, z, Ymax, Xmax] = delts[d, z, y / filtH, x / filtW];
                        }
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
