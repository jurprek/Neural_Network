using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using ExcelDataReader;

class NeuralNetwork
{

    private int inputSize;
    private int hiddenSize1;
    private int hiddenSize2;
    private int outputSize;

    private double[] output;
    private double[,] weights1;
    private double[,] weights2;
    private double[,] weights3;
    private double[] hidden1;
    private double[] hidden2;
    private double[] bias1;
    private double[] bias2;
    private double[] bias3;


    static void Main(string[] args)
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        // Učitavanje podataka iz Excela.
        string filePath = @"C:\Users\jpreksavec\Desktop\DataSet01.xlsx";
        List<double[]> inputs = new List<double[]>();
        List<(double, double)> targetOutputs = new List<(double, double)>();
        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                int w = 0;
                while (reader.Read())
                {
                    // Ignoriraj prvi red koji je zaglavlje.
                    if (reader.Depth > 0)
                    {
                        double[] input = new double[20];
                        for (int m = 0; m < 20; m++) {
                            input[m] = (double)reader.GetDouble(m);
                        }
                        double output1 = reader.GetDouble(20);
                        double output2 = reader.GetDouble(21);
                        targetOutputs.Add((output1, output2));
                        inputs.Add(input);
                       /* for (int r  = 0; r < 20; r++) {
                            Console.WriteLine(w + ". " + input[r].ToString()); w++;
                        }
                       */
                    }
                    //Console.WriteLine(w+". DataSet01.xlsx trainer loaded.");
                    w++;
                }
            }
        }

        // Definicija mreže.
        int inputSize = 20;
        int hiddenSize1 = 16;
        int hiddenSize2 = 8;
        NeuralNetwork nn = new NeuralNetwork(inputSize, hiddenSize1, hiddenSize2, 2);

        // Treniranje mreže.
        double learningRate = 0.05;
        int epochs = inputSize;
        for (int j = 0; j < inputs.Count; j++)
        {

            //Console.WriteLine("vector("+j+")  ----> to Train()");
            //learningRate *= 0.999;
            nn.Train(inputs[j], targetOutputs[j], learningRate);
        }

        double[,] primjer = {
            { 0.1490, 0.5099, 0.37661, -0.6192, -0.59174, -0.00809, 0.53247, -0.515077, 0.407493, -0.9106, -0.3370, -0.0476199, -0.22796, 0.8017, 0.24172, 0.4203, 0.04533, 0.540929, -0.933125, 0.539305 }, // 81 %
            { -0.4649, 0.17665, -0.2107, -0.08921, -0.5958, 0.362617, -0.55707, -0.10903, 0.281384, -0.81309, -0.13954, -0.84013, 0.856531, -0.79735, 0.362592, -0.57548, -0.9549, 0.940136, 0.306131, 0.32432 },
            { -0.79381, 0.561917, 0.124387, 0.803938, -0.22372, -0.78332, 0.896787, -0.97826, 0.990252, 0.61365, 0.220648, 0.120601, 0.521923, 0.765011, 0.035289, -0.88796, -0.44779, -0.02094, -0.6047, 0.32432  },
            { -0.2333, 0.576397, -0.73647, -0.18417, 0.698457, 0.471181, -0.22578, -0.40393, -0.32579, -0.65091, -0.87646, 0.526412, -0.15106, 0.202285, -0.52103, 0.061123, -0.05817, 0.668332, 0.733007, 0.82432  },
            { -0.10774, 0.235272, 0.338, 0.664694, -0.54434, 0.880816, 0.142886, 0.263006, 0.639518, 0.342628, -0.61323, 0.909403, -0.47817, -0.80061, 0.750915, -0.90898, 0.835737, 0.329355, 0.217662, 0.42432  },
            { 0.036086, -0.33671, -0.32175, 0.622437, 0.158403, 0.408799, 0.432763, 0.187294, -0.0939, -0.15798, -0.17026, -0.53457, 0.15949, -0.88593, -0.95954, 0.456371, 0.389603, 0.993939, -0.11227, 0.52432  },
            { -0.25784, -0.59175, 0.119169, 0.656153, -0.66653, -0.72966, -0.465, 0.367703, 0.51204, 0.847283, -0.01865, 0.828711, -0.3578, 0.919156, -0.8565, 0.981319, 0.842529, -0.4037, -0.24169, 0.32432  },
            { 0.552131, -0.1087, -0.77359, 0.048449, 0.64386, -0.4921, 0.77737, 0.551631, -0.45682, -0.36398, 0.301398, -0.31909, 0.369391, 0.632753, 0.141006, -0.85041, 0.322267, -0.42969, -0.34635, 0.7432  },
            { -0.21706, -0.66031, -0.69375, 0.130809, -0.72796, -0.52197, 0.930021, -0.08802, 0.668399, -0.54429, -0.4374, -0.07783, -0.18804, -0.39991, 0.621346, 0.586478, 0.586427, -0.23107, 0.957595, 0.32432  },
            { -0.03403, -0.99981, 0.219334, 0.641439, 0.901445, -0.68821, 0.444771, 0.423315, 0.455698, 0.536018, -0.285, 0.408362, 0.47575, -0.81967, 0.445074, 0.811256, -0.74934, -0.24928, 0.4464, 0.9432  },
            { -0.66087, 0.69755, 0.06695, 0.145548, 0.650759, 0.906018, 0.030859, 0.809533, 0.944963, 0.071479, 0.595646, 0.649483, -0.80164, 0.616642, 0.407233, -0.38305, -0.68296, 0.987899, 0.460225, 0.32432  },
            { 0.337971, 0.245767, 0.568216, 0.103626, 0.022881, 0.629238, 0.057159, -0.52581, -0.85246, 0.80944, -0.76616, 0.356659, 0.154284, 0.312209, -0.65126, 0.116139, -0.23405, -0.36845, 0.707301, 0.32432  },
            { -0.08269, 0.236512, -0.83991, 0.484702, -0.41903, 0.26302, 0.656452, -0.74619, 0.972138, -0.90314, 0.523514, 0.775201, 0.356399, -0.67294, -0.16301, -0.01333, 0.047145, 0.214818, 0.834488, 0.32432  },
            { -0.73092, 0.62425, 0.899906, -0.41323, 0.995189, -0.14591, 0.206167, -0.31111, -0.44597, -0.39743, 0.174527, 0.769217, 0.558273, 0.079125, 0.998364, 0.37562, 0.050601, 0.052906, 0.347405, 0.3432  },
            { -0.82224, 0.121999, 0.802807, -0.74203, 0.553616, 0.15354, -0.56896, -0.54586, -0.92024, -0.50605, 0.059794, -0.2063, -0.14541, 0.75348, 0.827601, 0.709394, -0.54999, 0.640473, -0.3461, 0.32432  },
            { 0.247878, -0.06025, 0.563484, -0.02466, -0.02864, 0.339897, -0.30324, 0.902218, -0.39939, -0.56401, -0.49247, -0.32185, -0.87182, -0.0938, 0.754163, 0.025399, 0.478451, 0.690717, -0.79642, 0.32432  },
            { -0.03875, -0.70535, 0.547687, 0.103545, -0.21147, -0.09457, 0.755469, -0.86616, -0.05956, 0.477182, -0.07912, 0.25583, -0.75239, -0.61162, 0.519305, -0.11073, 0.735064, -0.31386, 0.572547, 0.5432  },
            { 0.050554, 0.606938, 0.07877, 0.069146, -0.34599, -0.15907, -0.66859, -0.68299, -0.80791, 0.830693, 0.371897, 0.612181, 0.136414, -0.34566, 0.381957, -0.98007, -0.26049, 0.321411, 0.900027, 0.3732  },
            { -0.52123, -0.15823, 0.972796, -0.3082, 0.385309, 0.074176, 0.949089, -0.27146, -0.84841, -0.17676, 0.394822, 0.816735, -0.03195, 0.558883, 0.414815, -0.56614, 0.194809, 0.928439, -0.76369, 0.8432  },
            { 0.809049, 0.790673, -0.6865, 0.072126, 0.790721, 0.628, -0.9436, 0.739136, 0.018214, 0.841743, 0.378866, 0.702843, -0.34385, -0.14602, -0.68473, -0.36486, 0.317706, 0.104182, 0.059027, 0.82432  },
            { 0.268659, 0.208261, -0.10206, -0.29665, -0.26104, -0.02867, -0.12786, 0.169592, -0.75504, -0.72001, -0.8154, -0.49283, 0.702987, 0.639801, -0.62413, -0.97538, -0.48814, -0.7559, -0.92069, 0.62432  },
            { -0.49649, 0.531759, 0.00339, 0.564711, -0.0274, 0.566745, 0.717493, -0.93083, 0.325328, 0.498622, -0.86142, -0.44236, 0.709072, -0.18349, -0.23398, 0.098027, -0.76872, -0.86023, -0.73155, 0.32432  },
            { -0.46174, 0.840871, -0.04266, -0.20971, 0.428911, -0.79431, 0.518025, -0.40673, -0.41951, 0.323136, -0.71522, 0.34532, 0.525408, -0.3611, 0.140307, 0.315782, -0.44164, 0.688955, -0.07818, 0.4432  },
            { -0.75149, -0.03446, -0.46465, 0.895676, -0.25792, -0.8635, 0.801397, -0.77974, -0.51353, 0.747268, 0.893764, 0.655583, 0.023009, -0.57337, -0.66504, -0.15326, -0.82892, 0.175634, 0.56008, 0.32432  },
            { -0.424, -0.98564, -0.79799, -0.4204, -0.09863, -0.49144, -0.99619, 0.452037, -0.96724, -0.60653, 0.678453, 0.18072, 0.688977, 0.291312, -0.41557, 0.958234, 0.837904, -0.08237, -0.67638, 0.54332  },
            { -0.01467, -0.58221, -0.96839, 0.580159, 0.987551, 0.153402, -0.95039, -0.76795, 0.307826, 0.468137, 0.246291, -0.93677, -0.3643, 0.900028, 0.252059, -0.37141, 0.766939, -0.45537, 0.716433, 0.672432  },
            { 0.582646, 0.900091, -0.90029, 0.941691, 0.205893, -0.01759, 0.07224, 0.479345, 0.844189, -0.96809, 0.302269, -0.60599, -0.40992, 0.301607, 0.520916, -0.5637, -0.38026, 0.761027, 0.719132, 0.3632  },
            { 0.586419, 0.736896, -0.89117, -0.8606, -0.4373, -0.1109, -0.45315, -0.98498, 0.720525, 0.881272, -0.54646, 0.641219, -0.54881, -0.81466, 0.473086, -0.09294, -0.87579, -0.72964, -0.46366, 0.387832  },
            { -0.54269, -0.51594, 0.745287, -0.78776, 0.517395, 0.055746, 0.8257, -0.34345, 0.461029, -0.04701, 0.045643, 0.154997, 0.363292, 0.889401, -0.11138, -0.56752, -0.38609, 0.78256, -0.05412, 0.324432  },
            { -0.08634, -0.15228, -0.25055, 0.021745, 0.775755, -0.46115, 0.499051, -0.44277, -0.30275, -0.0225, 0.804136, 0.203773, 0.551783, 0.202415, 0.309453, 0.64167, 0.888467, 0.514989, -0.71555, 0.12432  },
            { -0.90238, 0.927529, -0.9887, -0.71297, -0.27255, -0.61092, 0.283927, -0.74259, -0.47112, -0.72998, -0.67693, -0.94842, 0.622748, 0.11031, -0.64279, -0.31466, -0.84094, 0.121546, 0.280881, 0.82432  }
        };


        //double[] primjer = { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }; // 100 %
        //double[] primjer = { -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1 }; // 0.0%

        int red;
        for(int t = 0; t < 31; t++) {
            red = t;
            double[] Row = new double[primjer.GetLength(1)];
            for (int i = 0; i < primjer.GetLength(1); i++)
            {
                Row[i] = primjer[red, i];
            }

            double SigmoidajRedak(double[] x)
            {
                double Sgm = 0;
                for(int i = 0; i < 20; i++)
                {
                    Sgm += x[i]; 
                }
                return 1-1/(1+Math.Exp(-Sgm));
            }
        
            Console.WriteLine(Math.Round(nn.Predict(Row, 0)[1] * 100, 3) + " %    -->   " + Math.Round(SigmoidajRedak(Row)*100,3) + " %.");
        }
        Console.ReadLine();
    }


    private NeuralNetwork(int inputSize, int hiddenSize1, int hiddenSize2, int outputSize)
    {
        this.inputSize = inputSize;
        this.hiddenSize1 = hiddenSize1;
        this.hiddenSize2 = hiddenSize2;
        this.outputSize = outputSize;

        hidden1 = new double[hiddenSize1];
        for (int i = 0; i < hiddenSize1; i++)
        {
            hidden1[i] = 0; // set each element to 0
        }
        hidden2 = new double[hiddenSize2];
        for (int i = 0; i < hiddenSize2; i++)
        {
            hidden2[i] = 0; // set each element to 0
        }

        // Inicijalizacija težina i pomaka slučajnim vrijednostima između -1 i 1.
        Random rand = new Random();

        weights1 = new double[inputSize, hiddenSize1];
        for (int i = 0; i < inputSize; i++)
        {
            for (int j = 0; j < hiddenSize1; j++)
            {
                weights1[i, j] = 0.5;// rand.NextDouble() * 2 - 1;
            }
        }

        weights2 = new double[hiddenSize1, hiddenSize2];
        for (int i = 0; i < hiddenSize1; i++)
        {
            for (int j = 0; j < hiddenSize2; j++)
            {
                weights2[i, j] = 0.5;// rand.NextDouble() * 2 - 1;
            }
        }

        weights3 = new double[hiddenSize2, outputSize];
        for (int i = 0; i < hiddenSize2; i++)
        {
            for (int j = 0; j < outputSize; j++)
            {
                weights3[i, j] = 0.5;// rand.NextDouble() * 2 - 1;
            }
        }

        bias1 = new double[hiddenSize1];
        for (int i = 0; i < hiddenSize1; i++)
        {
            bias1[i] = 0;// rand.NextDouble() * 2 - 1;
        }

        bias2 = new double[hiddenSize2];
        for (int i = 0; i < hiddenSize2; i++)
        {
            bias2[i] = 0;// rand.NextDouble() * 2 - 1;
        }

        bias3 = new double[outputSize];
        for (int i = 0; i < outputSize; i++)
        {
            bias3[i] = 0;// rand.NextDouble() * 2 - 1;
        }
    }



    private double Sigmoid(double x)
    {
        return 1.0 / (1.0 + Math.Exp(-1*x));
    }
    private double Gradient(double x)
    {
        return 0;
    }



    public double[] Predict(double[] input, int x)
    {
        // Propagacija ulaza kroz mrežu.
        double[] hidden1 = new double[hiddenSize1];
        for (int i = 0; i < hiddenSize1; i++)
        {
            double sum = 0;
            for (int j = 0; j < inputSize; j++)
            {
                sum += input[j] * weights1[j, i];
            }
            hidden1[i] = Sigmoid(sum + bias1[i]);
        }

        double[] hidden2 = new double[hiddenSize2];
        for (int i = 0; i < hiddenSize2; i++)
        {
            double sum = 0;
            for (int j = 0; j < hiddenSize1; j++)
            {
                sum += hidden1[j] * weights2[j, i];
            }
            hidden2[i] = Sigmoid(sum + bias2[i]);
        }

        double[] output = new double[outputSize];
        for (int i = 0; i < outputSize; i++)
        {
            double sum = 0;
            for (int j = 0; j < hiddenSize2; j++)
            {
                sum += hidden2[j] * weights3[j, i];
            }
            output[i] = Sigmoid(sum + bias3[i]);
        }

        // Izračun pogreške izlaza.
        double[] targetOutput = { 0, 0 };
        double[] outputError = new double[outputSize];

        for (int i = 0; i < outputSize; i++)
        {
            outputError[i] = (targetOutput[i] - output[i]) * output[i] * (1 - output[i]);
        }

        if (x == 1) {
            Console.WriteLine("Hidden layer 1 values:");
            for (int i = 0; i < hiddenSize1; i++)
            {
                Console.WriteLine($"Node {i + 1}: {hidden1[i]}");
            }

            Console.WriteLine("\nHidden layer 2 values:");
            for (int i = 0; i < hiddenSize2; i++)
            {
                Console.WriteLine($"Node {i + 1}: {hidden2[i]}");
            }

            Console.WriteLine("\nOutput values:");
            for (int i = 0; i < outputSize; i++)
            {
                Console.WriteLine($"Node {i + 1}: {output[i]}");
            }

            Console.WriteLine("\nOutput error:");
            for (int i = 0; i < outputSize; i++)
            {
                Console.WriteLine($"Node {i + 1}: {outputError[i]}");
            }
        }
        return output;
    }



    //Treniranje Neuralne Mreže --------------------------------------------------
    public void Train(double[] input, (double, double) targetOutput, double learningRate)
    {
        // Propagacija unaprijed
        for (int i = 0; i < hiddenSize1; i++)
        {
            double sum = 0;
            for (int j = 0; j < inputSize; j++)
            {
                sum += input[j] * weights1[j, i];
            }
            hidden1[i] = Sigmoid(sum + bias1[i]);
        }
        for (int i = 0; i < hiddenSize2; i++)
        {
            double sum = 0;
            for (int j = 0; j < hiddenSize1; j++)
            {
                sum += hidden1[j] * weights2[j, i];
            }
            hidden2[i] = Sigmoid(sum + bias2[i]);
        }
        for (int i = 0; i < outputSize; i++)
        {
            double sum = 0;
            for (int j = 0; j < hiddenSize2; j++)
            {
                sum += hidden2[j] * weights3[j, i];
            }
            output = new double[outputSize];
            output[i] = Sigmoid(sum + bias3[i]);
        }

        // Propagacija unatrag
        double[] outputError = new double[outputSize];
        for (int i = 0; i < outputSize; i++)
        {
            outputError[i] = (targetOutput.Item1 - output[i]) * output[i] * (1 - output[i]);
        }
        double[] hidden2Error = new double[hiddenSize2];
        for (int i = 0; i < hiddenSize2; i++)
        {
            double sum = 0;
            for (int j = 0; j < outputSize; j++)
            {
                sum += outputError[j] * weights3[i, j];
            }
            hidden2Error[i] = sum * hidden2[i] * (1 - hidden2[i]);
        }
        double[] hidden1Error = new double[hiddenSize1];
        for (int i = 0; i < hiddenSize1; i++)
        {
            double sum = 0;
            for (int j = 0; j < hiddenSize2; j++)
            {
                sum += hidden2Error[j] * weights2[i, j];
            }
            hidden1Error[i] = sum * hidden1[i] * (1 - hidden1[i]);
        }

        // Ažuriranje težina i pomaka
        for (int i = 0; i < outputSize; i++)
        {
            for (int j = 0; j < hiddenSize2; j++)
            {
                weights3[j, i] += learningRate * outputError[i] * hidden2[j];
            }
            bias3[i] += learningRate * outputError[i];
        }
        for (int i = 0; i < hiddenSize2; i++)
        {
            for (int j = 0; j < hiddenSize1; j++)
            {
                weights2[j, i] += learningRate * hidden2Error[i] * hidden1[j];
            }
            bias2[i] += learningRate * hidden2Error[i];
        }
        for (int i = 0; i < hiddenSize1; i++)
        {
            for (int j = 0; j < inputSize; j++)
            {
                weights1[j, i] += learningRate * hidden1Error[i] * input[j];
            }
            bias1[i] += learningRate * hidden1Error[i];
        }
    }
}




