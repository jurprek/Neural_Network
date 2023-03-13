using System;
using System.IO;
using System.Linq;
using ExcelDataReader;
using static OfficeOpenXml.ExcelErrorValue;

class NeuralNetwork
{ 
    // Definicija mreže.
    static int Ulaz = 10;    //  <----------------------------------------------------------------------
    static int Size1 = 20;
    static int Size2 = 20;
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

    string weightsfilePath1 = @"Weights\weights1.txt";
    string weightsfilePath2 = @"Weights\weights2.txt";
    string weightsfilePath3 = @"Weights\weights3.txt";

    string[] lines1 = new string[Size1];
    string[] lines2 = new string[Size2];
    string[] lines3 = new string[Izlaz];

    string fileContent1;
    string fileContent2;
    string fileContent3;

    double ErrDiff = 999999999;
    static int Ciklus = 1;
    static int qTrain;

    //Funkcija kreira weights.txt file-ove
    public static void CreateMatrix(int m, int n, int i)
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
        string filePath = Path.Combine(directory, fileName);
        using (StreamWriter writer = new StreamWriter(filePath))
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

    static void Main(string[] args)
    {
        Console.Write("Train NeuralNetwork (1) or skip to Predict (0)?: ");
        qTrain = Convert.ToInt32(Console.ReadLine());
        if (qTrain == 1)
        {
            Console.Write("Start from existing WeightMatrices (1) or generate new ones (0)?: "); int qWeights = Convert.ToInt32(Console.ReadLine());

            if (qWeights == 0)
            {
                //Kreiraj weights1.txt
                CreateMatrix(Ulaz, Size1, 1);

                //Kreiraj weights1.txt
                CreateMatrix(Size1, Size2, 2);

                //Kreiraj weights1.txt
                CreateMatrix(Size2, Izlaz, 3);
            }

            Console.Write("Number of training cycles?: ");
            Ciklus = Convert.ToInt32(Console.ReadLine());
        }

        System.Globalization.CultureInfo customCulture = new System.Globalization.CultureInfo("en-US");
        System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        // Učitavanje podataka iz CSV datoteke.
        string filePath = @"Data\DataSet01.csv";

        List<double[]> inputs = new List<double[]>();
        List<double[]> targetOutputs = new List<double[]>();

        using (var reader = new StreamReader(filePath))
        {
                int rowNumber = 0;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');


                    // Ignoriraj redove zaglavlja.
                    if (rowNumber > 0)
                    {
                        double[] input = new double[Ulaz];
                        for (int m = 0; m < Ulaz; m++)
                        {
                            input[m] = double.Parse(values[m], customCulture);
                        }

                        double[] output = new double[Izlaz];
                        for (int m = 0; m < Izlaz; m++)
                        {
                            output[m] = double.Parse(values[m + Ulaz], customCulture);
                        }

                        targetOutputs.Add(output);
                        inputs.Add(input);
                    }

                    rowNumber++;
                }
        }

        //Učitavanje matrica težina
        double[,] weights1 = LoadWeight (@"Weights\weights1.txt");
        for (int i = 0; i < Ulaz; i++){
            for (int j = 0; j < Size1; j++)
                Console.Write(Math.Round(weights1[i, j], 6) + " "); Console.WriteLine();
        }
        Console.WriteLine();

        double[,] weights2 = LoadWeight(@"Weights\weights2.txt");
        for (int i = 0; i < Size1; i++){
            for (int j = 0; j < Size2; j++) 
                Console.Write(Math.Round(weights2[i, j], 6) + " "); Console.WriteLine();
        }
        Console.WriteLine();

        double[,] weights3 = LoadWeight(@"Weights\weights3.txt");
        for (int i = 0; i < Size2; i++){
            for (int j = 0; j < Izlaz; j++)
                Console.Write(Math.Round(weights3[i, j], 6) + " "); Console.WriteLine();
        }
        Console.WriteLine();

        //-----------------------------------------------------------------
        NeuralNetwork nn = new NeuralNetwork(Ulaz, Size1, Size2, Izlaz);  //
        //-----------------------------------------------------------------


        // start: Treniranje mreže.
        if(qTrain == 1)
        {
            int epochs = Ulaz;
            for (int j = 0; j < inputs.Count; j++)
            {
                double learningRate = 0.25;
                bool bjeg = false;

                for (int r = 0; r < Ciklus; r++)
                {   //   <---------------------------------------------------------------------------------- broj iteracija po istom uzorku
                
                    if (learningRate < 0.01) learningRate = 0.01;
                    else learningRate *= 0.9995;
                    try{
                        nn.Train(inputs[j], targetOutputs[j], learningRate, weights1, weights2, weights3);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(">>> Overfitting! <<<"); Console.WriteLine();
                        bjeg = true;
                        break;
                    }   
                }
                if (bjeg) break;
            }
        }

