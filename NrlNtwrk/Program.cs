using System;
using System.IO;
using System.Linq;
using ExcelDataReader;
using static OfficeOpenXml.ExcelErrorValue;

class NeuralNetwork
{ 
    // Definicija mreže.
    static int Ulaz = 5;    //  <----------------------------------------------------------------------
    static int Size1 = 6;
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

    string weightsfilePath1 = @"C:\Users\Jurica\Desktop\weights1.txt";
    string weightsfilePath2 = @"C:\Users\Jurica\Desktop\weights2.txt";
    string weightsfilePath3 = @"C:\Users\Jurica\Desktop\weights3.txt";

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
        string filePath = @"C:\Users\Jurica\Desktop\DataSet01.xlsx";

        List<double[]> inputs = new List<double[]>();
        List < double[]> targetOutputs = new List<double[]>();

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
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

                    }
                }
            }
        }
            
        //Učitavanje matrica težina
        double[,] weights1 = LoadWeight("C:/Users/Jurica/Desktop/weights1.txt");
        for (int i = 0; i < Ulaz; i++){
            for (int j = 0; j < Size1; j++)
                Console.Write(Math.Round(weights1[i, j], 6) + " "); Console.WriteLine();
        }
        Console.WriteLine();

        double[,] weights2 = LoadWeight("C:/Users/Jurica/Desktop/weights2.txt");
        for (int i = 0; i < Size1; i++){
            for (int j = 0; j < Size2; j++) 
                Console.Write(Math.Round(weights2[i, j], 6) + " "); Console.WriteLine();
        }
        Console.WriteLine();

        double[,] weights3 = LoadWeight("C:/Users/Jurica/Desktop/weights3.txt");
        for (int i = 0; i < Size2; i++){
            for (int j = 0; j < Izlaz; j++)
                Console.Write(Math.Round(weights3[i, j], 6) + " "); Console.WriteLine();
        }
        Console.WriteLine();

        //-----------------------------------------------------------------
        NeuralNetwork nn = new NeuralNetwork(Ulaz, Size1, Size2, Izlaz);  //
        //-----------------------------------------------------------------


        // start: Treniranje mreže.
        
        int epochs = Ulaz;
        for (int j = 0; j < inputs.Count; j++)
        {
            double learningRate = 0.1;

            for (int r = 0; r < 100; r++){   //   <---------------------------------------------------------------------------------- broj iteracija po istom uzorku
                if (learningRate < 0.005) learningRate = 0.005;
                else learningRate *= 0.99;
                
                nn.Train(inputs[j], targetOutputs[j], learningRate, weights1, weights2, weights3);        

            }
        }

        //Testiraj na ovom uzorku
        double[,] primjer = {
           {  0.7, 0.65, 0.25, 0.40, 0.15 },
           {  0,   0,    0,    0,    0    },
           { -0.7,-0.65,-0.25,-0.40,-0.15 },
           {-0.275041350075365,0.609512040645118,-0.83777580922789,-0.20984521389192,0.778961697337921},
            {-0.275041350075365,0.609512040645118,-0.83777580922789,-0.20984521389192,0.778961697337921},
            {-0.470940876207609,0.0743824581498598,-0.477193069242254,0.572612197069567,-0.262232320822954},
            {-0.132444787928778,0.946373277203434,0.976211552101984,0.699576485845871,-0.330044130862337},
            {0.924109948241495,-0.813619135832025,-0.70162142206353,0.859295528989206,0.88952142232623},
            {-0.475048263000629,0.890088138776809,-0.797315837703715,-0.899594549835415,-0.218311082104202},
            {0.01412294726304,0.266968986226497,0.428884784375237,-0.138488191437025,0.582773174663588},
            {0.869186035268081,0.41889337170468,-0.325843710547832,-0.337792098795935,-0.871283864762525},
            {0.386393239292518,0.814454662164547,-0.737735249746914,0.588876136690549,-0.637288209636352},
            {-0.741490740356795,-0.948050019690307,0.954370173242092,-0.791420584633522,0.449408181588992},
            {-0.400263550694755,0.518755543168551,-0.908667414699385,-0.756752170934909,-0.203019824163141},
            {0.685109568283524,-0.572582952944771,0.371628423234307,0.121135212848419,-0.714661307354033},
            {-0.449783254862828,0.459323977001248,0.579335864048244,-0.897447537882985,0.668867336485821},
            {-0.12671323388057,-0.622250340959255,-0.682604940112916,-0.767555352470545,0.946470298331044},
            {-0.295490388879467,-0.981781208671966,-0.494302732780594,-0.229986255502789,0.197074790847524},
            {-0.120691255689762,-0.0583388106645468,0.810586179674496,-0.887143738558362,0.285246455348275},
            {0.415059097668001,-0.226507129028569,0.615706091804598,-0.287149462600236,0.358131064473005},
            {0.808498031933762,0.604560363445248,0.228750151592554,0.619950185177681,0.0487232565733033},
            {-0.0784774055075883,-0.0604516161928781,-0.784584585565156,-0.948087861031031,-0.656900520512969},
            {-0.455949965426837,-0.827580224479949,-0.0961337873784271,-0.71685112320409,-0.263600853582687},
            {0.7146347846636,0.993536914607503,0.618853688197704,0.626592551675728,0.55919726878289},
            {0.673870657629575,0.494498261362984,0.180109037686892,-0.234524485585144,-0.631636343115023},
            {-0.982213459886085,0.282323488658628,0.352833695175605,-0.210610750757305,0.201810379216961},
            {-0.329185685127236,-0.673628030659662,0.273472015389684,-0.275333812253375,-0.711685250754057},
            {0.9120980438339,-0.279572335163764,0.189635491410045,0.977571157371985,-0.917277692073761},
            {-0.184748304709361,0.74997557592164,-0.978003665750006,0.0315159237574023,-0.984357055712029},
            {0.619091864968979,-0.866685372981821,0.507700089103159,-0.149606486962195,0.824706343819546},
            {0.618944426958471,-0.654155970114822,-0.421389715372646,0.125029227567311,-0.840252311573113},
            {0.235690200253333,-0.93993489358436,0.407204608510173,-0.993079223881019,0.498041686761066},
            {0.778776527152206,-0.791388789024887,0.303306070325474,-0.22628589772784,-0.703347186113185},
            {-0.254609727964867,-0.844295410264514,-0.987699225039101,-0.933290471719669,-0.572145135854391}
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
                return 1- 1 / (1 + Math.Exp(-Sgm));
            }

            double[] OutputedVals = nn.Predict(Row, 0, weights1, weights2, weights3); //  <------------------------------------------------------------------------ 0 (ili 1 za detalje)
            
            Console.WriteLine(Math.Round(OutputedVals[0]*100, 6) + " %.");
           
            Console.WriteLine(" ---> " + Math.Round(SigmoidajRedak(Row)*100, 2) + " %."); 
            Console.WriteLine("-------------");
        }
        //Console.ReadLine();
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


    public double[] Predict(double[] input, int x, double[,] weights1, double[,] weights2, double[,] weights3)
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
        
        File.WriteAllText(weightsfilePath3, fileContent3); TransposeMatrix(weightsfilePath3);
        File.WriteAllText(weightsfilePath2, fileContent2); TransposeMatrix(weightsfilePath2);
        File.WriteAllText(weightsfilePath1, fileContent1); TransposeMatrix(weightsfilePath1);


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