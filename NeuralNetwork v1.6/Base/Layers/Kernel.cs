using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatLib;
namespace NeuralNetwork.Base.Layers
{
    public class Kernel : ILayer
    {
        public Tensor4 input { get; set; }

        public Tensor4 output { get; set; }

        public Tensor4 weights { get; set; } // опорные вектора

        public Tensor4 drop { get; set; }

        public Tensor4 delts { get; set; }

        public Tensor4 grads { get; set; }
        public ILayer lastLayer { get; set; }
        public ILayer nextLayer { get; set; }
        public Tensor4 lastWeightsDelts;
        public Kernel(ILayer lastLayer)
        {
            this.lastLayer = lastLayer;
            int width = lastLayer.output.width;
            int height = lastLayer.output.height;
            int deep = lastLayer.output.deep;
            int bs = lastLayer.output.bs;
            input = new Tensor4(width, height, deep, bs);
            weights = new Tensor4(width * height * deep, width, height, deep);
            lastWeightsDelts = new Tensor4(width * height * deep, width, height, deep);
            output = new Tensor4(2 * width * height * deep, 1, 1, bs);
        }
        public Tensor4 CalcOutp(Tensor4 inp)
        {
            this.input = inp;
            output = new Tensor4(output.width, output.height, output.deep, input.bs);
            int mul = input.height * input.width;
            if (input.bs == 1)
            {
                    Parallel.For(0, input.deep, z =>
                    {//for (int z = 0; z < input.deep; z++)
                        for (int y = 0; y < input.height; y++)
                            for (int x = 0; x < input.width; x++)
                            {
                                for (int zz = 0; zz < input.deep; zz++)
                                    for (int yy = 0; yy < input.height; yy++)
                                        for (int xx = 0; xx < input.width; xx++)
                                        {
                                            double a = input[0, zz, yy, xx] - weights[zz, yy, xx, z * mul + y * input.width + x];
                                            output.elements[z * input.height * inp.width + y * inp.width + x] += a * a;
                                        }
                                output.elements[z * input.height * inp.width + y * inp.width + x] = Math.Sqrt(output.elements[z * input.height * inp.width + y * inp.width + x]);
                            }
                    });
            }
            else
            {
                Parallel.For(0, input.bs, d =>
                {//for (int d = 0; d < input.bs; d++)
                    for (int z = 0; z < input.deep; z++)
                        for (int y = 0; y < input.height; y++)
                            for (int x = 0; x < input.width; x++)
                            {
                                for (int zz = 0; zz < input.deep; zz++)
                                    for (int yy = 0; yy < input.height; yy++)
                                        for (int xx = 0; xx < input.width; xx++)
                                        {
                                            double a = input[d, zz, yy, xx] - weights[zz, yy, xx, z * mul + y * input.width + x];
                                            output[d, z, y, x] += a * a;
                                        }
                                output[d, z, y, x] = Math.Sqrt(output[d, z, y, x]);
                            }
                });
            }

            int l = output.width / 2;
            for (int d = 0; d < output.bs; d++)
                for (int i = l; i < output.width; i++)
                    output[d, 0, 0, i] = input.elements[d * input.dhw + i - l];

                return output;
        }

        public Tensor4 CalcDelts(ILayer lastLayer, ILayer nextLayer)
        {
            this.lastLayer = lastLayer;
            this.nextLayer = nextLayer;
            var a = nextLayer.Backward();
            var b = CalcDerivs();
            delts = new Tensor4(b.dhw, 1, 1, a.bs);
            for (int d = 0; d < b.bs; d++)
                for (int i = 0; i < b.dhw; i++)
                    delts[d, 0, 0, i] = a[d, 0, 0, i] * b[d, 0, 0, i];
                //delts = nextLayer.Backward() * CalcDerivs();
                return delts;
        }

        public Tensor4 CalcDerivs()
        {
            Tensor4 derivs = new Tensor4(input.dhw, 1, 1, input.bs);

                        for (int ds = 0; ds < input.bs; ds++)
                        {
                            for (int i = 0; i < input.dhw; i++)
                                derivs[ds, 0, 0, i] = 1.0 / output[ds, 0, 0, i];
                        }
            return derivs;
        }
        public Tensor4 CalcGrads(double lambda)
        {
            grads = new Tensor4(weights.width, weights.height, weights.deep, weights.bs);
            double l = Math.Pow((double)input.dhw, 0.2);
            for (int dz = 0; dz < weights.bs; dz++)
                for (int dy = 0; dy < weights.deep; dy++)
                    for (int dx = 0; dx < weights.height; dx++)
                    {
                        for (int z = 0; z < delts.deep; z++)
                            for (int y = 0; y < delts.height; y++)
                                for (int x = 0; x < delts.width; x++) 
                                {
                                    for (int ds = 0; ds < input.bs; ds++)
                                        grads[dz, dy, dx, z * delts.hw + y * delts.width + x] += delts[ds, z, y, x] * (weights[dz, dy, dx, z * delts.hw + y * delts.width + x] - input[ds, dz, dy, dx]) / output[ds, z, y, x];

                                    grads[dz, dy, dx, z * delts.hw + y * delts.width + x] /= (double)input.bs;
                                    grads[dz, dy, dx, z * delts.hw + y * delts.width + x] /= l;
                                }
                    }
            
            grads += lambda * weights;
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
            weights *= 0.2;
            //weights *= 7.0 / Math.Sqrt((double)input.width * input.height * input.deep);
        }

        public void SetWeights(Tensor4 weights)
        {
            this.weights = weights.Copy();
        }


        public Tensor4 CalcDelts(Tensor4 y, ILayer lastLayer)
        {
            this.lastLayer = lastLayer;
            delts = (output - y) * CalcDerivs();
            return delts;
        }


        public Tensor4 Backward()
        {
            Tensor4 lastDelts = new Tensor4(input.width, input.height, input.deep, input.bs);
                for (int ds = 0; ds < input.bs; ds++)
                    for (int dz = 0; dz < input.deep; dz++)
                        for (int dy = 0; dy < input.height; dy++)
                            for (int dx = 0; dx < input.width; dx++)
                            {
                                for (int z = 0; z < delts.deep; z++)
                                    for (int y = 0; y < delts.height; y++)
                                        for (int x = 0; x < delts.width; x++)
                                        {
                                            lastDelts[ds, dz, dy, dx] += delts[ds, z, y, x] * (input[ds, dz, dy, dx] - weights[dz, dy, dx, z * delts.hw + y * delts.width + x]);
                                        }
                            }
          
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
