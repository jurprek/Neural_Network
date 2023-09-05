partial class NeuralNetwork
{
    // Define Neural Network.
    private static int INPUT = 3;       //Titanic=3, Bank=9
    private static int Size1 = 30;
    private static int Size2 = 54;
    private static int OUTPUT = 1;      //Titanic=1, Bank=1
    private static int CycleNumber = 300;
    private static double ErrSlip = 2.25;
    private static double learningRateMin = 0.05;
    private static double learningRateMax = 0.25;
    private static double towardsNull = 0.50;

    // Load data from .CSV & .txt files.
    //private static string TrainDataPath = @"data\kaggle\bankmarketing\Train001.csv";
    //private static string TestDataPath = @"data\kaggle\bankmarketing\Testbankcard.csv";
    private static string TrainDataPath = @"Data\Kaggle\Titanic\TrainTitanicData.csv";
    private static string TestDataPath = @"Data\Kaggle\Titanic\TestTitanicData.csv";
    //private static string TrainDataPath = @"data\kaggle\EURUSD\EURUSD_TrainData.csv";
    //private static string TestDataPath = @"data\kaggle\EURUSD\EURUSD_TrainData.csv";

    private static string weightsfilePath1 = @"Weights\weights1.txt";
    private static string weightsfilePath2 = @"Weights\weights2.txt";
    private static string weightsfilePath3 = @"Weights\weights3.txt";
    private static string biasesFile = @"Weights\Biases.txt";
}