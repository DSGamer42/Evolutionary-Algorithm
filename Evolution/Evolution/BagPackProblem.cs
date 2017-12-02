using System;

namespace Evolution
{
    public class BagPackProblem // Used a Testcase with the optimal Result Value:309 - Weight:165
    {
        public Random r = new Random(42);
        public int MaxWeigth = 165;
        public BagPack BagPack = new BagPack();
        public Evo GeneticAlgo;

        private int _itemNumber = 10;
        private int _maxValue = 100;
        private int _maxWeight = 100;
        private int _startPop = 10000;
        

        public BagPackProblem()
        {
            BagPack.TransformIntoTestBag();
            Console.WriteLine($"Bag Initialized with a MaxWeight of {MaxWeigth} and a MaxWeight regardless of Limitations of {BagPack.EverythingWeight}");
        }

        public void InitBag()
        {
            BagPack.Content = new Item[_itemNumber];
            for (int i = 0; i < _itemNumber; i++)
            {
                BagPack.Content[i] = new Item(r.Next(_maxValue), r.Next(_maxWeight), $"{i}");
            }
        }

        public int FitnesFunc(bool[] genomCode)
        {
            int weight = 0;
            int value = 0;

            for (int i = 0; i < genomCode.Length; i++)
            {
                if (genomCode[i])
                {
                    weight += BagPack[i].Weight;
                    value += BagPack[i].Value;

                    if (weight > MaxWeigth)
                        return 1;
                }
            }
            return value;
        }

        public void Evaluate()
        {
            bool[] tmp = GeneticAlgo.FittestElement();
            Console.WriteLine("Best Result:");
            Console.WriteLine($"Value: {FitnesFunc(tmp)}, Weight: {WeightFromGenom(tmp)}");
            Console.WriteLine("Content:");

            for (int i = 0; i < _itemNumber; i++)
            {
                Console.WriteLine($"{BagPack[i].Name}: {(tmp[i]? "yes" : "no")}");
            }
            Console.ReadKey();
        }

        public int WeightFromGenom(bool[] code)
        {
            int n = 0;
            for (int i = 0; i < code.Length; i++)
            {
                if (code[i])
                    n += BagPack[i].Weight;
            }
            return n;
        }

        public void StartBpTest(int gens)
        {
            GeneticAlgo = new Evo(_startPop, FitnesFunc, _itemNumber,0.5f,0.004f); // better Values than the usual ones
            GeneticAlgo.CalculateGenerations(gens);
            Evaluate();
        }
    }

    public class Item
    {
        public int Value;
        public int Weight;
        public string Name;

        public Item(Random r)
        {
            Value = r.Next(0, 100 + 1);
            Weight = r.Next(0, 100 + 1);
        }

        public Item(int value, int weight, string name)
        {
            Value = value;
            Weight = weight;
            Name = name;
        }
    }

    public class BagPack
    {
        public Item[] Content;

        public Item this[int n]
        {
            get => Content[n];
            set => Content[n] = value;
        }
        
        public int EverythingWeight
        {
            get
            {
                int n = 0;
                foreach (Item i in Content)
                {
                    n += i.Weight;
                }
                return n;
            }
        }

        public void TransformIntoTestBag()
        {
            Content = new Item[10];
            Content[0] = new Item(92, 23, "1");
            Content[1] = new Item(57, 31, "2");
            Content[2] = new Item(49, 29, "3");
            Content[3] = new Item(68, 44, "4");
            Content[4] = new Item(60, 53, "5");
            Content[5] = new Item(43, 38, "6");
            Content[6] = new Item(67, 63, "7");
            Content[7] = new Item(84, 85, "8");
            Content[8] = new Item(87, 89, "9");
            Content[9] = new Item(72, 82, "10");
        }
    }
}
