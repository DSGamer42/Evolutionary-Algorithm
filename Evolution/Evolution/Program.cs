using System;

namespace Evolution
{
    class Program
    {
        static void Main(string[] args)
        {
            BagPackProblem b = new BagPackProblem();
            Console.WriteLine("How many Generations?");
            b.StartBpTest(Convert.ToInt32(Console.ReadLine()));
        }
    }
}
