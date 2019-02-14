using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatLib;
namespace NeuralNetwork.Base.Layers
{
    public class Dropout : ILayer
    {

        public Tensor4 input { get; set; }

        public Tensor4 output { get; set; }

        public Tensor4 weights
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Tensor4 drop
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Tensor4 delts { get; set; }

        public Tensor4 grads
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Dropout()
        {

        }
        public Tensor4 CalcOutp(Tensor4 inp)
        {
            this.input = inp;
            output = inp;
            return output;
        }

        public Tensor4 CalcDelts(ILayer lastLayer, ILayer nextLayer)
        {
            this.nextLayer = nextLayer;
            this.lastLayer = lastLayer;
            delts = nextLayer.Backward();
            return delts;
        }

        public Tensor4 CalcDelts(Tensor4 y, ILayer lastLayer)
        {
            throw new NotImplementedException();
        }

        public Tensor4 CalcGrads(double lambda)
        {
            throw new NotImplementedException();
        }

        public Tensor4 Train(double norm, double moment)
        {
            throw new NotImplementedException();
        }

        public ILayer lastLayer { get; set; }

        public ILayer nextLayer { get; set; }

        public void RandomWeights()
        {
           
        }
        public override string ToString()
        {
            return "Dropout";
        }

        public void SetWeights(Tensor4 weights)
        {
            
        }

        public Tensor4 Backward()
        {
            throw new NotImplementedException();
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
