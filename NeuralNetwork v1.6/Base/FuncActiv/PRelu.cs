using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MatLib;
namespace NeuralNetwork.Base.FuncActiv
{
    class PRelu : ILayer
    {
        public Tensor4 input { get; set; }

        public Tensor4 output { get; set; }

        public Tensor4 weights { get; set; }

        public Tensor4 drop { get; set; }

        public Tensor4 delts { get; set; }

        public Tensor4 grads { get; set; }
        public ILayer lastLayer { get; set; }
        public ILayer nextLayer { get; set; }
        public PRelu(ILayer lastLayer)
        {
            this.lastLayer = lastLayer;
            int width = lastLayer.output.width;
            int height = lastLayer.output.height;
            int deep = lastLayer.output.deep;
            int bs = lastLayer.output.bs;

            input = new Tensor4(width, height, deep, bs);
            output = new Tensor4(width, height, deep, bs);
        }
        public Tensor4 CalcOutp(Tensor4 inp)
        {
            this.input = inp;

            output = new Tensor4(inp.width, inp.height, inp.deep, inp.bs);

            for (int i = 0; i < input.elements.Length; i++)
                output.elements[i] = (input.elements[i] > 0.0) ? input.elements[i] : 0.1 * input.elements[i];

            return output;
        }
        public Tensor4 GetDeriv()
        {
            Tensor4 res = new Tensor4(input.width, input.height, input.deep, input.bs);

            for (int i = 0; i < input.elements.Length; i++)
                res.elements[i] = (output.elements[i] > 0.0) ? 1 : 0.1;

            return res;
        }

        public Tensor4 CalcDelts(ILayer lastLayer, ILayer nextLayer)
        {
            this.lastLayer = lastLayer;
            this.nextLayer = nextLayer;
            delts = nextLayer.Backward() * GetDeriv();
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
            delts = (output - y) * GetDeriv();
            return delts;
        }


        public Tensor4 Backward()
        {
            return delts;
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
