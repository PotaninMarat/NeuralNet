using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatLib;
namespace NeuralNetwork.Base.Layers
{
    public class VectorToTensor3 : ILayer
    {
       public Tensor4 input { get; set; }

        public Tensor4 output { get; set; }

        public Tensor4 weights { get; set; }

        public Tensor4 drop { get; set; }

        public Tensor4 delts { get; set; }

        public Tensor4 grads { get; set; }
        public ILayer lastLayer { get; set; }
        public ILayer nextLayer { get; set; }
        public VectorToTensor3(ILayer lastLayer, int newWidth, int newHeight, int newDeep)
        {
            this.lastLayer = lastLayer;
            int width = lastLayer.output.width;
            int height = lastLayer.output.height;
            int deep = lastLayer.output.deep;
            int bs = lastLayer.output.bs;

            if (height * deep != 1) throw new Exception();
            if (width * height * deep != newWidth * newHeight * newDeep) throw new Exception("Размеры не совпадают");
            input = new Tensor4(width, height, deep, bs);
            output = new Tensor4(newWidth, newHeight, newDeep, bs);
        }
        public Tensor4 CalcOutp(Tensor4 inp)
        {
            this.input = inp;
            output = new Tensor4(output.width, output.height, output.deep, input.bs);
            int mul = output.height * output.width;
            for (int d = 0; d < output.bs; d++)
                for (int z = 0; z < output.deep; z++)
                    for (int y = 0; y < output.height; y++)
                        for (int x = 0; x < output.width; x++)
                            output[d, z, y, x] = input[d, 0, 0, z * mul + y * output.width + x];
            return output;
        }
        public override string ToString()
        {
            return "VectorToTensor3";
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
            Tensor4 lastDelts = new Tensor4(lastLayer.output.width, lastLayer.output.height, lastLayer.output.deep, lastLayer.output.bs);
            lastDelts.elements = delts.ToVector().elements;
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
