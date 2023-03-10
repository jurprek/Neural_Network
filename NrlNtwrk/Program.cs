using System;
using System.IO;
using System.Linq;
using ExcelDataReader;
using static OfficeOpenXml.ExcelErrorValue;

class NeuralNetwork
{ 
    // Definicija mreže.
    static int Ulaz = 5;    //  <----------------------------------------------------------------------
    static int Size1 = 4;
    static int Size2 = 4;
    static int Izlaz = 1;

    private int inputSize;
    private int hiddenSize1;
    private int hiddenSize2;
    private int outputSize;

    private double[] output;
    private double[] hidden1;
    private double[] hidden2;
    private double[] bias1;
    private double[] bias2;
    private double[] bias3;

    double[,] weights1 = new double[Ulaz, Size1];
    double[,] weights2 = new double[Size1, Size2];
    double[,] weights3 = new double[Size2, Izlaz];

    string line1;
    string line2;
    string line3;

    string weightsfilePath1 = @"C:\Users\jpreksavec\Desktop\weights1.txt";
    string weightsfilePath2 = @"C:\Users\jpreksavec\Desktop\weights2.txt";
    string weightsfilePath3 = @"C:\Users\jpreksavec\Desktop\weights3.txt";

    string[] lines1 = new string[Size1];
    string[] lines2 = new string[Size2];
    string[] lines3 = new string[Izlaz];

    string fileContent1;
    string fileContent2;
    string fileContent3;



