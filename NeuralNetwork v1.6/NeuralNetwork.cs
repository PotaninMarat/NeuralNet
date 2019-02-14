using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuralNetwork.Base.Layers;
using NeuralNetwork.Base.FuncActiv;
using MatLib;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
namespace NeuralNetwork
{
    public class NeuralNet
    {
        public List<ILayer> layers;
        public string[] architecture;
        public Tensor4 output;
        // for ConjGrads
        Tensor4[] lastGrads;
        Tensor4[] lastB;
        //
        // for Adam
        public Tensor4[] m, v;
        public double b1 = 0.9, b2 = 0.999;
        double e = 1e-8;
        Processor processor;
        //
    
        public enum Optimizer
        {
            SGD,
            ConjGrads,
            Adam,
            Taylor,
            Marat,
            NonIteratorLinear
        }
        public enum Processor
        {
            CPU,
            GPU
        }
        /// <summary>
        /// layers:
        /// input
        /// direct
        /// polariz
        /// softmax
        /// conv
        /// convEuclid
        /// pool
        /// unpool
        /// vectortotensor3
        /// tensor3tovector
        /// butchnormalization
        /// kernel
        /// 
        /// funcs:
        /// gauss
        /// sign
        /// threshold
        /// relu
        /// sin
        /// tanh
        /// sigmoid
        /// </summary>
        /// <param name="architecture"></param>
        public NeuralNet(string[] architecture, Processor processor = Processor.CPU)
        {
            this.processor = processor;
            SetNet(architecture);
        }
        void SetNet(string[] architecture)
        {
            layers = new List<ILayer>();

            this.architecture = new string[architecture.Length];
            architecture.CopyTo(this.architecture, 0);

            for (int i = 0; i < architecture.Length; i++)
            {
                string[] param = architecture[i].Replace(" ", string.Empty).Split(':');
                param[0] = param[0].ToLower();
                switch (param[0])
                {
                    #region layers
                    case "input":
                        layers.Add(new Input(int.Parse(param[1]), int.Parse(param[2]), int.Parse(param[3]), int.Parse(param[4])));
                        break;
                    case "direct":
                        layers.Add(new Direct(layers[i - 1], int.Parse(param[1])));
                        break;
                    case "kernel":
                        layers.Add(new Kernel(layers[i - 1]));
                        break;
                    case "polariz":
                        layers.Add(new Polarization(layers[i - 1]));
                        break;
                    case "hand":
                        layers.Add(new Hand(layers[i - 1]));
                        break;
                    case "softmax":
                        layers.Add(new Softmax(layers[i - 1], int.Parse(param[1])));
                        break;
                    case "conv":
                        if (param.Length > 4)
                            layers.Add(new Convolutional(layers[i - 1], int.Parse(param[1]), int.Parse(param[2]), int.Parse(param[3]), ((param[4] == "same") ? true : false)));
                        else layers.Add(new Convolutional(layers[i - 1], int.Parse(param[1]), int.Parse(param[2]), int.Parse(param[3]), false));
                        break;
                    case "conveuclid":
                        if (param.Length > 4)
                            layers.Add(new ConvolutionalEuclidean(layers[i - 1], int.Parse(param[1]), int.Parse(param[2]), int.Parse(param[3]), ((param[4] == "same") ? true : false)));
                        else layers.Add(new ConvolutionalEuclidean(layers[i - 1], int.Parse(param[1]), int.Parse(param[2]), int.Parse(param[3]), false));
                        break;
                    case "convdistance":
                        layers.Add(new ConvolutionalDistance(layers[i - 1], int.Parse(param[1])));
                        break;
                    case "pool":
                        layers.Add(new Pool(layers[i - 1], int.Parse(param[1]), int.Parse(param[2])));
                        break;
                    case "unpool":
                        layers.Add(new Unpool(layers[i - 1]));
                        break;
                    case "unpool1":
                        layers.Add(new Unpool1(layers[i - 1]));
                        break;
                    case "vectortotensor3":
                        layers.Add(new VectorToTensor3(layers[i - 1], int.Parse(param[1]), int.Parse(param[2]), int.Parse(param[3])));
                        break;
                    case "tensor3totensor3":
                        layers.Add(new Tensor3ToTensor3(layers[i - 1], int.Parse(param[1]), int.Parse(param[2]), int.Parse(param[3])));
                        break;
                    case "deeptomatrix":
                        layers.Add(new DeepToMatrix(layers[i - 1]));
                        break;
                    case "tensor3tovector":
                        layers.Add(new Tensor3ToVector(layers[i - 1]));
                        break;
                    case "butchnormalization":
                        layers.Add(new BatchNormalization(layers[i - 1]));
                        break;
                    #endregion

                    #region funvActiv
                    case "sigmoid":
                        layers.Add(new Sigmoid(layers[i - 1]));
                        break;
                    case "tanh":
                        layers.Add(new Tanh(layers[i - 1]));
                        break;
                    case "sin":
                        layers.Add(new Sin(layers[i - 1]));
                        break;
                    case "cos":
                        layers.Add(new Cos(layers[i - 1]));
                        break;
                    case "relu":
                        layers.Add(new Relu(layers[i - 1]));
                        break;
                    case "prelu":
                        layers.Add(new PRelu(layers[i - 1]));
                        break;
                    case "threshold":
                        layers.Add(new Threshold(layers[i - 1], double.Parse(param[1])));
                        break;
                    case "sign":
                        layers.Add(new Sign(layers[i - 1]));
                        break;
                    case "gauss":
                        layers.Add(new Gauss(layers[i - 1], double.Parse(param[1])));
                        break;
                    #endregion

                    default: throw new Exception("Такого типа слоя или функции активации не существует.");
                }
            }
            output = layers[layers.Count - 1].output;
            Random();
        }
        public NeuralNet(string pathToModel, Processor processor = Processor.CPU)
        {
            this.processor = processor;

            using(FileStream stream = new FileStream(pathToModel, FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                string[] architecture = (string[])bf.Deserialize(stream);
                Tensor4[] weights = (Tensor4[])bf.Deserialize(stream);

                SetNet(architecture);
                SetWeights(weights);
            }
        }
        public void Random()
        {
            for (int i = 0; i < layers.Count; i++)
                layers[i].RandomWeights();
        }
        public int[] CalculationBase(Tensor4 inp, int layNum, double thres)
        {
            layers[layNum].CalcOutp(inp);
            for (int i = layNum + 1; i < layers.Count; i++)
                layers[i].CalcOutp(layers[i - 1].output);

            this.output = layers[layers.Count - 1].output;

            Vector[] res = new Vector[inp.bs];
            int[] classesMax = new int[res.Length];
            for (int i = 0; i < res.Length; i++)
                res[i] = layers[layers.Count - 1].output.ToVector(i);

            for (int j = 0; j < res.Length; j++)
            {
                classesMax[j] = 0;
                for (int i = 1; i < res[j].Length; i++)
                {
                    if (res[j][classesMax[j]] < res[j][i])
                        classesMax[j] = i;
                }
                if (res[j][classesMax[j]] < thres)
                    classesMax[j] = -1;
            }
            return classesMax;
        }
        public int Calculation(Matrix inp, int layNum = 0, double thres = 0.4)
        {
            Tensor4 input = inp.ToTensor4();
            return CalculationBase(input, layNum, thres)[0];
        }
        public int Calculation(Vector inp, int layNum = 0, double thres = 0.4)
        {
            Tensor4 input = inp.ToTensor4();
            return CalculationBase(input, layNum, thres)[0];
        }
        public int Calculation(Tensor3 inp, int layNum = 0, double thres = 0.4)
        {
            Tensor4 input = inp.ToTensor4();
            return CalculationBase(input, layNum, thres)[0];
        }
        public int[] Calculation(Tensor4 inp, int layNum = 0, double thres = 0.4)
        {
            return CalculationBase(inp, layNum, thres);
        }
        /*
        public void MethodWTA(Vector[] x, double n)
        {
            if (Layers.Count == 2)
                    for (int i = 0; i < x.Length; i++)
                    {
                        Run(x[i].elements);
                        for (int j = 0; j < Layers[1].Length; j++)
                            if (Layers[1].elements.Max() == Layers[1].elements[j])
                                for (int k = 0; k < weights[0].height; k++)
                                    weights[0].elements[k, j] += n * (Layers[0].elements[k] - weights[0].elements[k, j]);
                    }
            else { throw new Exception("Количество слоев не равно 2."); }
        }
        //*/
        static public void RandomSort(Tensor4 x, Tensor4 y)
        {
            int[] indexes = new int[x.bs];
            for (int i = 0; i < x.bs; i++)
            {
                bool exist = true;
                while (exist)
                {
                    exist = false;
                    indexes[i] = (int)(matlib.random.NextDouble() * x.bs);
                    for (int j = i - 1; j >= 0; j--)
                        if (indexes[i] == indexes[j]) { exist = true; break; }
                }
            }
            for (int i = 0; i < x.bs; i++)
            {
                //Tensor3 copy
            }
        }
     
        void BackwardBase(Tensor4 x, Tensor4 y, double norm, double moment, double lambda, Optimizer optimizer)
        {
        
            Tensor4[] grads, weights;
            Vector E;
            Matrix A;
            Vector b;
            switch(optimizer)
            {
                #region SGD
                case Optimizer.SGD:
                    CalcDelts(x, y);
                    CalcGrads(lambda);
                    Train(norm, moment);
                    break;
                #endregion
                #region ConjGrads
                case Optimizer.ConjGrads:
                    CalcDelts(x, y);
                    grads = CalcGrads(lambda);
                    weights = GetWeights();
                    if(lastGrads == null)
                    {
                        lastGrads = new Tensor4[layers.Count];
                        lastB = new Tensor4[layers.Count];
                        for(int i = 0; i < lastGrads.Length; i++)
                            if(grads[i] != null)
                            {
                            lastGrads[i] = new Tensor4(grads[i].width, grads[i].height, grads[i].deep, grads[i].bs);
                            lastB[i] = new Tensor4(grads[i].width, grads[i].height, grads[i].deep, grads[i].bs);
                            }
                    }
                    for (int i = 0; i < layers.Count; i++ )
                    {
                       if(grads[i] != null)
                       {
                           double lG_l2 = lastGrads[i].EuclidNorm();
                           double w = (lG_l2 != 0.0) ? grads[i].EuclidNorm() / lG_l2 : 0.0;
                           lastB[i] = grads[i] + w * lastB[i];

                           weights[i] -= norm * lastB[i];
                       }

                    }
                    lastGrads = grads;
                    SetWeights(weights);
                    break;
                #endregion
                #region Adam
                case Optimizer.Adam:
                    CalcDelts(x, y);
                    grads = CalcGrads(lambda);
                    weights = GetWeights();
                        if (m == null || v == null)
                        {
                            m = new Tensor4[grads.Length];
                            v = new Tensor4[grads.Length];
                            for (int i = 0; i < grads.Length; i++)
                                if (grads[i] != null)
                                {
                                    m[i] = new Tensor4(grads[i].width, grads[i].height, grads[i].deep, grads[i].bs);
                                    v[i] = new Tensor4(grads[i].width, grads[i].height, grads[i].deep, grads[i].bs);
                                }
                        }

                        Parallel.For(0, grads.Length, i =>
                        {//for (int i = 1; i < grads.Length; i++)
                            if (grads[i] != null)
                            {
                                    m[i] = b1 * m[i] + (1.0 - b1) * grads[i];
                                    v[i] = b2 * v[i] + (1.0 - b2) * (grads[i] * grads[i]);
                                    var M = m[i] / (1.0 - b1);
                                    var V = v[i] / (1.0 - b2);
                                    weights[i] -= norm * M / (V.ElementsPow(0.5) + e);
                            }
                        });
                        SetWeights(weights);
                    break;
                #endregion
                #region Marat
                case Optimizer.Marat:
                    
                    CalcDelts(x, y);
                    grads = CalcGrads(lambda);
                    weights = GetWeights();

                    int d = x.bs;


                    E = CalcErrRootMSEBase2(x, y);
                    //*
                    for (int i = 0; i < weights.Length; i++)
                    {
                        if (grads[i] != null)
                        {
                            if (d < weights[i].dhw) throw new Exception("Недостаточно обучающей выборки.");
                            A = new Matrix(weights[i].dhw, weights[i].dhw);
                            b = new Vector(weights[i].dhw);

                            var w = layers[i].WeightsNeurons();
                            for (int n = 0; n < w.Length; n++)
                            {
                                
                            }
                        }
                    }
                    //*
                    //*/

                    /*
                    k = 0;
                    for (int i = 0; i < weights.Length; i++)
                    {
                        if (grads[i] != null)
                        {
                            for (int j = 0; j < weights[i].dhw; j++)
                            {
                                weights[i].elements[j] = a[k++];
                            }
                        }
                    }
                    //*/
                    SetWeights(weights);
                    break;
                #endregion
                #region Taylor
                case Optimizer.Taylor:
                    CalcDelts(x, y);

                    grads = CalcGrads(lambda);
                    weights = GetWeights();
                    E = CalcErrRootMSEBase2(x, y);
                    if (E.Length != 1) throw new Exception("Ты пидор");

                        for (int i = 1; i < grads.Length; i++)
                        {//for (int i = 1; i < grads.Length; i++)
                            if (grads[i] != null)
                            {
                                for (int j = 0; j < grads[i].elements.Length; j++)
                                {
                                    var grad = E[0] / grads[i].elements[j];
                                    if (!Double.IsNaN(grad) && !Double.IsInfinity(grad))
                                    {
                                        weights[i] -= norm * grad;
                                    }
                                    else
                                    {

                                    }
                                }
                            }
                        };
                        SetWeights(weights);
                    //Train(norm, moment);
                    break;
                #endregion
                #region NonIteratorLinear
                case Optimizer.NonIteratorLinear:
                    int wN = layers[layers.Count - 1].weights.height;
                    var input = x;//[k, 0, 0, j]
                    for (int i = 0; i < layers[layers.Count - 1].weights.width; i++)
                    {
                        A = new Matrix(wN, wN);
                        b = new Vector(wN);

                        for (int j = 0; j < b.Length; j++) {
                            for (int k = 0; k < input.bs; k++) {
                                b[j] += y[k, 0, 0, i] * input[k, 0, 0, j]; 
                            }
                        }
                        for (int q = 0; q < wN; q++)
                         for (int n = 0; n < wN; n++)
                        for (int k = 0; k < input.bs; k++)
                        {
                            A[q, n] += input[k, 0, 0, n] * input[k, 0, 0, q];
                        }
                        //A.Transpose();
                            var result = matlib.SolvingSystems.SolvingSLAY(A, b);
                            for (int j = 0; j < wN; j++)
                            {
                                layers[layers.Count - 1].weights[0, 0, j, i] = result[j];
                            }
                    }
                    break;
                #endregion
            }
            //if(lambda != 0.0)
            //Regularization(lambda);
        }
        
        void Regularization(double lambda)
        {
            var grads = CalcGrads(lambda);
            var weights = GetWeights();

            Parallel.For(0, grads.Length, i =>
            {//for (int i = 1; i < grads.Length; i++)
                if (grads[i] != null && !(layers[i] is Direct) && !(layers[i] is Softmax))
                    weights[i] -= lambda * weights[i];  
            });
            SetWeights(weights);
        }
        public void TrainWithTeach(Tensor4 x, Tensor4 y, double norm = 1e-3, double moment = 0.0, double lambda = 0.0, Optimizer optimizer = Optimizer.SGD)
        {
            BackwardBase(x, y, norm, moment, lambda, optimizer);
        }
        public void TrainWithTeach(Tensor4 x, int[] y, double norm = 1e-3, double moment = 0.0, double lambda = 0.0, Optimizer optimizer = Optimizer.SGD)
        {
            Tensor4 Y = new Tensor4(output.width, output.height, output.deep, output.bs);
            if (output.width != 1 && output.height * output.deep == 1)
                for (int i = 0; i < y.Length; i++)
                {
                    if(y[i] != -1)
                    Y[i, 0, 0, y[i]] = 1.0;
                }
            else if (output.deep != 1 && output.height * output.width == 1)
                for (int i = 0; i < y.Length; i++)
                    if(y[i] != -1)
                    Y[i, y[i], 0, 0] = 1.0;
            else throw new Exception();
                BackwardBase(x, Y, norm, moment, lambda, optimizer);
        }
        public void TrainWithTeach(Vector x, int y, double norm = 1e-3, double moment = 0.0, double lambda = 0.0, Optimizer optimizer = Optimizer.SGD)
        {
            Tensor4 X = new Tensor4(x.Length, 1, 1, 1);
            X.elements = x.elements;
            Tensor4 Y = new Tensor4(output.width, output.height, output.deep, 1);
            if(y != -1)
            Y[0, 0, 0, y] = 1.0;
            BackwardBase(X, Y, norm, moment, lambda, optimizer);
        }
        public void TrainWithTeach(Vector x, int y, double val, double norm = 1e-3, double moment = 0.0, double lambda = 0.0, Optimizer optimizer = Optimizer.SGD)
        {
            Tensor4 X = new Tensor4(x.Length, 1, 1, 1);
            X.elements = x.elements;
            Tensor4 Y = new Tensor4(output.width, output.height, output.deep, 1);
            if (y != -1)
                Y[0, 0, 0, y] = 1.0;

            for (int i = 0; i < layers.Count; i++)
            {
                if(layers[i] is Hand) {
                    layers[i].output.elements[layers[i].output.elements.Length - 1] = val;
                    break;
                }
            }
            BackwardBase(X, Y, norm, moment, lambda, optimizer);
        }
        public int Calculation(Vector inp, double val, int layNum = 0, double thres = 0.4)
        {
            Tensor4 input = inp.ToTensor4();
            for (int i = 0; i < layers.Count; i++)
            {
                if (layers[i] is Hand)
                {
                    layers[i].output.elements[layers[i].output.elements.Length - 1] = val;
                    break;
                }
            }
            return CalculationBase(input, layNum, thres)[0];
        }
        public void TrainWithTeach(Matrix x, int y, double norm = 1e-3, double moment = 0.0, double lambda = 0.0, Optimizer optimizer = Optimizer.SGD)
        {
            Tensor4 X = new Tensor4(x.width, x.height, 1, 1);
            X.elements = x.elements;
            Tensor4 Y = new Tensor4(output.width, output.height, output.deep, 1);
            if (y != -1)
            Y[0, 0, 0, y] = 1.0;
            BackwardBase(X, Y, norm, moment, lambda, optimizer);
        }
        //public void TrainWithoutTeach(Vector x, int y)
        void CalcDelts(Tensor4 x, Tensor4 y)
        {
            Calculation(x);

            layers[layers.Count - 1].CalcDelts(y, layers[layers.Count - 2]);
            for (int i = layers.Count - 2; i >= 1; i--)
                layers[i].CalcDelts(layers[i - 1], layers[i + 1]);
        }
        Tensor4[] CalcGrads(double lambda)
        {
            Tensor4[] grads = new Tensor4[layers.Count];
            //for (int i = 0; i < layers.Count; i++)
            Parallel.For(0, layers.Count, i =>
            {
                grads[i] = layers[i].CalcGrads(lambda);
            });
            return grads;
        }
        Tensor4[] GetWeights()
        {
            Tensor4[] weights = new Tensor4[layers.Count];
            for (int i = 0; i < layers.Count; i++)
                weights[i] = layers[i].weights;
            return weights;
        }
        public void SetWeights(Tensor4[] weights)
        {
            for (int i = 0; i < layers.Count; i++)
                layers[i].SetWeights(weights[i]);
        }
        void Train(double norm, double moment)
        {
            for (int i = 0; i < layers.Count; i++)
                layers[i].Train(norm, moment);
        }
        public Tensor4[] CopyWeights()
        {
            Tensor4[] weights = new Tensor4[layers.Count];
            for (int i = 0; i < layers.Count; i++)
                if (layers[i].weights != null)
                weights[i] = layers[i].weights.Copy();
            return weights;
        }
        public double CalcErrRootMSEBase(Tensor4[] x, Tensor4[] y)
        {
            if (x.Length != y.Length) throw new Exception();
            double err = 0.0;
            for (int i = 0; i < x.Length; i++)
            {
                Calculation(x[i]);
                err += (output - y[i]).ElementsPow(2.0).elements.Sum();
            }
            err /= 2.0;
            return err;
        }
        public Vector CalcErrRootMSEBase2(Tensor4 x, Tensor4 y)
        {
            if (x.bs != y.bs) throw new Exception();
            Vector err = new Vector(x.bs);
            Calculation(x);
            for (int i = 0; i < x.bs; i++)
            {
                for (int j = 0; j < y.dhw; j++) {
                    err[i] += Math.Pow((output.elements[i * output.dhw + j] - y.elements[i * y.dhw + j]), 2.0);
                }

                err[i] /= 2.0;
            }
            return err;
        }
        public double CalcErrRootMSE(Tensor4[] x, Tensor4[] y)
        {
            if (x.Length != y.Length) throw new Exception();
            double err = 0;
            for (int i = 0; i < x.Length; i++)
            {
                Calculation(x[i]);
                err += (output - y[i]).ElementsPow(2.0).ToVector().elements.Sum();
                err /= Math.Sqrt((double)output.width * output.height * output.deep);
            }
            err /= (double)x.Length;
            err = Math.Sqrt(err);
            return err;
        }
        public double CalcErrRootMSE(Tensor3[] x, Tensor3[] y)
        {
            if (x.Length != y.Length) throw new Exception();
            Tensor4[] X = new Tensor4[x.Length];
            Tensor4[] Y = new Tensor4[y.Length];
            for(int i = 0; i < x.Length; i++)
            {
                X[i] = x[i].ToTensor4();
                Y[i] = y[i].ToTensor4();
            }
            double err = 0;
            for (int i = 0; i < x.Length; i++)
            {
                Calculation(X[i]);
                err += (output - Y[i]).ElementsPow(2.0).ToVector().elements.Sum();
                err /= Math.Sqrt((double)output.width * output.height * output.deep);
            }
            err /= (double)x.Length;
            err = Math.Sqrt(err);
            return err;
        }
        public double CalcErrRootMSE(Vector[] x, Vector[] y)
        {
            if (x.Length != y.Length) throw new Exception();
            Tensor4[] X = new Tensor4[x.Length];
            Tensor4[] Y = new Tensor4[y.Length];
            for (int i = 0; i < x.Length; i++)
            {
                X[i] = x[i].ToTensor4();
                Y[i] = y[i].ToTensor4();
            }
            double err = 0;
            for (int i = 0; i < x.Length; i++)
            {
                Calculation(X[i]);
                err += (output - Y[i]).ElementsPow(2.0).ToVector().elements.Sum();
                err /= Math.Sqrt((double)output.width * output.height * output.deep);
            }
            err /= (double)x.Length;
            err = Math.Sqrt(err);
            return err;
        }
        public double CalcErrRootMSE(Tensor4 x, Tensor4 y)
        {
            if (x.bs != y.bs) throw new Exception();
            double err = 0;
            Calculation(x);
            for (int i = 0; i < x.bs; i++)
            {
                err += (output - y).ElementsPow(2.0).elements.Sum();
                err /= Math.Sqrt((double)output.dhw);
            }
            err /= (double)x.bs;
            err = Math.Sqrt(err);
            return err;
        }
        public double CalcErrRootMSE(Tensor3[] x, int[] y)
        {
            if (x.Length != y.Length) throw new Exception();
            Tensor4[] X = new Tensor4[x.Length];
            Tensor4[] Y = new Tensor4[y.Length];
            for (int i = 0; i < x.Length; i++)
            {
                X[i] = x[i].ToTensor4();
                Y[i] = new Tensor4(output.width, output.height, output.deep, output.bs);
                Y[i][0, 0, 0, y[i]] = 1.0;
            }
            double err = 0;
            for (int i = 0; i < x.Length; i++)
            {
                Calculation(X[i]);
                err += (output - Y[i]).ElementsPow(2.0).ToVector().elements.Sum();
                err /= Math.Sqrt((double)output.width * output.height * output.deep);
            }
            err /= (double)x.Length;
            err = Math.Sqrt(err);
            return err;
        }
        public double CalcErrRootMSE(Tensor4[] x, int[] y)
        {
            if (x.Length != y.Length) throw new Exception();
            Tensor4[] Y = new Tensor4[y.Length];
            for (int i = 0; i < x.Length; i++)
            {
                Y[i] = new Tensor4(output.width, output.height, output.deep, output.bs);

                if (output.width != 1 && output.height * output.deep == 1)
                        Y[i][0, 0, 0, y[i]] = 1.0;

                else if (output.deep != 1 && output.height * output.width == 1)
                        Y[i][0, y[i], 0, 0] = 1.0;

            }
            double err = 0;
            for (int i = 0; i < x.Length; i++)
            {
                Calculation(x[i]);
                err += (output - Y[i]).ElementsPow(2.0).Sum();
                err /= Math.Sqrt((double)output.width * output.height * output.deep);
            }
            err /= (double)x.Length;
            err = Math.Sqrt(err);
            return err;
        }
        public double CalcAccuracy(Vector[] x, int[] y)
        {
            if (x.Length != y.Length) throw new Exception();
            Tensor4[] X = new Tensor4[x.Length];
            for (int i = 0; i < x.Length; i++)
                X[i] = x[i].ToTensor4();
            
            double accuracy = 0;
            for (int i = 0; i < x.Length; i++)
                accuracy += (Calculation(X[i])[0] == y[i]) ? 1.0 : 0.0;
            
            accuracy /= (double)x.Length;

            return accuracy;
        }
        public double CalcAccuracy(Tensor3[] x, int[] y)
        {
            if (x.Length != y.Length) throw new Exception();
            Tensor4[] X = new Tensor4[x.Length];
            for (int i = 0; i < x.Length; i++)
                X[i] = x[i].ToTensor4();

            double accuracy = 0;
            for (int i = 0; i < x.Length; i++)
                accuracy += (Calculation(X[i])[0] == y[i]) ? 1.0 : 0.0;

            accuracy /= (double)x.Length;

            return accuracy;
        }
        public double CalcAccuracy(Tensor4[] x, int[] y)
        {
            if (x.Length != y.Length) throw new Exception();

            double accuracy = 0;
            for (int i = 0; i < x.Length; i++)
                accuracy += (Calculation(x[i])[0] == y[i]) ? 1.0 : 0.0;

            accuracy /= (double)x.Length;

            return accuracy;
        }
        public double CalcAccuracy(Tensor4[] x, double[] y)
        {
            if (x.Length != y.Length) throw new Exception();

            double accuracy = 0;
            for (int i = 0; i < x.Length; i++)
            {
                Calculation(x[i]);
                accuracy += (output[0, 0, 0, 0] == y[i]) ? 1.0 : 0.0;
            }

            accuracy /= (double)x.Length;

            return accuracy;
        }
        public void SaveWeights(string path)
        {
            using(FileStream stream = new FileStream(path, FileMode.Create))
            {
                Tensor4[] weights = new Tensor4[layers.Count];
                for (int i = 0; i < weights.Length; i++)
                    weights[i] = layers[i].weights;
                BinaryFormatter binForm = new BinaryFormatter();
                binForm.Serialize(stream, weights);
            }
        }
        public void SaveModel(string path)
        {
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                Tensor4[] weights = new Tensor4[layers.Count];
                for (int i = 0; i < weights.Length; i++)
                    weights[i] = layers[i].weights;
                BinaryFormatter binForm = new BinaryFormatter();
                binForm.Serialize(stream, architecture);
                binForm.Serialize(stream, weights);
            }
        }
        public void LoadWeights(string path)
        {
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                BinaryFormatter binForm = new BinaryFormatter();
                Tensor4[] weights = (Tensor4[])binForm.Deserialize(stream);
                for (int i = 0; i < weights.Length; i++)
                    layers[i].SetWeights(weights[i]);
            }
        }
        public static Tensor4[] GetBatches(Tensor3[] x, int batchSize)
        {
            int num = 1;
            for (int i = batchSize + 1; i <= x.Length; i++)
                num *= i;
            num /= (int)matlib.Factorial(x.Length - batchSize);
            Tensor4[] batches = new Tensor4[num];
            Vector[] permuts = matlib.Combinatorics.GetAllPermutations(x.Length);
            return null;
        }
        public static Tensor4[] GetRandBatch(Tensor3[] x, Tensor3[] y, int batchSize, NeuralNet net)
        {
            Tensor4 batchX = new Tensor4(x[0].width, x[0].height, x[0].deep, batchSize);
            Tensor4 batchY = new Tensor4(net.output.width, net.output.height, net.output.deep, batchSize);


            Tensor3[] Y = y;

            int[] indexes = new int[batchSize];

            Random rand = new Random();
            int index = 0;
            while (true)
            {
                indexes[index] = (int)(rand.NextDouble() * x.Length);
                for (int i = 0; i < index; i++)
                {
                    if (indexes[i] == indexes[index])
                        break;
                    if (i == index - 1)
                    {
                        index++;
                        break;
                    }
                }
                if (index == 0) index++;
                if (index == indexes.Length) break;
            }


            for (int i = 0; i < batchSize; i++)
            {
                for (int zInd = 0; zInd < x[indexes[i]].deep; zInd++)
                    for (int yInd = 0; yInd < x[indexes[i]].height; yInd++)
                        for (int xInd = 0; xInd < x[indexes[i]].width; xInd++)
                        {
                            batchX[i, zInd, yInd, xInd] = x[indexes[i]][zInd, yInd, xInd];
                        }

                for (int zInd = 0; zInd < Y[indexes[i]].deep; zInd++)
                    for (int yInd = 0; yInd < Y[indexes[i]].height; yInd++)
                        for (int xInd = 0; xInd < Y[indexes[i]].width; xInd++)
                        {
                            batchY[i, zInd, yInd, xInd] = Y[indexes[i]][zInd, yInd, xInd];
                        }
            }

            return new Tensor4[] { batchX, batchY };
        }
        public static Random rand = new Random();
        public static Tensor4[] GetRandBatch(Tensor3[] x, int[] y, int batchSize, NeuralNet net)
        {
            Tensor4 batchX = new Tensor4(x[0].width, x[0].height, x[0].deep, batchSize);
            Tensor4 batchY = new Tensor4(net.output.width, net.output.height, net.output.deep, batchSize);

            int[] indexes = new int[batchSize];

            int index = 0;
            while (true)
            {
                indexes[index] = (int)(rand.NextDouble() * x.Length);
                for (int i = 0; i < index; i++)
                {
                    if (indexes[i] == indexes[index])
                        break;
                    if (i == index - 1)
                    {
                        index++;
                        break;
                    }
                }
                if (index == 0) index++;
                if (index == indexes.Length) break;
            }


            for (int i = 0; i < batchSize; i++)
            {
                for (int zInd = 0; zInd < x[indexes[i]].deep; zInd++)
                    for (int yInd = 0; yInd < x[indexes[i]].height; yInd++)
                        for (int xInd = 0; xInd < x[indexes[i]].width; xInd++)
                        {
                            batchX[i, zInd, yInd, xInd] = x[indexes[i]][zInd, yInd, xInd];
                        }

                Tensor3 Y = new Tensor3(net.output.width, net.output.height, net.output.deep);
                if(net.output.width != 1)
                Y[0, 0, y[indexes[i]]] = 1.0;
                else Y[y[indexes[i]], 0, 0] = 1.0;

                for (int zInd = 0; zInd < Y.deep; zInd++)
                    for (int yInd = 0; yInd < Y.height; yInd++)
                        for (int xInd = 0; xInd < Y.width; xInd++)
                        {
                            batchY[i, zInd, yInd, xInd] = Y[zInd, yInd, xInd];
                        }

            }

            return new Tensor4[] { batchX, batchY };
        }
        /*
        public static Tensor4[] GetRandBatch(Vector[] x, int[] y, int batchSize, NeuralNet net)
        {
            Tensor4 batchX = new Tensor4(x[0].width, x[0].height, x[0].deep, batchSize);
            Tensor4 batchY = new Tensor4(net.output.width, net.output.height, net.output.deep, batchSize);


            Tensor3[] Y = new Tensor3[y.Length];
            for (int i = 0; i < x.Length; i++)
            {
                Y[i] = new Tensor3(net.output.width, net.output.height, net.output.deep);
                Y[i][0, 0, y[i]] = 1.0;
            }

            int[] indexes = new int[batchSize];

            Random rand = new Random();
            int index = 0;
            while (true)
            {
                indexes[index] = (int)(rand.NextDouble() * x.Length);
                for (int i = 0; i < index; i++)
                {
                    if (indexes[i] == indexes[index])
                        break;
                    if (i == index - 1)
                    {
                        index++;
                        break;
                    }
                }
                if (index == 0) index++;
                if (index == indexes.Length) break;
            }


            for (int i = 0; i < batchSize; i++)
            {
                for (int zInd = 0; zInd < x[indexes[i]].deep; zInd++)
                    for (int yInd = 0; yInd < x[indexes[i]].height; yInd++)
                        for (int xInd = 0; xInd < x[indexes[i]].width; xInd++)
                        {
                            batchX[i, zInd, yInd, xInd] = x[indexes[i]][zInd, yInd, xInd];
                        }

                for (int zInd = 0; zInd < Y[indexes[i]].deep; zInd++)
                    for (int yInd = 0; yInd < Y[indexes[i]].height; yInd++)
                        for (int xInd = 0; xInd < Y[indexes[i]].width; xInd++)
                        {
                            batchY[i, zInd, yInd, xInd] = Y[indexes[i]][zInd, yInd, xInd];
                        }
            }

            return new Tensor4[] { batchX, batchY };
        }
        //*/
        public string GetInfoAboutOutputs()
        {
            string result = "";
            for (int i = 0; i < layers.Count; i++)
                result += "layer: " + layers[i].ToString() + "   width: " + layers[i].output.width + "   height: " + layers[i].output.height + "   deep: " + layers[i].output.deep + "\n";

            return result;
        }
    }
}
