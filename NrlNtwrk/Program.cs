using System;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using ExcelDataReader;
using static OfficeOpenXml.ExcelErrorValue;

partial class NeuralNetwork
{
    private static double[] output = new double[OUTPUT];
    private static double[] hidden1 = new double[Size1];
    private static double[] hidden2 = new double[Size2];
    private static double[] bias1 = new double[Size1];
    private static double[] bias2 = new double[Size2];
    private static double[] bias3 = new double[OUTPUT];

    private static double[,] weights1 = new double[INPUT, Size1];
    private static double[,] weights2 = new double[Size1, Size2];
    private static double[,] weights3 = new double[Size2, OUTPUT];

    private static string line1 = "";
    private static string line2 = "";
    private static string line3 = "";
    private static string bline1 = "";
    private static string bline2 = "";
    private static string bline3 = "";

    private static string[] lines1 = new string[Size1];
    private static string[] lines2 = new string[Size2];
    private static string[] lines3 = new string[OUTPUT];

    private static string fileContent1 = "";
    private static string fileContent2 = "";
    private static string fileContent3 = "";

    private static double ErrDiff = 999999999;
    private static int r = 0;
    private static int p;
    private static int t = 0;

    static void Main(string[] args)
    {
     
        List<double[]> inputs = new List<double[]>();
        List<double[]> targetOutputs = new List<double[]>();   // Kreiraj matrice težina
        if (CycleNumber > 0)
        {
            //Kreiraj weights1.txt
            CreateMatrix(INPUT, Size1, 1);

            //Kreiraj weights1.txt
            CreateMatrix(Size1, Size2, 2);

            //Kreiraj weights1.txt
            CreateMatrix(Size2, OUTPUT, 3);

            //Kreiraj Biases.txt
            CreateBiases(Size1, Size2, OUTPUT);

            System.Globalization.CultureInfo customCulture = new System.Globalization.CultureInfo("en-US");
        System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        using (var reader = new StreamReader(DatafilePath))
        {
            int rowNumber = 0;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = new string[0];
                if (line != null)
                {
                    values = line.Split(',');
                }

                // Ignoriraj redove zaglavlja.
                if (rowNumber > 0)
                {
                    double[] input = new double[INPUT];
                    for (int m = 0; m < INPUT; m++)
                    {
                        input[m] = double.Parse(values[m], customCulture);
                    }

                    double[] output = new double[OUTPUT];
                    for (int m = 0; m < OUTPUT; m++)
                    {
                        output[m] = double.Parse(values[m + INPUT], customCulture);
                    }

                    targetOutputs.Add(output);
                    inputs.Add(input);
                }

                rowNumber++;
                p = rowNumber;
            }
        }
 }
        //Učitavanje matrica težina i biasa

