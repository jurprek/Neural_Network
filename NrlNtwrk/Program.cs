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
                while (reader.Read())
                {
                    // Ignoriraj prvi red koji je zaglavlje.
                    if (reader.Depth > 0)
                    {
                        double[] input = new double[2];
                        input[0] = (double)reader.GetDouble(0);
                        input[1] = (double)reader.GetDouble(1);
                        double output1 = reader.GetDouble(2);
                        double output2 = reader.GetDouble(3);
                        targetOutputs.Add((output1, output2));
                        inputs.Add(input);
                    }
                }
            }
        }

        // Definicija mreže.
        int inputSize = 20;
        int hiddenSize1 = 10;
        int hiddenSize2 = 5;
        NeuralNetwork nn = new NeuralNetwork(inputSize, hiddenSize1, hiddenSize2, 0);

        // Treniranje mreže.
        double learningRate = 0.1;
        int epochs = inputSize+2;
        for (int i = 0; i < epochs; i++)
        {
            for (int j = 0; j < inputs.Count; j++)
            {
                Console.WriteLine(i+" "+j+" "+ inputs[j].ToString());
                //nn.Train(inputs[j], targetOutputs[j], learningRate);
            }
        }
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

    public (double, double) Predict(int[] input)
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




    //Treniranje Neuralne Mreže (20,10,5,2)--------------------------------------------------
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
    }
}




