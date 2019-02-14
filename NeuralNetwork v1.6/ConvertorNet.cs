using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MatLib;
namespace NeuralNetwork
{
    public class ConvertorNet
    {
        public static Tensor4 VectorToArrTensor4(Vector x)
        {
            Tensor4 res = new Tensor4(1, 1, 1, x.Length);
            for (int i = 0; i < x.Length; i++)
            {
                res[i, 0, 0, 0] = x[i];
            }
            return res;
        }
        public static Tensor4 VectorArrToTensor4(Vector[] x)
        {
            Tensor4 res = new Tensor4(x[0].Length, 1, 1, x.Length);
            for (int i = 0; i < x.Length; i++) {
                for (int j = 0; j < x[i].Length; j++)
                   res[i, 0, 0, j] = x[i][j];
            }
            return res;
        }
    }
}
