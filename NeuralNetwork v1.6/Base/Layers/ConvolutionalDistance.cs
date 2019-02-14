using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MatLib;

namespace NeuralNetwork.Base.Layers
{
    public class ConvolutionalDistance : ILayer
    {
        public Tensor4 input { get; set; }

        public Tensor4 output { get; set; }

        public Tensor4 weights { get; set; }

        public Tensor4 drop { get; set; }

        public Tensor4 delts { get; set; }

        public Tensor4 grads { get; set; }
        public ILayer lastLayer { get; set; }
        public ILayer nextLayer { get; set; }
        public Tensor4 lastWeightsDelts;
        public ConvolutionalDistance(ILayer lastLayer, int neurNum)
        {
            this.lastLayer = lastLayer;
            int widthInp = lastLayer.output.width;
            int heightInp = lastLayer.output.height;
            int deepInp = lastLayer.output.deep;
            int bsInp = lastLayer.output.bs;

            input = new Tensor4(widthInp, heightInp, deepInp, bsInp);
            weights = new Tensor4(widthInp, heightInp, deepInp, neurNum);
            drop = new Tensor4(widthInp, heightInp, deepInp, neurNum) + 1.0;
            lastWeightsDelts = new Tensor4(widthInp, heightInp, deepInp, neurNum);
            output = new Tensor4(neurNum, 1, 1, bsInp);
        }
        public Tensor4 CalcOutp(Tensor4 inp)
        {
            this.input = inp.NormalizeAbs();
            
            output = 1.0 - input.DistanceSquare(weights);

            return output;
        }
        public override string ToString()
        {
            return "ConvolutionalDistance";
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
            double w = -2.0 / (double)input.dhw;
            grads = new Tensor4(weights.width, weights.height, weights.deep, weights.bs);
                for (int s = 0; s < grads.bs; s++ )
                    for (int i = 0; i < grads.deep; i++)
                        for (int j = 0; j < grads.height; j++)
                            for (int k = 0; k < grads.width; k++)
                            {
                                for (int d = 0; d < input.bs; d++)
                                grads[s, i, j, k] += w * (weights[s, i, j, k] - input[d, i, j, k]) * delts[d, 0, 0, s];

                                grads[s, i, j, k] /= (double)input.bs;
                            }

            return grads;
        }

        public Tensor4 Train(double norm, double moment)
        {
            lastWeightsDelts = norm * grads + lastWeightsDelts * moment;
            weights -= lastWeightsDelts * drop;
            return null;
        }

        public void RandomWeights()
        {
            weights.Random();
            weights *= 0.2;//2.0 / Math.Sqrt((double)input.width * input.height * input.deep);
        }

        public void SetWeights(Tensor4 weights)
        {
            this.weights = weights.Copy();
        }

        public Tensor4 CalcDelts(Tensor4 y, ILayer lastLayer)
        {
            if (y.height != 1 || y.deep != 1) throw new Exception("Чет браток не то у тебя.");

            this.lastLayer = lastLayer;
            /*
            Tensor4 Y = new Tensor4(y.width, y.height, y.deep, y.bs) + 1.0;
            for (int i = 0; i < y.bs; i++ )
                for (int j = 0; j < y.width; j++)
                    if (y[i, 0, 0, j] == 1) { Y[i, 0, 0, j] = 0.0; break; }
            //*/
            delts = output - y;

            return delts;
        }


        public Tensor4 Backward()
        {
            //*
            double w = -1.0/(double)input.dhw;// 2.0 / (double)output.width;
            Tensor4 lastDelts = new Tensor4(input.width, input.height, input.deep, input.bs);
            for (int d = 0; d < lastDelts.bs; d++)
                for (int s = 0; s < weights.bs; s++)
                    for (int z = 0; z < lastDelts.deep; z++)
                        for (int y = 0; y < lastDelts.height; y++)
                            for (int x = 0; x < lastDelts.width; x++)
                                lastDelts[d, z, y, x] += w * (input[d, z, y, x] - weights[s, z, y, x]);
                                 
            
            return lastDelts;
            //*/
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