        string fileContents1 = System.IO.File.ReadAllText(weightsfilePath1);
        double[][] weightsArray1 = fileContents1
                .Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(val => Double.Parse(val, CultureInfo.InvariantCulture))
                    .ToArray())
                .ToArray();

           for (int i = 0; i < weightsArray1.Length; i++)
           {
                for (int j = 0; j < weightsArray1[i].Length; j++)
                {
                    weights1[i, j] = weightsArray1[i][j];
                }
           }

            string fileContents2 = System.IO.File.ReadAllText(weightsfilePath2);
            double[][] weightsArray2 = fileContents2
                .Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(val => Double.Parse(val, CultureInfo.InvariantCulture))
                    .ToArray())
                .ToArray();

            for (int i = 0; i < weightsArray2.Length; i++)
            {
                for (int j = 0; j < weightsArray2[i].Length; j++)
                {
                    weights2[i, j] = weightsArray2[i][j];
                }
            }

            string fileContents3 = System.IO.File.ReadAllText(weightsfilePath3);
            double[][] weightsArray3 = fileContents3
                .Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(val => Double.Parse(val, CultureInfo.InvariantCulture))
                    .ToArray())
                .ToArray();

           for (int i = 0; i < weightsArray3.Length; i++)
           {
                for (int j = 0; j < weightsArray3[i].Length; j++)
                {
                    weights3[i, j] = weightsArray3[i][j];
                }
           }

            //Load Biases
            string path1 = Path.Combine("Weights", "Biases.txt");
            string firstLine = File.ReadLines(path1).First();

            bias1 = firstLine.Split(' ')
                .Select(s => Double.Parse(s, CultureInfo.InvariantCulture))
                .ToArray();

            string path2 = Path.Combine("Weights", "Biases.txt");
            string secondLine = File.ReadLines(path2).Skip(1).First();

            bias2 = secondLine.Split(' ')
                .Select(s => Double.Parse(s, CultureInfo.InvariantCulture))
                .ToArray();


            string path3 = Path.Combine("Weights", "Biases.txt");
            string thirdLine = File.ReadLines(path3).Skip(2).First();

            bias3 = thirdLine.Split(' ')
                .Select(s => Double.Parse(s, CultureInfo.InvariantCulture))
                .ToArray();

        //Inicijalizacija Neuralne Mreže ----------------------------------
        NeuralNetwork nn = new NeuralNetwork(INPUT, Size1, Size2, OUTPUT);//
        //-----------------------------------------------------------------

        // start: Treniranje mreže.
        if (CycleNumber > 0){ 
        int epochs = INPUT;
        for (int j = 0; j < inputs.Count; j++)
        {
            double learningRate = 0.50;
            bool bjeg = false;

            for (r = 0; r < CycleNumber; r++)
            {   //   <---------------------------------------------------------------------------------- broj iteracija po istom uzorku

                if (learningRate < 0.01) learningRate = 0.01;
                else learningRate *= 0.995;
                try
                {
                    nn.Train(inputs[j], targetOutputs[j], learningRate, weights1, weights2, weights3);
                }
                catch (Exception)
                {
                    Console.WriteLine(">>> Overfitting! <<<"); Console.WriteLine();
                    bjeg = true;
                    break;
                }
                t++;
            }
            if (bjeg) break;
        }
        }

        //Testiraj na ovom uzorku
        double[,] primjer = {
            { 1,0.781640136615092,0.377540668798145,0.401312339887548,1 },   //11
            { 1,0.423847173228711,0.377540668798145,0.450166002687522,1 },   //11
            { 1,0.447321223770207,0.377540668798145,0.466715961748869,0 },   //10
            { 0,0.331812227831834,0.377540668798145,0.841130895119085,0 },   //10
            { 0,0.322840161187185,0.377540668798145,0,0 },                   //00
            { 0,0.813847686078649,0.377540668798145,0.450166002687522,1 },   //00
            { 1,0.813847686078649,0.377540668798145,0.660756368765817,1 },   //11
            { 1,0.441005324252225,0.377540668798145,0.516660496569411,1 },   //11
            { 0,0.438418494318759,0.377540668798145,0.549833997312478,1 },   //00
            { 0,0.383561639011043,0.377540668798145,0.558070105873953,0 },   //10
            { 1,0.383561639011043,0.377540668798145,0.637993670660984,0 },   //10
            { 0,0.331812227831834,0.377540668798145,0.466715961748869,1 },   //10
            { 1,0.781640136615092,0.377540668798145,0.598687660112452,1 },   //11
            { 1,0.771674749039444,0.377540668798145,0.491667438185881,0 },   //10
            { 0,0.323004143761477,0.377540668798145,0.4584295167832,1 },     //00
            { 0,0.477712778307445,0.377540668798145,0.4584295167832,1 },     //10
            { 1,0.477712778307445,0.377540668798145,0.409346717299114,1 },   //11
            { 1,0.58815246782807,0.377540668798145,0.5415704832168,0 },      //10
            { 0,0.324208011232084,0.377540668798145,0.483339503430589,0 },   //10
            { 0,0.344117158792186,0.377540668798145,0.622459331201855,0 },   //00
            { 0,0.33403307324818,0.377540668798145,0.582570206462315,1 },    //10
            { 1,0.656710011331464,0.377540668798145,0.5,0 },                 //10
            { 1,0.324208011232084,0.377540668798145,0.71775105695774,0 },    //10
            { 0,0.324208011232084,0.377540668798145,0.598687660112452,0 },   //00
            { 1,0.835311651549435,0.377540668798145,0.622459331201855,1 },   //11
            { 1,0.389360766050778,0.377540668798145,0.433725605804561,0, },  //10
            { 0,0.324208011232084,0.377540668798145,0,0 },                   //10
            { 0,0.332921722904375,0.377540668798145,0.590653282700886,0 },   //00
            { 0,0.378706522667112,0.377540668798145,0.645656306225795,1 },   //00
            { 0,0.353429090902552,0.377540668798145,0,1 },                   //00
            { 1,0.326778462045695,0.377540668798145,0.61459449826755,1 },    //11
            { 1,0.381023088331113,0.377540668798145,0.724454595424802,0 },   //10
            { 1,0.441005324252225,0.377540668798145,0.731058578630005,1 },   //11
            { 1,0.585404569129465,0.377540668798145,0.590653282700886,1 },   //11
            { 0,0.324208011232084,0.377540668798145,0.622459331201855,0 }    //00
        };
        double[] Row = new double[primjer.GetLength(1)];
        for (int red = 0; red < primjer.GetLength(0); red++)
        {
            for (int i = 0; i < primjer.GetLength(1); i++)
            {
                Row[i] = primjer[red, i];
            }

            double SigmoidajRedak(double[] x)
            {
                double Sgm = 0;
                for (int i = 0; i < INPUT; i++)
                {
                    Sgm += x[i];
                }
                return 1 - 1 / (1 + Math.Exp(-Sgm));
            }

            double[] OutputedVals = nn.Predict(Row, weights1, weights2, weights3, bias1, bias2, bias3);

            Console.WriteLine(Math.Round(OutputedVals[0] * 100, 6) + " %.");

            //Console.WriteLine(" ---> " + Math.Round(SigmoidajRedak(Row) * 100, 2) + " %.");
            Console.WriteLine("-------------");
        }
        Console.WriteLine("Done.");
        Console.ReadLine();
    }

    private double[] Predict(double[] input, double[,] weights1, double[,] weights2, double[,] weights3, double[] bias1, double[] bias2, double[] bias3)
    {
        // Propagacija INPUTa kroz mrežu.
        for (int i = 0; i < Size1; i++)
        {
            double sum = 0;
            for (int j = 0; j < INPUT; j++)
            {
                sum += input[j] * weights1[j, i];
            }
            hidden1[i] = Sigmoid(sum + bias1[i]);
        }

        for (int i = 0; i < Size2; i++)
        {
            double sum = 0;
            for (int j = 0; j < Size1; j++)
            {
                sum += hidden1[j] * weights2[j, i];
            }
            hidden2[i] = Sigmoid(sum + bias2[i]);
        }

        for (int i = 0; i < OUTPUT; i++)
        {
            double sum = 0;
            for (int j = 0; j < Size2; j++)
            {
                sum += hidden2[j] * weights3[j, i];
            }
            output[i] = Sigmoid(sum + bias3[i]);
        }
        return output;
    }

    private void Train(double[] input, double[] targetOutput, double learningRate, double[,] weights1, double[,] weights2, double[,] weights3)
    {
        // Propagacija unaprijed
        Predict(input, weights1, weights2, weights3, bias1, bias2, bias3);

        // Propagacija unatrag
        double[] outputError = new double[OUTPUT];
        double tmpErr = 0;

        for (int j = 0; j < OUTPUT; j++)
        {
            outputError[j] = (targetOutput[j] - output[j]) * output[j] * (1 - output[j]);
            tmpErr += outputError[j];
        }

        double[] hidden2Error = new double[Size2];
        for (int i = 0; i < Size2; i++)
        {
            double sum = 0;
            for (int j = 0; j < OUTPUT; j++)
            {
                sum += outputError[j] * weights3[i, j];
            }
            hidden2Error[i] = sum * hidden2[i] * (1 - hidden2[i]);
        }

        double[] hidden1Error = new double[Size1];
        for (int i = 0; i < Size1; i++)
        {
            double sum = 0;
            for (int j = 0; j < Size2; j++)
            {
                sum += hidden2Error[j] * weights2[i, j];
            }
            hidden1Error[i] = sum * hidden1[i] * (1 - hidden1[i]);
        }

        // Ažuriranje težina i pomaka
        for (int j = 0; j < OUTPUT; j++)
        {
            for (int i = 0; i < Size2; i++)
            {
                weights3[i, j] = Math.Round(weights3[i, j] + learningRate * outputError[j] * hidden2[i], 6);
                line3 += weights3[i, j] + " ";
            }
            bias3[j] += learningRate * outputError[j]; if (t >= (p - 1) * CycleNumber - 1) bline3 += bias3[j] + " ";
            lines3[j] = line3.TrimEnd();
            line3 = "";
        }

        for (int j = 0; j < Size2; j++)
        {
            for (int i = 0; i < Size1; i++)
            {
                weights2[i, j] = Math.Round(weights2[i, j] + learningRate * hidden2Error[j] * hidden1[i], 6);
                line2 += weights2[i, j] + " ";
            }
            bias2[j] += learningRate * hidden2Error[j]; if (t >= (p - 1) * CycleNumber - 1) bline2 += bias2[j] + " ";
            lines2[j] = line2.TrimEnd();
            line2 = "";
        }

        for (int j = 0; j < Size1; j++)
        {
            for (int i = 0; i < INPUT; i++)
            {
                weights1[i, j] = Math.Round(weights1[i, j] + learningRate * hidden1Error[j] * input[i], 6);
                line1 += weights1[i, j] + " ";
            }
            bias1[j] += learningRate * hidden1Error[j]; if (t >= (p - 1) * CycleNumber - 1) bline1 += bias1[j] + " ";
            lines1[j] = line1.TrimEnd();
            line1 = "";
        }

        if (t >= (p - 1) * CycleNumber - 1)
        {
            fileContent1 = string.Join(Environment.NewLine, lines1);
            fileContent2 = string.Join(Environment.NewLine, lines2);
            fileContent3 = string.Join(Environment.NewLine, lines3);

            File.WriteAllText(weightsfilePath3, fileContent3); TransposeMatrix(weightsfilePath3);
            File.WriteAllText(weightsfilePath2, fileContent2); TransposeMatrix(weightsfilePath2);
            File.WriteAllText(weightsfilePath1, fileContent1); TransposeMatrix(weightsfilePath1);

            string[][] Biases = new string[][] { bline1.TrimEnd().Split(' '), bline2.TrimEnd().Split(' '), bline3.TrimEnd().Split(' ') };
            string directory = "Weights";
            string filename = "Biases.txt";
            string path = Path.Combine(directory, filename);
            File.WriteAllLines(path, Biases.Select(row => string.Join(" ", row)).Where(row => !string.IsNullOrWhiteSpace(row)));
        }

        // Transponira matricu
        static void TransposeMatrix(string localfilePath)
        {
            string[] lines = File.ReadAllLines(localfilePath);

            // Get the number of rows and columns
            int rows = lines.Length;
            int columns = lines[0].Split(' ').Length;

            double[,] matrix = new double[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                string[] elements = lines[i].Split(' ');
                for (int j = 0; j < columns; j++)
                {
                    matrix[i, j] = double.Parse(elements[j]);
                }
            }

            // Transpose the matrix
            double[,] transposedMatrix = new double[columns, rows];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    transposedMatrix[j, i] = matrix[i, j];
                }
            }

            // Write the transposed matrix to the file
            using (StreamWriter writer = new StreamWriter(localfilePath))
            {
                for (int i = 0; i < columns; i++)
                {
                    for (int j = 0; j < rows; j++)
                    {
                        writer.Write(transposedMatrix[i, j]);
                        if (j != rows - 1)
                        {
                            writer.Write(" ");
                        }
                    }
                    if (i != columns - 1)
                    {
                        writer.WriteLine();
                    }
                }
            }
        }
        if (ErrDiff <= tmpErr) throw new Exception(); //provjeava Overfitting
    }
    
    private NeuralNetwork(int inputSize, int hiddenSize1, int hiddenSize2, int outputSize)
    {
        // Inicijalizacija
        Random rand = new Random();

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
    }

    private static void CreateMatrix(int m, int n, int i)
    {
        // Create the matrix and initialize it to all 0's.
        double[,] matrix = new double[(int)m, (int)n];
        for (int row = 0; row < m; row++)
        {
            for (int col = 0; col < n; col++)
            {
                matrix[row, col] = 0.0;
            }
        }

        // Write the matrix to a file.
        // Create the "Weights" directory if it doesn't exist.
        string directory = "Weights";
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string fileName = $"weights{i}.txt";
        string DatafilePath = Path.Combine(directory, fileName);
        using (StreamWriter writer = new StreamWriter(DatafilePath))
        {
            for (int row = 0; row < m; row++)
            {
                for (int col = 0; col < n; col++)
                {
                    writer.Write(matrix[row, col]);
                    if (col < n - 1)
                    {
                        writer.Write(" ");
                    }
                }
                writer.WriteLine();
            }
        }
    }

    public static void CreateBiases(int size1, int size2, int output)
    {
        using (StreamWriter writer = new StreamWriter(biasesFile))
        {
            for (int i = 0; i < size1 - 1; i++)
            {
                writer.Write("0 ");
            }
            writer.WriteLine("0");

            for (int i = 0; i < size2 - 1; i++)
            {
                writer.Write("0 ");
            }
            writer.WriteLine("0");

            for (int i = 0; i < output - 1; i++)
            {
                writer.Write("0 ");
            }
            writer.Write("0");
            writer.WriteLine();
        }
    }

    private static double[,] LoadWeight(string inputFilePath)
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

    private double Sigmoid(double x)
    {
        return 1.0 / (1.0 + Math.Exp(-x));
    }

    private double Gradient(double x)
    {
        return Sigmoid(x) * (1 - Sigmoid(x));
    }
}
