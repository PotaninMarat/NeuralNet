using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatLib;
namespace NeuralNetwork.Base.Layers
{
    class Convolutional : ILayer
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
        double l;
        int xStart, yStart;
        public Convolutional(ILayer lastLayer, int widthFilt, int heightFilt, int neurNum, bool same)
        {
            this.lastLayer = lastLayer;
            int widthInp = lastLayer.output.width;
            int heightInp = lastLayer.output.height;
            int deepInp = lastLayer.output.deep;
            int bsInp = lastLayer.output.bs;
            this.same = same;

            input = new Tensor4(widthInp + Convert.ToInt32(same) * (widthFilt - 1), heightInp + Convert.ToInt32(same) * (heightFilt - 1), deepInp, bsInp);

            weights = new Tensor4(widthFilt, heightFilt, deepInp, neurNum);
            drop = new Tensor4(widthFilt, heightFilt, deepInp, neurNum) + 1.0;
            lastWeightsDelts = new Tensor4(widthFilt, heightFilt, deepInp, neurNum);
            output = new Tensor4(input.width - weights.width + 1, input.height - weights.height + 1, neurNum, bsInp);

            l = Math.Pow((double)output.hw, 0.2);
            yStart = Convert.ToInt32(same) * (weights.height - 1);
            xStart = Convert.ToInt32(same) * (weights.width - 1);
        }
        public override string ToString()
        {
            return "Convolutional";
        }
        public Tensor4 CalcOutp(Tensor4 inp)
        {
            if(!same)
            this.input = inp;
            else
            {
               input = new Tensor4(input.width, input.height, input.deep, inp.bs);
               for (int d = 0; d < input.bs; d++)
                   for (int z = 0; z < input.deep; z++)
                       for (int y = weights.height - 1; y < input.height - (weights.height - 1); y++)
                           for (int x = weights.width - 1; x < input.width - (weights.width - 1); x++)
                               input[d, z, y, x] = inp[d, z, y - (weights.height - 1), x - (weights.width - 1)];
            }
            output = input.Conv(weights);
            return output;
        }

        public Tensor4 CalcDelts(ILayer lastLayer, ILayer nextLayer)
        {
            this.lastLayer = lastLayer;
            this.nextLayer = nextLayer;
            delts = nextLayer.Backward();
            return delts;
        }
        void grad(int d, double lambda)
        {
            for (int z = 0; z < weights.deep; z++)//Parallel.For(0, weights[i].deep, z =>
                for (int y = 0; y < weights.height; y++)
                    for (int x = 0; x < weights.width; x++)
                    {
                            for (int a = 0; a < delts.height; a++)
                                for (int b = 0; b < delts.width; b++)
                                {
                                    for (int bs = 0; bs < input.bs; bs++)
                                        grads[d, z, y, x] += delts[bs, d, a, b] * input[bs, z, a + y, b + x];

                                    grads[d, z, y, x] /= (double)input.bs;
                                  //  grads[d, z, y, x] /= l;
                                    grads[d, z, y, x] += lambda * weights[d, z, y, x];
                                }
                    }
        }
        public Tensor4 CalcGrads(double lambda)
        {
            grads = new Tensor4(weights.width, weights.height, weights.deep, weights.bs);
            #region
            /*
            Parallel.For(0, weights.bs, d =>
            {//for (int d = 0; d < weights.bs; d++)
                for (int z = 0; z < weights.deep; z++)//Parallel.For(0, weights[i].deep, z =>
                    for (int yw = 0; yw < weights.height; yw++)
                        for (int xw = 0; xw < weights.width; xw++)
                        {
                            for (int b = 0; b < input.bs; b++)
                                for (int y = 0; y < delts.height; y++)
                                    for (int x = 0; x < delts.width; x++)
                                        grads[d, z, yw, xw] += delts[b, d, y, x] * input[b, z, y + yw, x + xw];

                            grads[d, z, yw, xw] /= (double)input.bs;
                            grads[d, z, yw, xw] /= l;
                            grads[d, z, yw, xw] += lambda * weights[d, z, yw, xw];
                        }
            });
            //*/
            #endregion

            Parallel.For(0, weights.bs, d =>
            {//for (int d = 0; d < weights.bs; d++)
                grad(d, lambda);
            });
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
            weights *= 0.05;//2.0 / Math.Sqrt((double)input.width * input.height * input.deep);
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
            Tensor4 lastDelts = new Tensor4(input.width, input.height, input.deep, input.bs);
            Parallel.For(0, output.bs, d =>
            {//for (int d = 0; d < output.bs; d++ )
                Parallel.For(0, lastDelts.deep, n =>
                {//for (int n = 0; n < lastDelts.deep; n++)
                    {
                        for (int y = yStart; y < lastDelts.height; y++)//Parallel.For(0, delts.height, y =>
                            for (int x = xStart; x < lastDelts.width; x++)
                                for (int i = 0; i < delts.deep; i++)
                                    for (int dy = 0; dy < weights.height; dy++)
                                        for (int dx = 0; dx < weights.width; dx++)
                                                if ( ( (y - dy) > -1) && ( (x - dx) > -1) && ( (y - dy) < delts.height) && ( (x - dx) < delts.width) )
                                                    lastDelts[d, n, y - yStart, x - xStart] += delts[d, i, y - dy, x - dx] * weights[i, n, dy, dx];
                    }
                });
            });
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