    static void Main(string[] args)
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        // Učitavanje podataka iz Excela.
        string filePath = @"C:\Users\jpreksavec\Desktop\DataSet03.xlsx";
        List<double[]> inputs = new List<double[]>();
        List < double[]> targetOutputs = new List<double[]>();
        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                int w = 0;
                while (reader.Read())
                {
                    // Ignoriraj redove zaglavlja.
                    if (reader.Depth > 12)
                    {
                        double[] input = new double[Ulaz];
                        for (int m = 0; m < Ulaz; m++) {
                            input[m] = (double)reader.GetDouble(m);
                        }
                        
                        double[] output = new double[Izlaz];
                        for (int m = 0; m < Izlaz; m++)
                        {
                            output[m] = reader.GetDouble(m + Ulaz);
                        }
                       
                        targetOutputs.Add(output);
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

        NeuralNetwork nn = new NeuralNetwork(Ulaz, Size1, Size2, Izlaz);

        // start: Treniranje mreže.
        
        int epochs = Ulaz;
        for (int j = 0; j < inputs.Count; j++)
        {
            //if(j%10==0) Console.WriteLine(j);
            double learningRate = 0.30;

            for (int r = 0; r < 1; r++){   //   <---------------------------------------------------------------------------------- broj iteracija po istom uzorku
                if (learningRate < 0.005) learningRate = 0.005;
                else learningRate *= 0.99;
                
                nn.Train(inputs[j], targetOutputs[j], learningRate); /*Console.Write(100 - r);*/              

            }
        }


        double[,] primjer = {
           { 1,1,1,1,1 },
           { 0,0,0,0,0 },
           { -1,-1,-1,-1,-1 },
        };
        
        double[] Row = new double[primjer.GetLength(1)];
        for (int red = 0; red < primjer.GetLength(0); red++) //    <--------------------------------------------------------------BROJ REDOVA (UNESENIH PRIMJERA) !!!!!!!!!!!!!!!!
        {           
            for (int i = 0; i < primjer.GetLength(1); i++)
            {
                Row[i] = primjer[red, i];
            }

            double SigmoidajRedak(double[] x)
            {
                double Sgm = 0;
                for (int i = 0; i < Ulaz; i++)
                {
                    Sgm += x[i];
                }
                return 1 - 1 / (1 + Math.Exp(-Sgm));
            }

            double[] OutputedVals = nn.Predict(Row, 0); //  <------------------------------------------------------------------------ 0 (ili 1 za detalje)
            
            Console.WriteLine(Math.Round(OutputedVals[0]*100, 6) + " %.");
           
            Console.WriteLine(" ---> " + Math.Round(SigmoidajRedak(Row)*100, 2) + " %."); 
            Console.WriteLine("-------------");
        }
        Console.ReadLine();
    }


    private NeuralNetwork(int inputSize, int hiddenSize1, int hiddenSize2, int outputSize)
    {

        double[,] weights1 = LoadWeight("C:/Users/jpreksavec/Desktop/weights1.txt");
        for (int i = 0; i < Ulaz; i++) { for (int j = 0; j < Size1; j++) Console.Write(Math.Round(weights1[j, i], 6) + " "); Console.WriteLine(); }
        Console.WriteLine();
        double[,] weights2 = LoadWeight("C:/Users/jpreksavec/Desktop/weights2.txt");
        for (int i = 0; i < Size1; i++) { for (int j = 0; j < Size2; j++) Console.Write(Math.Round(weights2[j, i], 6) + " "); Console.WriteLine(); }
        Console.WriteLine();
        double[,] weights3 = LoadWeight("C:/Users/jpreksavec/Desktop/weights3.txt");
        for (int i = 0; i < Size2; i++) { for (int j = 0; j < Izlaz; j++) Console.Write(Math.Round(weights3[j, i],6) + " "); Console.WriteLine(); }
        Console.WriteLine();

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

        // Inicijalizacija težina i pomaka
        Random rand = new Random();
 
        /*
        for (int i = 0; i < inputSize; i++)
        {
            for (int j = 0; j < hiddenSize1; j++)
            {
                weights1[i, j] = 0;// rand.NextDouble() * 2 - 1;
            }
        }

        for (int i = 0; i < hiddenSize1; i++)
        {
            for (int j = 0; j < hiddenSize2; j++)
            {
                weights2[i, j] = 0.50;// rand.NextDouble() * 2 - 1;
            }
        }

        for (int i = 0; i < hiddenSize2; i++)
        {
            for (int j = 0; j < outputSize; j++)
            {
                weights3[i, j] = -4.50;// rand.NextDouble() * 2 - 1;
            }
        }
        */

        bias1 = new double[hiddenSize1];
        for (int i = 0; i < hiddenSize1; i++)
        {
            bias1[i] = 0;// rand.NextDouble() * 2 - 1;
        }

        bias2 = new double[hiddenSize2];
        for (int i = 0; i < hiddenSize2; i++)
        {
            bias2[i] = 0;//  rand.NextDouble() * 2 - 1;
        }

        bias3 = new double[outputSize];
        for (int i = 0; i < outputSize; i++)
        {
            bias3[i] = 0;//  rand.NextDouble() * 2 - 1;
        }
    }



    private double Sigmoid(double x)
    {
        return  1.0 / (1.0 + Math.Exp(-5*x));
    }
    private double Gradient(double x)
    {
        return Sigmoid(x) * ( 1 - Sigmoid(x) );
    }



    public double[] Predict(double[] input, int x)
    {
        double s1 = 0;
        double s2 = 0;
        double s3 = 0;

        // Propagacija ulaza kroz mrežu.
        double[] hidden1 = new double[hiddenSize1];
        for (int i = 0; i < hiddenSize1; i++)
        {
            double sum = 0;
            for (int j = 0; j < inputSize; j++)
            {
                sum += input[j] * weights1[j, i];
            }
            hidden1[i] = Gradient(sum + bias1[i]); s1 = Gradient(sum);
        }

        double[] hidden2 = new double[hiddenSize2];
        for (int i = 0; i < hiddenSize2; i++)
        {
            double sum = 0;
            for (int j = 0; j < hiddenSize1; j++)
            {
                sum += hidden1[j] * weights2[j, i];
            }
            hidden2[i] = Gradient(sum + bias2[i]); s2 = Gradient(sum);
        }

        double[] output = new double[outputSize];
        for (int i = 0; i < outputSize; i++)
        {
            double sum = 0;
            for (int j = 0; j < hiddenSize2; j++)
            {
                sum += hidden2[j] * weights3[j, i];
            }
            output[i] = Gradient(sum + bias3[i]); s3 = Gradient(sum);
        }

        Console.WriteLine(s1 + " " + s2 + " " + s3);

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
        }
        
        File.WriteAllText(weightsfilePath3, fileContent3);
        File.WriteAllText(weightsfilePath2, fileContent2);
        File.WriteAllText(weightsfilePath1, fileContent1);
        
        return output;
    }



