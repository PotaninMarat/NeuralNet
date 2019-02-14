using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatLib;
namespace NeuralNetwork.Base.Layers
{
    class Plastic : ILayer
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
        public Plastic(ILayer lastLayer)
        {
            var neurNum = 3 + 3 + 1 + 1;
            this.lastLayer = lastLayer;
            input = new Tensor4(lastLayer.output.width, lastLayer.output.height, lastLayer.output.deep, lastLayer.output.bs);
            weights = new Tensor4(neurNum, input.width * input.height * input.deep, 1, 1);
            drop = new Tensor4(neurNum, input.width * input.height * input.deep, 1, 1) + 1;
            lastWeightsDelts = new Tensor4(neurNum, input.width * input.height * input.deep, 1, 1);
            output = new Tensor4(neurNum, 1, 1, input.bs);
        }
        public Tensor4 CalcOutp(Tensor4 inp)
        {
            this.input = inp;
            output = inp.VectorOnMatrix(weights);
            for (int i = 0; i < 3; i++ )
            {
                output.elements[i] = matlib.Function.Func.SigmoidalFunc.HyperbolicTangent(output.elements[i]);
            }

            for (int i = 3; i < 6; i++)
            {
                output.elements[i] = Math.Sin(output.elements[i]);
            }
            output.elements[6] = Math.Sin(output.elements[6]) * Math.Log(output.elements[6]);
            output.elements[7] = - output.elements[7] / (1.0 + Math.Exp(Math.Sin(100 * output.elements[7])));

            return output;
        }
        public override string ToString()
        {
            return "Direct";
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
            bool JoinGrads = false;
            grads = new Tensor4(weights.width, weights.height, weights.deep, input.bs);
            if (weights.height * weights.width < 10000)
            {
                for (int y = 0; y < weights.height; y++)
                    for (int x = 0; x < weights.width; x++)
                    {
                        for (int d = 0; d < input.bs; d++)
                            grads[d, 0, y, x] = delts[d, 0, 0, x] * input.elements[d * input.dhw + y];
                    }
            }
            else
            {
                Parallel.For(0, weights.height, y =>
                {//for (int y = 0; y < weights.height; y++)
                    for (int x = 0; x < weights.width; x++)
                    {
                        for (int d = 0; d < input.bs; d++)
                            grads[d, 0, y, x] = delts[d, 0, 0, x] * input.elements[d * input.dhw + y];
                    }
                });
            }
            /*
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
            //*/
            //grads += lambda * weights;
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
            weights *= 0.3;
            //weights *= 4.0 / Math.Sqrt((double)input.width * input.height * input.deep);
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
            Tensor4 lastDelts = new Tensor4(input.width, input.height, input.deep, input.bs);
            int mul = lastDelts.height * lastDelts.width;
            for (int d = 0; d < input.bs; d++)
                for (int z = 0; z < input.deep; z++)
                    for (int y = 0; y < input.height; y++)
                        for (int x = 0; x < input.width; x++)
                            lastDelts[d, z, y, x] = a[d, 0, 0, z * mul + y * input.width + x];
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
