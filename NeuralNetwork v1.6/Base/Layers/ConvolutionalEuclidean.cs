using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatLib;
namespace NeuralNetwork.Base.Layers
{
    class ConvolutionalEuclidean : ILayer
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
        public bool same;
        public ConvolutionalEuclidean(ILayer lastLayer, int widthFilt, int heightFilt, int neurNum, bool same)
        {
            this.lastLayer = lastLayer;
            int widthInp = lastLayer.output.width;
            int heightInp = lastLayer.output.height;
            int deepInp = lastLayer.output.deep;
            int bsInp = lastLayer.output.bs;

            this.same = same;
            input = new Tensor4(widthInp + Convert.ToInt32(same) * (widthFilt - 1), heightInp + Convert.ToInt32(same) * (heightFilt - 1), deepInp, bsInp);
            weights = new Tensor4(widthFilt, heightFilt, deepInp, neurNum);
            lastWeightsDelts = new Tensor4(widthFilt, heightFilt, deepInp, neurNum);
            output = new Tensor4(input.width - weights.width + 1, input.height - weights.height + 1, neurNum, bsInp);
        }
        public override string ToString()
        {
            return "ConvolutionalEuclidean";
        }
        public Tensor4 CalcOutp(Tensor4 inp)
        {
            if(!same)
            this.input = inp;
            else
            {
               input = new Tensor4(input.width, input.height, input.deep, inp.bs);
               int a = weights.height - 1;
               int b = weights.width - 1;
               for (int d = 0; d < inp.bs; d++)
                   for (int z = 0; z < input.deep; z++)
                       for (int y = weights.height - 1; y < input.height; y++)
                           for (int x = weights.width - 1; x < input.width; x++)
                               input[d, z, y, x] = inp[d, z, y - a, x - b];
            }
            output = input.ConvEuclid(weights);

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
            grads = new Tensor4(weights.width, weights.height, weights.deep, weights.bs);
            double l = Math.Pow((double)delts.width * delts.height, 0.3);

            Parallel.For(0, weights.bs, d =>
            {//for (int d = 0; d < weights.bs; d++)
                for (int z = 0; z < weights.deep; z++)//Parallel.For(0, weights[i].deep, z =>
                    for (int yw = 0; yw < weights.height; yw++)
                        for (int xw = 0; xw < weights.width; xw++)
                        {
                            for (int b = 0; b < input.bs; b++)
                                for (int y = 0; y < delts.height; y++)
                                    for (int x = 0; x < delts.width; x++)
                                        grads[d, z, yw, xw] += delts[b, d, y, x] * 2.0 * (weights[d, z, yw, xw] - input[b, z, y + yw, x + xw]);

                            grads[d, z, yw, xw] /= (double)weights.hw;
                            grads[d, z, yw, xw] /= (double)input.bs;

                            grads[d, z, yw, xw] /= l;
                            grads[d, z, yw, xw] += lambda * weights[d, z, yw, xw];
                        }
            });
            return grads;
        }

        public Tensor4 Train(double norm, double moment)
        {
            lastWeightsDelts = norm * grads + lastWeightsDelts * moment;
            weights -= lastWeightsDelts;
            return null;
        }

        public void RandomWeights()
        {
            weights.Random();
            weights *= 0.15;//2.0 / Math.Sqrt((double)input.width * input.height * input.deep);
        }

        public void SetWeights(Tensor4 weights)
        {
            this.weights = weights.Copy();
        }


        public Tensor4 CalcDelts(Tensor4 y, ILayer lastLayer)
        {
            this.lastLayer = lastLayer;
            delts = output - y;
            return delts;
        }
        public Tensor4 Backward()
        {
            Tensor4 derivs = new Tensor4(lastLayer.output.width, lastLayer.output.height, lastLayer.output.deep, lastLayer.output.bs);
            var yStart = Convert.ToInt32(same) * (weights.height - 1);
            var xStart = Convert.ToInt32(same) * (weights.width - 1);

            Parallel.For(0, output.bs, d =>
            {//for (int d = 0; d < output.bs; d++ )
                for (int n = 0; n < derivs.deep; n++)
                {
                    for (int y = yStart; y < derivs.height + yStart; y++)//Parallel.For(0, delts.height, y =>
                        for (int x = xStart; x < derivs.width + xStart; x++)
                        {
                            for (int i = 0; i < delts.deep; i++)
                                for (int dy = 0; dy < weights.height; dy++)
                                    for (int dx = 0; dx < weights.width; dx++)
                                        if (((y - dy) >= 0) && ((x - dx) >= 0) && ((y - dy) < delts.height) && ((x - dx) < delts.width))
                                            derivs[d, n, y - yStart, x - xStart] += 2.0 * delts[d, i, y - dy, x - dx] * (input[d, n, dy, dx] - weights[i, n, dy, dx]);

                            derivs[d, n, y - yStart, x - xStart] /= weights.hw;
                        }
                }
                //);
            });
            return derivs;
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
