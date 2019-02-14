using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatLib;
namespace NeuralNetwork.Base.Layers
{
    class WTA : ILayer
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
        public WTA(int width, int height, int deep, int bs, int neurNum)
        {
            input = new Tensor4(width, height, deep, bs);
            weights = new Tensor4(neurNum, width * height * deep, 1, 1);
            lastWeightsDelts = new Tensor4(neurNum, width * height * deep, 1, 1);
            output = new Tensor4(neurNum, 1, 1, bs);
        }
        public Tensor4 CalcOutp(Tensor4 inp)
        {
            this.input = inp;
            output = inp.VectorOnMatrix(weights);
            for (int d = 0; d < output.bs; d++ )
            {
                int indMax = 0;
                for (int i = 1; i < output.width; i++)
                    if (output[d, 0, 0, indMax] < output[d, 0, 0, i]) indMax = i;

                for (int i = 0; i < output.width; i++)
                    output[d, 0, 0, i] = 0.0;

                output[d, 0, 0, indMax] = 1.0;
            }
            return output;
        }
        public override string ToString()
        {
            return "WTA";
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
                int ind = 0;
                for (int i = 0; i < output.width; i++)
                    if (output[0, 0, 0, i] == 1) { ind = i; break; }

                for (int i = 0; i < input.dhw; i++)
                    grads[0, 0, i, ind] = input.elements[i] - weights[0, 0, i, ind];

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
            var w = weights.ToMatrix(0, 0);
            for (int i = 0; i < weights.width; i++ )
            {
                var v = w.GetVector(i, 0);
                v.Normalize(2.0);
                w.Set(v.elements, i, 0);
            }
            weights = w.ToTensor4();
            //weights *= 4.0 / Math.Sqrt((double)input.width * input.height * input.deep);
        }

        public void SetWeights(Tensor4 weights)
        {
            this.weights = weights.Copy();
        }


        public Tensor4 CalcDelts(Tensor4 y, ILayer lastLayer)
        {
            this.lastLayer = lastLayer;
            return null;
        }


        public Tensor4 Backward()
        {
            return null;
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