        //Testiraj na ovom uzorku
        double[,] primjer = {
            {  0.972315279830527,0.374642024214942,-0.987362755776673,-0.550211203146455,0.788607188358155,0.897486042559677,0.897486042559677,0.887676263161404,-0.141606483918327,-0.126034924581164},
            { -0.822590193090002,-0.829950048594149,0.919303705505362,-0.104787994048832,-0.799143611506213,0.769355431291827,0.769355431291827,0.280829713151262,0.152814519273307,0.650792136858826 },
            { 0.0180173416102605,0.163539814066219,-0.78031753831957,0.924620991692989,0.489977057513468,-0.322902370718307,-0.322902370718307,0.918334829309936,0.0175477707615459,-0.857574538568424},
            { -0.162241520603104,0.467550018912954,0.503584774405004,0.302621454351558,-0.472022306547612,-0.562927387593802,-0.562927387593802,0.755581321267899,-0.727943693132739,-0.202007532644133},
            { 0.095282151514809,-0.927529991180237,-0.450603425779466,0.893877510741974,0.351686009547855,0.173117707646684,0.173117707646684,0.530642510022081,0.892500750634307,-0.365627601639181 },
            { -0.00807763161823916,0.036010762709322,0.573810322741297,-0.602771749198344,-0.598629596685094,-0.72817068009583,-0.72817068009583,-0.330994627889557,-0.551537461608604,-0.664896316195539},
            { 0.564437629958376,0.421345762891236,-0.688534848698745,0.629055958173294,0.422057358002237,0.0329316342570658,0.0329316342570658,0.560384315818087,0.504107716907942,0.413131305297761 },
            { -0.420469366336617,0.637120561366121,0.62888638772556,0.693932139812637,0.291595714364456,0.242603724772777,0.242603724772777,-0.314153096670346,0.171132364978348,0.430001860480477 },
            { -0.277106315691558,-0.386685987179348,-0.570390036176618,0.175345139620588,0.300479191307506,-0.0681738561201917,-0.0681738561201917,0.883986898124351,0.531021570641045,-0.430903996266799 },
            { 0.151882095742381,-0.402516463802958,-0.892337557523416,-0.986684500213489,-0.354798839816851,-0.700418534732024,-0.700418534732024,0.973961986307434,0.252482999942402,0.911639153900963 },
            { -0.533992322562857,-0.159563976371338,0.870878756696713,0.687410612854526,0.734772640549174,-0.598173712359326,-0.598173712359326,0.478471252393423,-0.646216993107023,0.322958005653977 },
            { 0.245996705236797,-0.673632429800152,-0.193009831503513,0.951960423305128,-0.901811771194054,0.795672572647149,0.795672572647149,0.607335307082253,0.781713430136677,0.379359954504645 },
            { -0.716406356778469,0.342288607405709,0.704905800097272,0.719065492508729,0.407180036063208,-0.871458188322933,-0.871458188322933,-0.518344716321813,0.680253999722748,-0.557448281130896 },
            { 0.88703542756185,-0.144216169215548,-0.679668687689177,0.995030238813455,-0.506271038165337,-0.318888703150188,-0.318888703150188,0.363771548826103,-0.382860066535434,0.919266629107762}
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
                for (int i = 0; i < Ulaz; i++)
                {
                    Sgm += x[i];
                }
                return 1- 1 / (1 + Math.Exp(-Sgm));
            }

            double[] OutputedVals = nn.Predict(Row, weights1, weights2, weights3);
            
            Console.WriteLine(Math.Round(OutputedVals[0]*100, 6) + " %.");
           
            Console.WriteLine(" ---> " + Math.Round(SigmoidajRedak(Row)*100, 2) + " %."); 
            Console.WriteLine("-------------");
        }
        Console.WriteLine("Done.");
        Console.ReadLine();
    }

    private NeuralNetwork(int inputSize, int hiddenSize1, int hiddenSize2, int outputSize)
    {
        this.inputSize = inputSize;
        this.hiddenSize1 = hiddenSize1;
        this.hiddenSize2 = hiddenSize2;
        this.outputSize = outputSize;
        
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
        return  1.0 / (1.0 + Math.Exp(-x));
    }
    private double Gradient(double x)
    {
        return Sigmoid(x) * ( 1 - Sigmoid(x) );
    }

    public double[] Predict(double[] input, double[,] weights1, double[,] weights2, double[,] weights3)
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
        
        if(qTrain == 1)
        {
            File.WriteAllText(weightsfilePath3, fileContent3); TransposeMatrix(weightsfilePath3);
            File.WriteAllText(weightsfilePath2, fileContent2); TransposeMatrix(weightsfilePath2);
            File.WriteAllText(weightsfilePath1, fileContent1); TransposeMatrix(weightsfilePath1);
        }


        // Transponira matricu
        static void TransposeMatrix(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);

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
            using (StreamWriter writer = new StreamWriter(filePath))
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
        return output;
    }

    //Treniranje Neuralne Mreže --------------------------------------------------
    public void Train(double[] input, double[] targetOutput, double learningRate, double[,] weights1, double[,] weights2, double[,] weights3)
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
        

        // Propagacija unatrag
        double[] outputError = new double[outputSize];
        double tmpErr = 0;

        for (int j = 0; j < outputSize; j++)
        {
            outputError[j] = (targetOutput[j] - output[j]) * output[j] * (1 - output[j]);
            tmpErr += outputError[j];
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
                weights3[i, j] = Math.Round(weights3[i, j] + learningRate * outputError[j] * hidden2[i], 6);
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
                weights2[i, j] = Math.Round(weights2[i, j] + learningRate * hidden2Error[j] * hidden1[i], 6);
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
                weights1[i, j] = Math.Round(weights1[i, j] + learningRate * hidden1Error[j] * input[i], 6);
                line1 += weights1[i, j] + " ";
            }
            bias1[j] += learningRate * hidden1Error[j];
            lines1[j] = line1.TrimEnd();
            line1 = "";
        }

        fileContent1 = string.Join(Environment.NewLine, lines1);
        fileContent2 = string.Join(Environment.NewLine, lines2);
        fileContent3 = string.Join(Environment.NewLine, lines3);

        if (ErrDiff <= tmpErr) throw new Exception(); //provjeava Overfitting
    }

    //Učitava težine iz .txt fileova
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