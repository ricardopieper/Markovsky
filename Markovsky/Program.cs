namespace Markovsky
{
    class Program
    {
        static void Main(string[] args)
        {
            new NgramMarkovChain().Run(ngramSize: 2, file: "training.txt");
        }
    }
}
