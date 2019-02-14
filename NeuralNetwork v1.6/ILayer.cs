using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatLib;
namespace NeuralNetwork
{
    public interface ILayer
    {
        Tensor4 input {get; set;}
        Tensor4 output { get; set; }
        Tensor4 weights { get; set; }
        Tensor4 drop { get; set; }
        Tensor4 delts { get; set; }
        Tensor4 grads { get; set; }
        Tensor4 CalcOutp(Tensor4 inp);
        Tensor4 CalcDelts(ILayer lastLayer, ILayer nextLayer);
        Tensor4 CalcDelts(Tensor4 y, ILayer lastLayer);
        Tensor4 CalcGrads(double lambda);
        Vector[] WeightsNeurons();
        void SetWeightsNeurons(Tensor3[] weights);
        Tensor4 Train(double norm, double moment);
        ILayer lastLayer { get; set; }
        ILayer nextLayer { get; set; }
        void RandomWeights();
        void SetWeights(Tensor4 weights);
        Tensor4 Backward();
    }
}
