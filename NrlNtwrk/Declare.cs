partial class NeuralNetwork
{
    // Define Neural Network.
    private static int INPUT = 5;
    private static int Size1 = 6;
    private static int Size2 = 4;
    private static int OUTPUT = 1;
    private static int CycleNumber = 10;

    // Load data from .CSV & .txt files.
    private static string DatafilePath = @"Data\Titanic.csv";
    private static string weightsfilePath1 = @"Weights\weights1.txt";
    private static string weightsfilePath2 = @"Weights\weights2.txt";
    private static string weightsfilePath3 = @"Weights\weights3.txt";
    private static string biasesFile = @"Weights\Biases.txt";
}