    //Treniranje Neuralne Mreže --------------------------------------------------
    public void Train(double[] input, double[] targetOutput, double learningRate)
    {
        // Propagacija unaprijed

        double[] hidden1 = new double[hiddenSize1];
        for (int i = 0; i < hiddenSize1; i++)
        {
            double sum = 0;
            for (int j = 0; j < inputSize; j++)
            {
                sum += input[j] * weights1[j, i];
            }
            hidden1[i] = Gradient(sum + bias1[i]);
        }

        double[] hidden2 = new double[hiddenSize2];
        for (int i = 0; i < hiddenSize2; i++)
        {
            double sum = 0;
            for (int j = 0; j < hiddenSize1; j++)
            {
                sum += hidden1[j] * weights2[j, i];
            }
            hidden2[i] = Gradient(sum + bias2[i]);
        }

        double[] output = new double[outputSize];
        for (int i = 0; i < outputSize; i++)
        {
            double sum = 0;
            for (int j = 0; j < hiddenSize2; j++)
            {
                sum += hidden2[j] * weights3[j, i];
            }
            output[i] = Gradient(sum + bias3[i]);
        }


        // Propagacija unatrag
        double[] outputError = new double[outputSize];
        for (int j = 0; j < outputSize; j++)
        {
            outputError[j] = (targetOutput[j] - output[j]) * output[j] * (1 - output[j]);
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
        for (int j = 0; j < outputSize; j++)
        {
            for (int i = 0; i < hiddenSize2; i++)
            {
                weights3[i, j] = Math.Round(weights3[i, j] + learningRate * outputError[j] * output[j], 6);
                line3 += weights3[i, j] + " ";
            }
            bias3[j] += learningRate * outputError[j];
            lines3[j] = line3.TrimEnd();
            line3 = "";
        }

        for (int j = 0; j < hiddenSize2; j++)
        {
            for (int i = 0; i < hiddenSize1; i++)
            {
                weights2[i, j] = Math.Round(weights2[i, j] + learningRate * hidden2Error[j] * hidden2[j], 6);
                line2 += weights2[i, j] + " ";
            }
            bias2[j] += learningRate * hidden2Error[j];          
            lines2[j] = line2.TrimEnd();
            line2 = "";
        }

        for (int j = 0; j < hiddenSize1; j++)
        {
            for (int i = 0; i < inputSize; i++)
            {
                weights1[i, j] = Math.Round(weights1[i, j] + learningRate * hidden1Error[j] * hidden1[j], 6);
                line1 += weights1[i, j] + " ";
            }
            bias1[j] += learningRate * hidden1Error[j];
            lines1[j] = line1.TrimEnd();
            line1 = "";
        }

        fileContent1 = string.Join(Environment.NewLine, lines1);
        fileContent2 = string.Join(Environment.NewLine, lines2);
        fileContent3 = string.Join(Environment.NewLine, lines3);
    }




    static double[,] LoadWeight(string inputFilePath)
    {
        // Read the input file
        string[] inputLines = File.ReadAllLines(inputFilePath);

        // Determine the size of the matrix
        int numRows = inputLines.Length;
        int numCols = inputLines[0].Split(' ').Length;

        // Initialize the matrix
        double[,] matrix = new double[numRows, numCols];

        // Parse the input lines and fill the matrix
        for (int i = 0; i < numRows; i++)
        {
            string[] lineValues = inputLines[i].Split(' ');
            for (int j = 0; j < numCols; j++)
            {
                matrix[i, j] = double.Parse(lineValues[j]);
            }
        }

        return matrix;
    }
}