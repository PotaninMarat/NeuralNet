using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatLib;
namespace NeuralNetwork.Base.Layers
{
    public class BatchNormalization : ILayer
    {
               public Tensor4 input { get; set; }

        public Tensor4 output { get; set; }

        public Tensor4 weights { get; set; }

        public Tensor4 drop { get; set; }

        public Tensor4 delts { get; set; }

        public Tensor4 grads { get; set; }
        public ILayer lastLayer { get; set; }
        public ILayer nextLayer { get; set; }
        public BatchNormalization(ILayer lastLayer)
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
            output = new Tensor4(output.width, output.height, output.deep, input.bs);
            output = input.BatchNormalization();

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
            return "BatchNormalization";
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
