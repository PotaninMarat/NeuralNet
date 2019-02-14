using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZedGraph;
using NeuralNetwork;
using MatLib.Regression;
using MatLib;

namespace NeuralNetwork.ReadyNeuralNetworks
{
    public class TrygonomApproxim
    {
        Vector A, fi, f;
        public double k, b;
        public double step = 1.008;
        int n;
        double aver, disp;
        LinearRegression linear;

        const double P = Math.PI * 2.0 * 10;
        const double FI = 30;
        const double A_ = 10;
        public Vector pow = new Vector(21);
        void Random()
        {
            A.GaussRandom();
            A *= 0.2;

            fi.GaussRandom();
            fi *= 0.2;

            f.GaussRandom();
            f *= 0.2;
        }
        public TrygonomApproxim(int n = 9)
        {
            this.n = n;
            linear = new LinearRegression();
            A = new Vector(n);
            fi = new Vector(n);
            f = new Vector(n);
            pow[0] = 1;
            for (int i = 1; i < pow.Length; i++)
            {
                pow[i] = pow[i - 1] * 2;
            }
           // net.layers[5].drop.elements[0] = 0.0;
           // net.layers[5].weights.elements[0] = 1.0;
        }

        public double Calculation(double x)
        {
            double res =  G(x);
           // res += k * x + b;
            res = res * disp + aver;
            return res;
        }
        double G(double x)
        {
            double res = 0.0;

            for (int i = 0; i < n; i++)
            {
                res += A[i] * Math.Sin(P * f[i] * x + FI * fi[i]);
            }
            res *= A_;
            return res;
        }
        public Vector Calculation(Vector x)
        {
            Vector y = new Vector(x.Length);
            for (int i = 0; i < x.Length; i++){
                y[i] = Calculation(x[i]);
            }
            return y;
        }
        void Train(double x, double y, double An = 0.01, double FIn = 0.01, double Fn = 0.01)
        {
            var g = G(x);
            var dif = g - y;
            //fi *= 0.0;
            Vector dA = new Vector(n);
            var dF = new Vector(n);
            var dFI = new Vector(n);

            for (int i = 0; i < n; i++)
            {
                double F = P * f[i] * x + FI * fi[i];

                double a = dif * Math.Cos(F) * A[i] * A_;


                A[i] -= dif * Math.Sin(F) * A_ * An;

                f[i] -= a * P * x * Fn;

                fi[i] -= a * FI * FIn;
            }
        }
        void TrainOffline(Vector x, Vector y, double An = 0.01, double FIn = 0.01, double Fn = 0.01)
        {
            var dA = new Vector(n);
            var df = new Vector(n);
            var dfi = new Vector(n);
            for (int k = 0; k < x.Length; k++)
            {
                int j = k;// (int)(matlib.random.NextDouble() * x.Length);
                var g = Calculation(x[j]);
                var dif = g - y[j];

                for (int i = 0; i < n; i++)
                {
                    double F = P * f[i] * x[j] + FI * fi[i];

                    double a = dif * Math.Cos(F) * A[i] * A_;


                    dA[i] += dif * Math.Sin(F) * A_;

                    df[i] += a * P * x[j];

                    dfi[i] += a * FI;
                }
            }
            double t = (double)x.Length;
            dA /= t;
            df /= t;
            dfi /= t;

            dA *= 0.000001;
            df *= 0.0001;
            dfi *= 0.0001;
            
            //A -= dA;
           // f -= df;
            //fi -= dfi;
        }
        int Search(int n)
        {
            int l = -1;
            for (int i = 0; i < pow.Length; i++)
            {
                if (pow[i] >= n)
                {
                    l = (int)pow[i];
                    break;
                }
            }
            return l;
        }
        public Vector Calcf(Vector x, Vector y)
        {
            var y_ = new Vector(y);
            y_.Resize(Search(y.Length));

            var cmplx = matlib.Transofm.FFT.fft(y_);
            var freq = matlib.Convertor.Complex_.ComplexInMagnitude(cmplx);
            freq.Resize(freq.Length / 2);
            //freq = freq.GetInterval(1, freq.Length - 1);
            Vector ind = Vector.GetVectorIndexes(freq.Length, 0);
            var res = matlib.Sort.qsort(ind, freq);
            ind = res[0];
            int C = 0;
            for (int i = 0; C < n; i++)
            {
                if (ind[ind.Length - i - 1] != 0)
                f[C++] = ind[ind.Length - i - 1];
            }
            C = 0;
            for (int i = 0; i < n; i++)
            {
                if (ind[ind.Length - i - 1] != 0)
                A[C++] = freq[(int)ind[ind.Length - 1 - i]] * 2.0 / x.Length;
                //if ((int)ind[ind.Length - 1 - i] == 0) A[i] /= 2.0;
            }

            for (int i = 0; i < n; i++)
            {
               // fi[i] = cmplx[(int)ind[ind.Length - 1 - i]].Phase;// *2.0 / x.Length;
            }
            return null;
        }
        public double Learn(Vector x, Vector y)
        {

            //

            Random();
            aver = y.AverageValue();
            var y_ = new Vector(y.Length);

            {
                y_ = y - aver; disp = Math.Sqrt(y_.Dispersion()); y_ /= disp;
            }

            if (y_.IsNaN()) return double.NaN;

            double m = 0.001;

            {
                var lin = linear.Regression(x, y_); k = linear.k; b = linear.b; 
               // y_ = y_ - lin;
            }
            //Calcf(x, y_);
            for (int ep = 0; ep < 50; ep++)
            {
                for (int i = 0; i < 2*x.Length; i+=1)
                {
                    int ind = (int)(matlib.random.NextDouble() * x.Length);
                    Train(x[ind], y_[ind], m, m, m);
                }

                if (m > 5e-5)
                {
                    m /= step;
                }
            }
            double err = 0.0;
            for (int i = 0; i < x.Length; i++)
            {
                err += Math.Abs(G(x[i]) - y_[i]);
            }
            err /= (double)x.Length;
            return err;
        }

    }
}
