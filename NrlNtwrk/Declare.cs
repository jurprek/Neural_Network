partial class NeuralNetwork
{
    // Define Neural Network.
    private static int INPUT = 9;       //Titanic=3, Bank=9
    private static int Size1 = 270;
    private static int Size2 = 540;
    private static int OUTPUT = 1;      //Titanic=1, Bank=1
    private static int CycleNumber = 5;
    private static double ErrSlip = 1.75;
    private static double learningRateMin = 0.10;
    private static double learningRateMax = 10;
    private static double towardsNull = 0.50;

    // Load data from .CSV & .txt files.
    private static string TrainDataPath = @"data\kaggle\bankmarketing\Train007.csv";
    private static string TestDataPath = @"data\kaggle\bankmarketing\Testbankcard.csv";
    //private static string TrainDataPath = @"Data\Kaggle\Titanic\TrainTitanicData.csv";
    //private static string TestDataPath = @"Data\Kaggle\Titanic\TestTitanicData.csv";

    private static string weightsfilePath1 = @"Weights\weights1.txt";
    private static string weightsfilePath2 = @"Weights\weights2.txt";
    private static string weightsfilePath3 = @"Weights\weights3.txt";
    private static string biasesFile = @"Weights\Biases.txt";
}