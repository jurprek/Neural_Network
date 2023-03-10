using System;
using System.IO;
using System.Linq;
using ExcelDataReader;
using static OfficeOpenXml.ExcelErrorValue;

class NeuralNetwork
{ 
    // Definicija mreže.
    static int Ulaz = 5;    //  <----------------------------------------------------------------------
    static int Size1 = 10;
    static int Size2 = 10;
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
        string filePath = @"C:\Users\jpreksavec\Desktop\DataSet02.xlsx";
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
                            output[m] = reader.GetDouble(m);
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
            double learningRate = 0.10;

            for (int r = 0; r < 1000; r++){   //   <---------------------------------------------------------------------------------- broj iteracija po istom uzorku
                if (learningRate < 0.005) learningRate = 0.005;
                else learningRate *= 0.99995;
                nn.Train(inputs[j], targetOutputs[j], learningRate); /*Console.Write(100 - r);*/              

            }
        }


        double[,] primjer = {
         /*   { 1, 1, 1, 1, 1 },
            { 0, 0, 0, 0, 0},
            { -1, -1, -1, -1, -1},
        */
           { -0.960151084445761,-0.522024543102316,-0.283819523839298,0,0 },
           {-0.635073104133522,-0.345282895986355,-0.187726857718821,0,0 },
           { 0,0,0.319639258322947,0.639278516645894,1.17581437334859 },
           { -0.898146721350269,-0.488313390930928,-0.265491107515236,0,0 },
           { -1.28417392540948,-0.69819252150588,-0.379600292017532,0,0 },
           { 0,0,0.231927631939479,0.463855263878957,0.853161293897179 },
           { -0.338334731091656,-0.183949209947223,-0.100011345956206,0,0 },
           { 0,0,0.079752311547507,0.159504623095014,0.293374207903426 },
           { -0.210409582783204,-0.1143975860634,-0.06219682356681,0,0 },
           { -0.204962595565051,-0.111436113582763,-0.0605867005938122,0,0 },
           { -0.325676931179689,-0.177067290713085,-0.0962697152877974,0,0 },
           { -0.329706297726075,-0.179258017010689,-0.0974607912685418,0,0 },
           { -0.463649949529699,-0.252081841059802,-0.137054376165804,0,0 },
           { -0.913186111233959,-0.496490156816791,-0.269936733359721,0,0 },
           { 0,0,0.203134090745992,0.406268181491985,0.747242328334133 },
           { -0.685963109235453,-0.372951282860223,-0.202769882978275,0,0 },
           { -1.14460598181805,-0.622310826254653,-0.338344173126392,0,0 },
           { 0,0,0.333975368236926,0.667950736473853,1.22855071175459 },
           { -0.608078492827658,-0.330606195765469,-0.179747282575728,0,0 },
           { 0,0,0.306192338234765,0.61238467646953,1.12634897914166 },
           { -0.864943629529141,-0.470261201938698,-0.255676312882051,0,0 },
           { -0.054651941273419,-0.0297137139509452,-0.0161550491379894,0,0 },
           { -1.04323919682635,-0.567198718922518,-0.308380271490984,0,0 },
           { -0.584293379230526,-0.317674467353845,-0.172716431155612,0,0 },
           { -0.24610301883102,-0.133803750308237,-0.0727477610050856,0,0 },
           { 0,0,0.261216398662313,0.522432797324626,0.960901979666034 },
           { -1.27583616317497,-0.693659363556601,-0.377135659372088,0,0 },
           { -0.506295730975247,-0.275267925973932,-0.149660418672767,0,0 },
           { -1.06607843646753,-0.579616185123029,-0.31513152368954,0,0 },
           { 0,0,0.267099875753598,0.534199751507197,0.982544743341236 },
        };

        int red;
        for (int t = 0; t < primjer.GetLength(0); t++) //    <--------------------------------------------------------------BROJ REDOVA (UNESENIH PRIMJERA) !!!!!!!!!!!!!!!!
        {
            red = t;
            double[] Row = new double[primjer.GetLength(1)];
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

            double[] OutputedVals = nn.Predict(Row, 0); //  <------------------------------------------------------------------------ 0 (ili 1 za detalje)

            for (int i = 0; i < Izlaz; i++) {
                Console.WriteLine(Math.Round(OutputedVals[i]*100, 2) + " %.");
            }
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
            bias1[i] =  rand.NextDouble() * 2 - 1;
        }

        bias2 = new double[hiddenSize2];
        for (int i = 0; i < hiddenSize2; i++)
        {
            bias2[i] =  rand.NextDouble() * 2 - 1;
        }

        bias3 = new double[outputSize];
        for (int i = 0; i < outputSize; i++)
        {
            bias3[i] =  rand.NextDouble() * 2 - 1;
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
        double[] targetOutput = new double[outputSize];
        for (int i = 0; i < outputSize; i++) targetOutput[i] = 0;
        
        double[] outputError = new double[outputSize];
        for (int i = 0; i < outputSize; i++) outputError[i] = 0;

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
        
        File.WriteAllText(weightsfilePath3, fileContent3);
        File.WriteAllText(weightsfilePath2, fileContent2);
        File.WriteAllText(weightsfilePath1, fileContent1);
        
        return output;
    }



    //Treniranje Neuralne Mreže --------------------------------------------------
    public void Train(double[] input, double[] targetOutput, double learningRate)
    {
        // Propagacija unaprijed
        for (int j = 0; j < hiddenSize1; j++)
        {
            double sum = 0;
            for (int i = 0; i < inputSize; i++)
            {
                sum += input[i] * weights1[i, j];
            }
            hidden1[j] = Sigmoid(sum + bias1[j]);
        }

        for (int j = 0; j < hiddenSize2; j++)
        {
            double sum = 0;
            for (int i = 0; i < hiddenSize1; i++)
            {
                sum += hidden1[i] * weights2[i, j];
            }
            hidden2[j] = Sigmoid(sum + bias2[j]);
        }

        for (int j = 0; j < outputSize; j++)
        {
            double sum = 0;
            for (int i = 0; i < hiddenSize2; i++)
            {
                sum += hidden2[i] * weights3[i, j];
            }
            output = new double[outputSize];
            output[j] = Sigmoid(sum + bias3[j]);
        }


        // Propagacija unatrag
        double[] outputError = new double[outputSize];
        for (int i = 0; i < outputSize; i++)
        {
            outputError[i] = (targetOutput[i] - output[i]) * output[i] * (1 - output[i]);
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
                weights3[i, j] += learningRate * outputError[j] * output[j];
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
                weights2[i, j] += learningRate * hidden2Error[j] * hidden2[j];
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
                weights1[i, j] += learningRate * hidden1Error[j] * hidden1[j];
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