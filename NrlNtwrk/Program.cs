using System;
using System.IO;
using System.Linq;
using ExcelDataReader;

class Program
{
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
        int hiddenSize1 = 10;
        int hiddenSize2 = 5;
        NeuralNetwork nn = new NeuralNetwork(inputSize, hiddenSize1, hiddenSize2, 2);

        // Treniranje mreže.
        double learningRate = 0.005;
        int epochs = inputSize;
        for (int j = 0; j < inputs.Count; j++)
        {

            //Console.WriteLine("vector("+j+")  ----> to Train()");

            nn.Train(inputs[j], targetOutputs[j], learningRate);
        }
        double[] primjer = { 0.9329245, 0.90895582, 0.948307237, 0.961109386, 0.980227584, 0.889202916, 0.822363443, 0.800458921, 0.895494824, 0.943359372, 0.896160404, 0.944518666, 0.998578481, 0.8074368943, 0.914042408, 0.943220914, 0.910998489, 0.989688387, 0.936573072, 0.900655177 };
        Console.WriteLine(nn.Predict(primjer));
        Console.ReadLine();
    }
}

public class NeuralNetwork
{
    private int inputSize;
    private int hiddenSize1;
    private int hiddenSize2;
    private int outputSize;

    private double[,] weights1;
    private double[,] weights2;
    private double[] bias1;
    private double[] bias2;

    public NeuralNetwork(int inputSize, int hiddenSize1, int hiddenSize2, int outputSize)
    {
        this.inputSize = inputSize;
        this.hiddenSize1 = hiddenSize1;
        this.hiddenSize2 = hiddenSize2;
        this.outputSize = outputSize;

        // Inicijalizacija težina i pomaka slučajnim vrijednostima između -1 i 1.
        Random rand = new Random();

        weights1 = new double[inputSize, hiddenSize1];
        for (int i = 0; i < inputSize; i++)
        {
            for (int j = 0; j < hiddenSize1; j++)
            {
                weights1[i, j] = rand.NextDouble() * 2 - 1;
            }
        }

        weights2 = new double[hiddenSize1, hiddenSize2];
        for (int i = 0; i < hiddenSize1; i++)
        {
            for (int j = 0; j < hiddenSize2; j++)
            {
                weights2[i, j] = rand.NextDouble() * 2 - 1;
            }
        }

        bias1 = new double[hiddenSize1];
        for (int i = 0; i < hiddenSize1; i++)
        {
            bias1[i] = rand.NextDouble() * 2 - 1;
        }

        bias2 = new double[hiddenSize2];
        for (int i = 0; i < hiddenSize2; i++)
        {
            bias2[i] = rand.NextDouble() * 2 - 1;
        }
    }

    public (double, double) Predict(double[] input)
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

        double output1 = hidden2[0];
        double output2 = hidden2[1];

        return (output1, output2);
    }

    private double Sigmoid(double x)
    {
        return 1 / (1 + Math.Exp(-x));
    }




    //Treniranje Neuralne Mreže --------------------------------------------------
    public void Train(double[] input, (double, double) targetOutput, double learningRate)
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

        double output1 = hidden2[0];
        double output2 = hidden2[1];

        // Izračunavanje greške.
        double error1 = targetOutput.Item1 - output1;
        double error2 = targetOutput.Item2 - output2;

        // Propagacija greške kroz mrežu i ažuriranje težina i pomaka.
        for (int i = 0; i < hiddenSize1; i++)
        {
            double sum = 0;
            for (int j = 0; j < hiddenSize2; j++)
            {
                sum += error1 * weights2[i, j];
                weights2[i, j] += hidden1[i] * error1 * learningRate;
                bias2[j] += error1 * learningRate;
            }
            double error = sum * hidden1[i] * (1 - hidden1[i]);
            for (int j = 0; j < inputSize; j++)
            {
                weights1[j, i] += input[j] * error * learningRate;
                bias1[i] += error * learningRate;
            }
        }

        for (int i = 0; i < hiddenSize2; i++)
        {
            double error = (targetOutput.Item2 - hidden2[i]) * hidden2[i] * (1 - hidden2[i]);
            for (int j = 0; j < hiddenSize1; j++)
            {
                weights2[j, i] += hidden1[j] * error * learningRate;
                bias2[i] += error * learningRate;
            }
        }
        //Console.WriteLine("h: 1 & 2 " + hidden1[0] + " " + hidden2[0]);
        Console.WriteLine("w: 1 & 2 " + weights1[4,4] + " " + weights2[4,4]);
    }
}




