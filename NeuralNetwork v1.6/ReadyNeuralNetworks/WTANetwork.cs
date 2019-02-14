using MatLib;
using NeuralNetwork.Base.Layers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork.ReadyNeuralNetworks
{
    public class WTANetwork
    {
        WTA layer;
        public WTANetwork(int inpNum, int neurNum)
        {
            layer = new WTA(inpNum, 1, 1, 1, neurNum);
            layer.RandomWeights();
        }
        public void Train(Vector x, double norm = 0.01, double moment = 0.0)
        {
            Vector vect = new Vector(x);
            if (vect.EuclidNorm() != 1.0) vect.Normalize(2.0);
            Tensor4 inp = new Tensor4(vect.Length, 1, 1, 1);
            inp.elements = vect.elements;
            layer.CalcOutp(inp);
            layer.CalcGrads(0.0);
            layer.Train(norm, moment);
        }
        public int Run(Vector x)
        {
            Tensor4 inp = new Tensor4(x.Length, 1, 1, 1);
            inp.elements = x.elements;
            var outp = layer.CalcOutp(inp);
            int ind = 0;
            for(int i = 0; i < outp.width; i++)
                if (outp[0, 0, 0, i] == 1) { ind = i; break; }

            return ind;
        }
        public void SaveWeights(string path)
        {
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter binForm = new BinaryFormatter();
                binForm.Serialize(stream, layer.weights);
            }
        }
        public void LoadWeights(string path)
        {
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                BinaryFormatter binForm = new BinaryFormatter();
                Tensor4 weights = (Tensor4)binForm.Deserialize(stream);
                layer.SetWeights(weights);
            }
        }
    }
}
