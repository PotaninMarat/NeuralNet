using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatLib;
namespace NeuralNetwork.Base.Layers
{
    public class Softmax : ILayer
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
        double l;
        public Softmax(ILayer lastLayer, int neurNum)
        {
            this.lastLayer = lastLayer;
            input = new Tensor4(lastLayer.output.width, lastLayer.output.height, lastLayer.output.deep, lastLayer.output.bs);
            weights = new Tensor4(neurNum, input.width * input.height * input.deep, 1, 1);
            drop = new Tensor4(neurNum, input.width * input.height * input.deep, 1, 1) + 1;
            lastWeightsDelts = new Tensor4(neurNum, input.width * input.height * input.deep, 1, 1);
            output = new Tensor4(neurNum, 1, 1, input.bs);
            l = Math.Pow((double)input.hw, 0.5);
        }
        public Tensor4 CalcOutp(Tensor4 inp)
        {
            this.input = inp;
            output = inp.VectorOnMatrix(weights);
            for (int d = 0; d < output.bs; d++)
                for (int x = 0; x < output.width; x++)
                    output[d, 0, 0, x] = Math.Exp(output[d, 0, 0, x]);
            double[] sum = new double[output.bs];
            for (int d = 0; d < output.bs; d++)
                for (int x = 0; x < output.width; x++)
                    sum[d] += output[d, 0, 0, x];
            for (int d = 0; d < output.bs; d++)
                for (int x = 0; x < output.width; x++)
                    output[d, 0, 0, x] /= sum[d];
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
            if (weights.height * weights.width < 10000)
            {
                for (int y = 0; y < weights.height; y++)
                    for (int x = 0; x < weights.width; x++)
                    {
                        for (int d = 0; d < input.bs; d++)
                            grads[0, 0, y, x] += delts[d, 0, 0, x] * input.elements[d * input.dhw + y];
                        grads[0, 0, y, x] /= (double)input.bs;
                    }
            }
            else
            {
                Parallel.For(0, weights.height, y =>
                {//for (int y = 0; y < weights.height; y++)
                    for (int x = 0; x < weights.width; x++)
                    {
                        for (int d = 0; d < input.bs; d++)
                            grads[0, 0, y, x] += delts[d, 0, 0, x] * input.elements[d * input.dhw + y];
                        grads[0, 0, y, x] /= (double)input.bs;
                    }
                });
            }
            grads /= l;
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
            weights *= 0.1;// 2.0 / ((double)input.width * input.height * input.deep);
        }
        public override string ToString()
        {
            return "Softmax";
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
            var a = weights.MatrixOnVector(delts);
            Tensor4 lastDelts = new Tensor4(lastLayer.output.width, lastLayer.output.height, lastLayer.output.deep, lastLayer.output.bs);
            int mul = lastDelts.height * lastDelts.width;
            for (int d = 0; d < lastDelts.bs; d++)
                for (int z = 0; z < lastDelts.deep; z++)
                    for (int y = 0; y < lastDelts.height; y++)
                        for (int x = 0; x < lastDelts.width; x++)
                            lastDelts[d, z, y, x] = a[d, 0, 0, z * mul + y * lastDelts.width + x];
            return lastDelts;
        }
        public Vector[] WeightsNeurons()
        {
            var weigh = new Vector[weights.width];
            for (int i = 0; i < weigh.Length; i++)
            {
                weigh[i] = new Vector(weights.height);
            }
            for (int i = 0; i < weigh.Length; i++)
            {
                for (int j = 0; j < weigh[i].Length; j++)
                {
                    weigh[i][j] = weights[0, 0, j, i];
                }
            }
            return weigh;

        }
        public void SetWeightsNeurons(Tensor3[] weights)
        {

        }
    }
}
